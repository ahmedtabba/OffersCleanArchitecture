using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Localization;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Events.GroceryLocalizationEvents;


namespace Offers.CleanArchitecture.Application.Groceries.Commands.UpdateGrocery;
public class UpdateGroceryCommand : IRequest
{
    public Guid CountryId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public FileDto File { get; set; } = new();
    public List<GroceryLocalizationApp> GroceryLocalizations { get; set; } = new List<GroceryLocalizationApp>();


    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateGroceryCommand, Grocery>()
                .ForMember(dest => dest.LogoPath, g => g.Ignore())
                .ForMember(dest => dest.CountryId, g => g.Ignore());
        }
    }
}

public class UpdateGroceryCommandHandler : IRequestHandler<UpdateGroceryCommand>
{
    private readonly IMapper _mapper;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IFileService _fileService;
    private readonly IUser _user;
    private readonly ILogger<UpdateGroceryCommandHandler> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IGroceryLocalizationRepository _groceryLocalizationRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public UpdateGroceryCommandHandler(IMapper mapper,
                                       IGroceryRepository groceryRepository,
                                       IFileService fileService,
                                       IUser user,
                                       ILogger<UpdateGroceryCommandHandler> logger,
                                       ICountryRepository countryRepository,
                                       ILanguageRepository languageRepository,
                                       IGroceryLocalizationRepository groceryLocalizationRepository,
                                       IUnitOfWorkAsync unitOfWork)
    {
        _mapper = mapper;
        _groceryRepository = groceryRepository;
        _fileService = fileService;
        _user = user;
        _logger = logger;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;
        _groceryLocalizationRepository = groceryLocalizationRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(UpdateGroceryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var existingGrocery = await _groceryRepository.GetByIdAsync(request.Id);
            // update Grocery Info
            var oldLogoPath = existingGrocery.LogoPath;

            _mapper.Map(request, existingGrocery);

            if (request.File is not null)
            {
                await _fileService.DeleteFileAsync(oldLogoPath);
                var newLogoPath = await _fileService.UploadFileAsync(request.File);
                existingGrocery.LogoPath = newLogoPath;
            }
            else // grocery.LogoPath is ignored by mapping process (request => existingGrocery)
                existingGrocery.LogoPath = oldLogoPath;

            await _groceryRepository.UpdateAsync(existingGrocery);

            //Update Grocery <=> Country relation
            if (request.CountryId != existingGrocery.CountryId)
            {
                var oldCountry = await _countryRepository.GetByIdAsync(existingGrocery.CountryId);
                oldCountry.Groceries.Remove(existingGrocery);
                //await _countryRepository.SaveChangesAsync(cancellationToken);
                var newCountry = await _countryRepository.GetByIdAsync(request.CountryId);
                newCountry.Groceries.Add(existingGrocery);
                //await _countryRepository.SaveChangesAsync(cancellationToken);
            }

            //Update groceryLocalization
            // First : delete old localization
            var groceryLocalizations = await _groceryLocalizationRepository.GetAll()
                .Where(gl => gl.GroceryId == existingGrocery.Id).ToListAsync();

            foreach (var groceryLocalization in groceryLocalizations)
            {
                await _groceryLocalizationRepository.DeleteAsync(groceryLocalization);
            }
            // Second : add new localization
            foreach (var groceryLocalization in request.GroceryLocalizations)
            {
                var language = await _languageRepository.GetByIdAsync(groceryLocalization.LanguageId);
                GroceryLocalization groceryLocalizationToAdd = new GroceryLocalization();

                groceryLocalizationToAdd.LanguageId = language.Id;
                groceryLocalizationToAdd.GroceryId = existingGrocery.Id;

                groceryLocalizationToAdd.GroceryLocalizationFieldType = (int)groceryLocalization.FieldType;
                groceryLocalizationToAdd.Value = groceryLocalization.Value;

                await _groceryLocalizationRepository.AddAsync(groceryLocalizationToAdd);
                groceryLocalizationToAdd.AddDomainEvent(new GroceryLocalizationCreatedEvent(groceryLocalizationToAdd));
                //await _groceryLocalizationRepository.SaveChangesAsync(cancellationToken);
                //await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
