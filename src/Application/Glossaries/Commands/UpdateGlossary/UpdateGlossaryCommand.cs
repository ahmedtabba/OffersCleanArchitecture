using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models.Localization;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Events.GlossaryLocalizationEvents;
using Offers.CleanArchitecture.Domain.Events.GroceryLocalizationEvents;

namespace Offers.CleanArchitecture.Application.Glossaries.Commands.UpdateGlossary;
public class UpdateGlossaryCommand : IRequest
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public List<GlossaryLocalizationApp> GlossaryLocalizations { get; set; } = new List<GlossaryLocalizationApp>();

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateGlossaryCommand, Glossary>();
        }
    }
}

public class UpdateGlossaryCommandHandler : IRequestHandler<UpdateGlossaryCommand>
{
    private readonly ILogger<UpdateGlossaryCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IGlossaryRepository _glossaryRepository;
    private readonly IGlossaryLocalizationRepository _glossaryLocalizationRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILanguageRepository _languageRepository;

    public UpdateGlossaryCommandHandler(ILogger<UpdateGlossaryCommandHandler> logger,
                                        IMapper mapper,
                                        IGlossaryRepository glossaryRepository,
                                        IGlossaryLocalizationRepository glossaryLocalizationRepository,
                                        IUnitOfWorkAsync unitOfWork,
                                        ILanguageRepository languageRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _glossaryRepository = glossaryRepository;
        _glossaryLocalizationRepository = glossaryLocalizationRepository;
        _unitOfWork = unitOfWork;
        _languageRepository = languageRepository;
    }

    public async Task Handle(UpdateGlossaryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var existingGlossary = await _glossaryRepository.GetByIdAsync(request.Id);

            // update Glossary Info
            _mapper.Map(request, existingGlossary);

            await _glossaryRepository.UpdateAsync(existingGlossary);

            //Update glossaryLocalization

            // First : delete old localization
            var glossaryLocalizations = await _glossaryLocalizationRepository.GetAll()
                .Where(gl => gl.GlossaryId == existingGlossary.Id).ToListAsync();
            foreach (var glossaryLocalization in glossaryLocalizations)
            {
                await _glossaryLocalizationRepository.DeleteAsync(glossaryLocalization);
            }

            // Second : add new localization
            foreach (var glossaryLocalization in request.GlossaryLocalizations)
            {
                var language = await _languageRepository.GetByIdAsync(glossaryLocalization.LanguageId);
                GlossaryLocalization glossaryLocalizationToAdd = new GlossaryLocalization();

                glossaryLocalizationToAdd.LanguageId = language.Id;
                glossaryLocalizationToAdd.GlossaryId = existingGlossary.Id;

                glossaryLocalizationToAdd.Value = glossaryLocalization.Value;

                await _glossaryLocalizationRepository.AddAsync(glossaryLocalizationToAdd);
                //TODO : seed event to create notifications later
                
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
