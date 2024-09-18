using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Localization;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;
using Offers.CleanArchitecture.Domain.Events.OnboardingPageLocalizationEvents;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Commands.UpdateOnboardingPage;
public class UpdateOnboardingPageCommand : IRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Order { get; set; }
    public FileDto? Asset { get; set; } = null!;
    public List<OnboardingPageLocalizationApp> OnboardingPageLocalizations { get; set; } = new List<OnboardingPageLocalizationApp>();
    public List<OnboardingPageLocalizationAssetApp> OnboardingPageLocalizationAssets { get; set; } = new List<OnboardingPageLocalizationAssetApp>();
    public List<Guid> DeletedLocalizedAssetsIds { get; set; } = new List<Guid>();

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateOnboardingPageCommand, OnboardingPage>()
                .ForMember(dest => dest.AssetPath, g => g.Ignore());
        }


    }

    public class UpdateOnboardingPageCommandHandler : IRequestHandler<UpdateOnboardingPageCommand>
    {
        private readonly ILogger<UpdateOnboardingPageCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IOnboardingPageRepository _onboardingPageRepository;
        private readonly IOnboardingPageLocalizationRepository _onboardingPageLocalizationRepository;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IFileService _fileService;
        private readonly ILanguageRepository _languageRepository;

        public UpdateOnboardingPageCommandHandler(ILogger<UpdateOnboardingPageCommandHandler> logger,
                                                  IMapper mapper,
                                                  IOnboardingPageRepository onboardingPageRepository,
                                                  IOnboardingPageLocalizationRepository onboardingPageLocalizationRepository,
                                                  IUnitOfWorkAsync unitOfWork,
                                                  IFileService fileService,
                                                  ILanguageRepository languageRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _onboardingPageRepository = onboardingPageRepository;
            _onboardingPageLocalizationRepository = onboardingPageLocalizationRepository;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _languageRepository = languageRepository;
        }

        public async Task Handle(UpdateOnboardingPageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var existingOnboardPage = await _onboardingPageRepository.GetByIdAsync(request.Id);
                var oldAssetPath = existingOnboardPage.AssetPath;

                _mapper.Map(request, existingOnboardPage);

                //check asset to update
                if (request.Asset is not null)
                {
                    await _fileService.DeleteFileAsync(oldAssetPath);
                    var newAssetPath = await _fileService.UploadFileAsync(request.Asset);
                    existingOnboardPage.AssetPath = newAssetPath;
                }
                else
                    existingOnboardPage.AssetPath = oldAssetPath;

                // delete Localization for Title,Description
                var onboardingPageLocalizationListToDelete = await _onboardingPageLocalizationRepository.GetAll()
                    .Where(l => l.OnboardingPageId == request.Id &&
                    (l.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.Title 
                    || l.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.Description))
                    .ToListAsync();

                foreach (var onboardingPageLocalizationToDelete in onboardingPageLocalizationListToDelete)
                {
                    await _onboardingPageLocalizationRepository.DeleteAsync(onboardingPageLocalizationToDelete);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //add new Localization for Title,Description
                foreach (var onboardingLocalization in request.OnboardingPageLocalizations)
                {
                    var language = await _languageRepository.GetByIdAsync(onboardingLocalization.LanguageId);
                    OnboardingPageLocalization onboardingPageLocalizationToAdd = new OnboardingPageLocalization();
                    //add foreign keys
                    onboardingPageLocalizationToAdd.LanguageId = language.Id;
                    onboardingPageLocalizationToAdd.OnboardingPageId = existingOnboardPage.Id;
                    //add localization
                    onboardingPageLocalizationToAdd.OnboardingPageLocalizationFieldType = (int)onboardingLocalization.FieldType;
                    onboardingPageLocalizationToAdd.Value = onboardingLocalization.Value;
                    //save and seed event
                    await _onboardingPageLocalizationRepository.AddAsync(onboardingPageLocalizationToAdd);
                    onboardingPageLocalizationToAdd.AddDomainEvent(new OnboardingPageLocalizationCreatedEvent(onboardingPageLocalizationToAdd));
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                // delete old localization assets
                var ListOfPathsOfAssetsToDelete = new List<string>();
                foreach (var id in request.DeletedLocalizedAssetsIds)
                {
                    var onboardingPageAssetToDelete = await _onboardingPageLocalizationRepository.GetByIdAsync(id);
                    string assetPathToDelete = onboardingPageAssetToDelete.Value;
                    await _onboardingPageLocalizationRepository.DeleteAsync(onboardingPageAssetToDelete);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    ListOfPathsOfAssetsToDelete.Add(assetPathToDelete);
                }

                //add new Localization for asset
                foreach (var onboardingPageLocalizationAsset in request.OnboardingPageLocalizationAssets)
                {
                    var language = await _languageRepository.GetByIdAsync(onboardingPageLocalizationAsset.LanguageId);
                    OnboardingPageLocalization onboardingPageLocalizationToAdd = new OnboardingPageLocalization();
                    //add foreign keys
                    onboardingPageLocalizationToAdd.LanguageId = language.Id;
                    onboardingPageLocalizationToAdd.OnboardingPageId = existingOnboardPage.Id;
                    //add localization
                    onboardingPageLocalizationToAdd.OnboardingPageLocalizationFieldType = (int)OnboardingPageLocalizationFieldType.AssetPath;
                    var assetPathLocalization = await _fileService.UploadFileAsync(onboardingPageLocalizationAsset.Asset);
                    onboardingPageLocalizationToAdd.Value = assetPathLocalization;
                    //save and seed event
                    await _onboardingPageLocalizationRepository.AddAsync(onboardingPageLocalizationToAdd);
                    onboardingPageLocalizationToAdd.AddDomainEvent(new OnboardingPageLocalizationCreatedEvent(onboardingPageLocalizationToAdd));
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                await _unitOfWork.CommitAsync();

                //delete assets from upload folder
                foreach (var pathOfAssetToDelete in ListOfPathsOfAssetsToDelete)
                {
                    await _fileService.DeleteFileAsync(pathOfAssetToDelete);
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
