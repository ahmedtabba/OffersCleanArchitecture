using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Events.GroceryEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Models.Localization;
using Offers.CleanArchitecture.Domain.Enums;
using Offers.CleanArchitecture.Domain.Events.GroceryLocalizationEvents;
using Offers.CleanArchitecture.Application.Utilities;
using System.Threading;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;
public class CreateGroceryCommand : IRequest<Guid>
{
    public Guid CountryId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public FileDto File { get; set; } = new ();
    public List<GroceryLocalizationApp> GroceryLocalizations { get; set; } = new List<GroceryLocalizationApp> ();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateGroceryCommand, Grocery>()
                .ForMember(dest => dest.LogoPath, s => s.Ignore())
                .ForMember(dest => dest.CountryId, s => s.Ignore());
        }
    }
}

public class CreateGroceryCommandHandler : IRequestHandler<CreateGroceryCommand, Guid>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IUser _user;
    private readonly ILogger<CreateGroceryCommandHandler> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IGroceryLocalizationRepository _groceryLocalizationRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public CreateGroceryCommandHandler(IGroceryRepository groceryRepository,
                                       IMapper mapper,
                                       IFileService fileService,
                                       IUser user,
                                       ILogger<CreateGroceryCommandHandler> logger,
                                       ICountryRepository countryRepository,
                                       ILanguageRepository languageRepository,
                                       IGroceryLocalizationRepository groceryLocalizationRepository,
                                       IUnitOfWorkAsync unitOfWork)
    {
        _groceryRepository = groceryRepository;
        _mapper = mapper;
        _fileService = fileService;
        _user = user;
        _logger = logger;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;
        _groceryLocalizationRepository = groceryLocalizationRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Guid> Handle(CreateGroceryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            //add standard Grocery
            // map CreateGroceryCommand => Grocery
            var grocery = _mapper.Map<Grocery>(request);

            var logoPath = await _fileService.UploadFileAsync(request.File);

            grocery.LogoPath = logoPath;
            // Seed GroceryCreatedEvent of the added grocery
            var country = await _countryRepository.GetByIdAsync(request.CountryId);
            //country.Groceries.Add(grocery);
            grocery.Country = country;
            await _groceryRepository.AddAsync(grocery);
            grocery.AddDomainEvent(new GroceryCreatedEvent(grocery));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //add Localization for Name,Discription,Addrress
            foreach (var groceryLocalization in request.GroceryLocalizations)
            {
                var language = await _languageRepository.GetByIdAsync(groceryLocalization.LanguageId);
                GroceryLocalization groceryLocalizationToAdd = new GroceryLocalization();

                groceryLocalizationToAdd.LanguageId = language.Id;
                groceryLocalizationToAdd.GroceryId = grocery.Id;

                groceryLocalizationToAdd.GroceryLocalizationFieldType = (int)groceryLocalization.FieldType;
                groceryLocalizationToAdd.Value = groceryLocalization.Value;

                await _groceryLocalizationRepository.AddAsync(groceryLocalizationToAdd);
                groceryLocalizationToAdd.AddDomainEvent(new GroceryLocalizationCreatedEvent(groceryLocalizationToAdd));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _unitOfWork.CommitAsync();
            return grocery.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

    }
}
