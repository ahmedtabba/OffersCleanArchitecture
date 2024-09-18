using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Posts.Commands.CreatePost;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Posts.Commands.DeletePost;
public class DeletePostCommand : IRequest
{
    public Guid postId { get; set; }
}

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<DeletePostCommandHandler> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly IPostLocalizationRepository _postLocalizationRepository;

    public DeletePostCommandHandler(IPostRepository postRepository,
                                    IFileService fileService,
                                    ILogger<DeletePostCommandHandler> logger,
                                    IUnitOfWorkAsync unitOfWork,
                                    IPostLocalizationRepository postLocalizationRepository)
    {
        _postRepository = postRepository;
        _fileService = fileService;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _postLocalizationRepository = postLocalizationRepository;
    }

    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var post = await _postRepository.GetByIdAsync(request.postId);
            var imagePathToDelete = post.ImagePath;

            // get localization of the post that will be deleted to get Localized Images and delete them
            var postLocalization = await _postLocalizationRepository.GetAll()
                .Where(pl => pl.PostId == request.postId)
                .ToListAsync();
            
            await _postRepository.DeleteAsync(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitAsync();

            //delete images of post
            await _fileService.DeleteFileAsync(imagePathToDelete);

            //delete Images for LocalizedImages
            if (postLocalization!.Count() > 0)
            {
                var listOfLocalizedImagesToDelete = postLocalization
                    .Where(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath)
                    .Select(pl => pl.Value)
                    .ToList();
                if (listOfLocalizedImagesToDelete.Count() > 0)
                {
                    foreach (var imagePath in listOfLocalizedImagesToDelete)
                    {
                        await _fileService.DeleteFileAsync(imagePath);
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
