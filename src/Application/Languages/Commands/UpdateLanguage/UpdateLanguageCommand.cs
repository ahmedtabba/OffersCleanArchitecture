using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Countries.Commands.UpdateCountry;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Languages.Commands.UpdateLanguage;
public class UpdateLanguageCommand :IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool RTL { get; set; }
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateLanguageCommand, Language>();
        }
    }
}

public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand>
{
    private readonly ILogger<UpdateLanguageCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public UpdateLanguageCommandHandler(ILogger<UpdateLanguageCommandHandler> logger,
                                        IMapper mapper,
                                        ILanguageRepository languageRepository,
                                        IUnitOfWorkAsync unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var existingLanguage = await _languageRepository.GetByIdAsync(request.Id);

            _mapper.Map(request, existingLanguage);

            await _languageRepository.UpdateAsync(existingLanguage);
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
