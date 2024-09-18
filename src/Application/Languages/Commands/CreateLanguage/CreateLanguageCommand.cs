using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Events.LanguageEvents;

namespace Offers.CleanArchitecture.Application.Languages.Commands.CreateLanguage;
public class CreateLanguageCommand : IRequest<Guid>
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool RTL { get; set; } = false;
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateLanguageCommand, Language>();
        }
    }
}

public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, Guid>
{
    private readonly ILogger<CreateLanguageCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public CreateLanguageCommandHandler(ILogger<CreateLanguageCommandHandler> logger,
                                        IMapper mapper,
                                        ILanguageRepository languageRepository,
                                        IUnitOfWorkAsync unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Guid> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var language = _mapper.Map<Language>(request);
            language.Code = language.Code.ToLower();
            await _languageRepository.AddAsync(language);
            language.AddDomainEvent(new LanguageCreatedEvent(language));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
            return language.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
