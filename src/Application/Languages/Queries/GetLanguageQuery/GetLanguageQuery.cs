using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithPagination;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageQuery;
public class GetLanguageQuery : IRequest<GetLanguageDto>
{
    public Guid LanguageId { get; set; }
}

public class GetLanguageQueryHandler : IRequestHandler<GetLanguageQuery, GetLanguageDto>
{
    private readonly ILogger<GetLanguageQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly ILanguageRepository _languageRepository;

    public GetLanguageQueryHandler(ILogger<GetLanguageQueryHandler> logger,
                                   IMapper mapper,
                                   ILanguageRepository languageRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _languageRepository = languageRepository;
    }

    public async Task<GetLanguageDto> Handle(GetLanguageQuery request, CancellationToken cancellationToken)
    {
        var language = await _languageRepository.GetByIdAsync(request.LanguageId);
        var languageDto = _mapper.Map<GetLanguageDto>(language);
        return languageDto;
    }
}
