using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Countries.Commands.DeleteCountry;
public class DeleteCountryCommand : IRequest
{
    public Guid CountryId { get; set; }
}

public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<DeleteCountryCommandHandler> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly IFileService _fileService;

    public DeleteCountryCommandHandler(ICountryRepository countryRepository,
                                       ILogger<DeleteCountryCommandHandler> logger,
                                       IUnitOfWorkAsync unitOfWork,
                                       IFileService fileService)
    {
        _countryRepository = countryRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }
    public async Task Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var country = await _countryRepository.GetByIdAsync(request.CountryId);
            var flagPath = country.FlagPath;
            await _countryRepository.DeleteAsync(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
            if (flagPath != null)
            {
                await _fileService.DeleteFileAsync(flagPath);
            }
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
