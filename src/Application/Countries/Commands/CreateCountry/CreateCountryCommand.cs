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
using Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Events.CountryEvents;

namespace Offers.CleanArchitecture.Application.Countries.Commands.CreateCountry;
public class CreateCountryCommand : IRequest<Guid>
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public FileDto Flag { get; set; } = null!;
    public string TimeZoneId { get; set; } = null!;
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateCountryCommand, Country>();
        }
    }
}

public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, Guid>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCountryCommandHandler> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly IFileService _fileService;

    public CreateCountryCommandHandler(ICountryRepository countryRepository,
                                       IMapper mapper,
                                       ILogger<CreateCountryCommandHandler> logger,
                                       IUnitOfWorkAsync unitOfWork,
                                       IFileService fileService)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }
    public async Task<Guid> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var country = _mapper.Map<Country>(request);
            var flagPath = request.Flag != null ? await _fileService.UploadFileAsync(request.Flag) : null;
            country.FlagPath = flagPath;
            await _countryRepository.AddAsync(country);
            country.AddDomainEvent(new CountryCreatedEvent(country));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
            return country.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}

