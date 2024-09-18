using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Languages.Commands.DeleteLanguage;
public class DeleteLanguageCommand : IRequest
{
    public Guid LanguageId { get; set; }
}

public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand>
{
    private readonly ILogger<DeleteLanguageCommandHandler> _logger;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public DeleteLanguageCommandHandler(ILogger<DeleteLanguageCommandHandler> logger,
                                        ILanguageRepository languageRepository,
                                        IUnitOfWorkAsync unitOfWork)
    {
        _logger = logger;
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var language = await _languageRepository.GetByIdAsync(request.LanguageId);
            await _languageRepository.DeleteAsync(language);
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
