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
using Offers.CleanArchitecture.Application.Countries.Commands.CreateCountry;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Countries.Commands.UpdateCountry;
public class UpdateCountryCommand : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public FileDto? Flag { get; set; } = null!;
    public string TimeZoneId { get; set; } = null!;
    public string Code { get; set; } = null!;
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateCountryCommand, Country>();
        }
    }
}

public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCountryCommandHandler> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly IFileService _fileService;

    public UpdateCountryCommandHandler(ICountryRepository countryRepository,
                                       IMapper mapper,
                                       ILogger<UpdateCountryCommandHandler> logger,
                                       IUnitOfWorkAsync unitOfWork,
                                       IFileService fileService)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }
    public async Task Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var existingCountry = await _countryRepository.GetByIdAsync(request.Id);

            _mapper.Map(request, existingCountry);
            var oldFlagPath = existingCountry.FlagPath;
            if (request.Flag != null)
            {
                existingCountry.FlagPath = await _fileService.UploadFileAsync(request.Flag);
            }
            await _countryRepository.UpdateAsync(existingCountry);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
            if (request.Flag != null && oldFlagPath != null)
                await _fileService.DeleteFileAsync(oldFlagPath);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
