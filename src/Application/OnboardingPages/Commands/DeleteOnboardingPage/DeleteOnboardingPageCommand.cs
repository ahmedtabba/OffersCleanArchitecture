using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Commands.DeleteOnboardingPage;
public class DeleteOnboardingPageCommand : IRequest
{
    public Guid OnboardingPageId { get; set; }
}

public class DeleteOnboardingPageCommandHandler : IRequestHandler<DeleteOnboardingPageCommand>
{
    private readonly ILogger<DeleteOnboardingPageCommandHandler> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly IOnboardingPageRepository _onboardingPageRepository;
    private readonly IOnboardingPageLocalizationRepository _onboardingPageLocalizationRepository;
    private readonly IFileService _fileService;

    public DeleteOnboardingPageCommandHandler(ILogger<DeleteOnboardingPageCommandHandler> logger,
                                              IUnitOfWorkAsync unitOfWork,
                                              IOnboardingPageRepository onboardingPageRepository,
                                              IOnboardingPageLocalizationRepository onboardingPageLocalizationRepository,
                                              IFileService fileService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _onboardingPageRepository = onboardingPageRepository;
        _onboardingPageLocalizationRepository = onboardingPageLocalizationRepository;
        _fileService = fileService;
    }

    public async Task Handle(DeleteOnboardingPageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var onboardingPageToDelete = await _onboardingPageRepository.GetByIdAsync(request.OnboardingPageId);
            // save asset path to delete later
            var assetPathToDelete = onboardingPageToDelete.AssetPath;
            //get asset localization of onboarding page to get assets paths

            var onboardingPageLocalization = await _onboardingPageLocalizationRepository.GetAll()
                .Where(ol => ol.OnboardingPageId == request.OnboardingPageId)
                .ToListAsync();

            await _onboardingPageRepository.DeleteAsync(onboardingPageToDelete);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();

            // delete standard asset
            await _fileService.DeleteFileAsync(assetPathToDelete);

            // delete localization assets

            if (onboardingPageLocalization!.Count > 0)
            {
                var listOfLocalizedAssetsToDelete = onboardingPageLocalization
                    .Where(ol => ol.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.AssetPath)
                    .Select(ol => ol.Value)
                    .ToList();
                if (listOfLocalizedAssetsToDelete.Count > 0)
                {
                    foreach (var assetPath in listOfLocalizedAssetsToDelete)
                    {
                        await _fileService.DeleteFileAsync(assetPath);
                    }
                }
            }
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
