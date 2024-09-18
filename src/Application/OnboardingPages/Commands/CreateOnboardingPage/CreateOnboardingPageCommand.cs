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
using Offers.CleanArchitecture.Domain.Events.OnboardingPageEvents;
using Offers.CleanArchitecture.Domain.Events.OnboardingPageLocalizationEvents;
using Offers.CleanArchitecture.Domain.Events.PostLocalizationEvents;


namespace Offers.CleanArchitecture.Application.OnboardingPages.Commands.CreateOnboardingPage;
public class CreateOnboardingPageCommand : IRequest<Guid>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Order { get; set; } = 0!;
    public FileDto Asset { get; set; } = null!;
    public List<OnboardingPageLocalizationApp> OnboardingPageLocalizations { get; set; } = new List<OnboardingPageLocalizationApp>();
    public List<OnboardingPageLocalizationAssetApp> OnboardingPageLocalizationAssets { get; set; } = new List<OnboardingPageLocalizationAssetApp>();

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateOnboardingPageCommand, OnboardingPage>();
        }
    }

}

public class CreateOnboardingPageCommandHandler : IRequestHandler<CreateOnboardingPageCommand, Guid>
{
    private readonly ILogger<CreateOnboardingPageCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILanguageRepository _languageRepository;
    private readonly IOnboardingPageLocalizationRepository _onboardingPageLocalizationRepository;
    private readonly IOnboardingPageRepository _onboardingPageRepository;

    public CreateOnboardingPageCommandHandler(ILogger<CreateOnboardingPageCommandHandler> logger,
                                              IMapper mapper,
                                              IFileService fileService,
                                              IUnitOfWorkAsync unitOfWork,
                                              ILanguageRepository languageRepository,
                                              IOnboardingPageLocalizationRepository onboardingPageLocalizationRepository,
                                              IOnboardingPageRepository onboardingPageRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
        _languageRepository = languageRepository;
        _onboardingPageLocalizationRepository = onboardingPageLocalizationRepository;
        _onboardingPageRepository = onboardingPageRepository;
    }

    public async Task<Guid> Handle(CreateOnboardingPageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            //add standard Onboarding Page
            var onboardingPageToAdd = _mapper.Map<OnboardingPage>(request);
            // upload asset and save the path
            var assetPath = await _fileService.UploadFileAsync(request.Asset);
            onboardingPageToAdd.AssetPath = assetPath;

            await _onboardingPageRepository.AddAsync(onboardingPageToAdd);
            onboardingPageToAdd.AddDomainEvent(new OnboardingPageCreatedEvent(onboardingPageToAdd));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //add Localization for Title,Description
            foreach (var onboardingLocalization in request.OnboardingPageLocalizations)
            {
                var language = await _languageRepository.GetByIdAsync(onboardingLocalization.LanguageId);
                OnboardingPageLocalization onboardingPageLocalizationToAdd = new OnboardingPageLocalization();
                //add foreign keys
                onboardingPageLocalizationToAdd.LanguageId = language.Id;
                onboardingPageLocalizationToAdd.OnboardingPageId = onboardingPageToAdd.Id;
                //add localization
                onboardingPageLocalizationToAdd.OnboardingPageLocalizationFieldType = (int)onboardingLocalization.FieldType;
                onboardingPageLocalizationToAdd.Value = onboardingLocalization.Value;
                //save and seed event
                await _onboardingPageLocalizationRepository.AddAsync(onboardingPageLocalizationToAdd);
                onboardingPageLocalizationToAdd.AddDomainEvent(new OnboardingPageLocalizationCreatedEvent(onboardingPageLocalizationToAdd));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            //add Localization for asset
            foreach (var onboardingPageLocalizationAsset in request.OnboardingPageLocalizationAssets)
            {
                var language = await _languageRepository.GetByIdAsync(onboardingPageLocalizationAsset.LanguageId);
                OnboardingPageLocalization onboardingPageLocalizationToAdd = new OnboardingPageLocalization();
                //add foreign keys
                onboardingPageLocalizationToAdd.LanguageId = language.Id;
                onboardingPageLocalizationToAdd.OnboardingPageId = onboardingPageToAdd.Id;
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
            return onboardingPageToAdd.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
