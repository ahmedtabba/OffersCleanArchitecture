using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Localization;
using Offers.CleanArchitecture.Application.Groceries.Commands.UpdateGrocery;
using Offers.CleanArchitecture.Application.Posts.Commands.DeletePost;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;
using Offers.CleanArchitecture.Domain.Events.PostLocalizationEvents;

namespace Offers.CleanArchitecture.Application.Posts.Commands.UpdatePost;
public class UpdatePostCommand : IRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool? IsActive { get; set; }
    public FileDto? File { get; set; } = null!;
    public DateTime? PublishDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<PostLocalizationApp> PostLocalizations { get; set; } = new List<PostLocalizationApp>();
    // Now we have Image for Post and images for PostLocalization, 
    // in future if we need to add video for Post and videos for PostLocalization we change PostLocalizationImages => PostLocalizationAssets
    // and handle the type of PostLocalizationAssetsApp.File property 
    public List<PostLocalizationAssetsApp> PostLocalizationImages { get; set; } = new List<PostLocalizationAssetsApp>();
    public List<Guid> DeletedLocalizedAssetsIds { get; set; } = new List<Guid>();

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdatePostCommand, Post>()
                .ForMember(dest => dest.ImagePath, g => g.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == null ? true : src.IsActive));
        }
    }

    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand>
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IFileService _fileService;
        private readonly ILogger<UpdatePostCommandHandler> _logger;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPostLocalizationRepository _postLocalizationRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IUserContext _userContext;
        private readonly ICountryRepository _countryRepository;

        public UpdatePostCommandHandler(IMapper mapper,
                                        IPostRepository postRepository,
                                        IFileService fileService,
                                        ILogger<UpdatePostCommandHandler> logger,
                                        IUnitOfWorkAsync unitOfWork,
                                        IPostLocalizationRepository postLocalizationRepository,
                                        ILanguageRepository languageRepository,
                                        IUserContext userContext,
                                        ICountryRepository countryRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _fileService = fileService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _postLocalizationRepository = postLocalizationRepository;
            _languageRepository = languageRepository;
            _userContext = userContext;
            _countryRepository = countryRepository;
        }
        public async Task Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var existingPost = await _postRepository.GetByIdAsync(request.Id);
                var oldImagePath = existingPost.ImagePath;

                _mapper.Map(request, existingPost);

                // convert datetime to UTC before saving in DB

                await PostHelper.ConvertDateTimeToUTC(existingPost, _userContext, _countryRepository);

                if (request.File is not null)
                {
                    await _fileService.DeleteFileAsync(oldImagePath);
                    var newImagePath = await _fileService.UploadFileAsync(request.File);
                    existingPost.ImagePath = newImagePath;
                }
                else // Post.AssetPath is ignored by mapping process (request => existingPost)
                    existingPost.ImagePath = oldImagePath;

                await _postRepository.UpdateAsync(existingPost);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // delete Localization for Title,Description
                var postLocalizationListToDelete = await _postLocalizationRepository.GetAll()
                    .Where(pl => pl.PostId == request.Id &&
                    (pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title 
                    || pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description))
                    .ToListAsync();

                foreach (var postLocalizationToDelete in postLocalizationListToDelete)
                {
                    await _postLocalizationRepository.DeleteAsync(postLocalizationToDelete);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //add new Localization for Title,Discription
                foreach (var postLocalization in request.PostLocalizations)
                {
                    var language = await _languageRepository.GetByIdAsync(postLocalization.LanguageId);
                    PostLocalization postLocalizationToAdd = new PostLocalization();
                    postLocalizationToAdd.LanguageId = language.Id;
                    postLocalizationToAdd.PostId = existingPost.Id;

                    postLocalizationToAdd.PostLocalizationFieldType = (int)postLocalization.FieldType;
                    postLocalizationToAdd.Value = postLocalization.Value;

                    await _postLocalizationRepository.AddAsync(postLocalizationToAdd);
                    postLocalizationToAdd.AddDomainEvent(new PostLocalizationCreatedEvent(postLocalizationToAdd));
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                // delete old localization images
                var ListOfPathsOfImagesToDelete = new List<string>();
                foreach (var id in request.DeletedLocalizedAssetsIds)
                {
                    var postLocalizationImageToDelete = await _postLocalizationRepository.GetByIdAsync(id);
                    string amagePathToDelete = postLocalizationImageToDelete.Value;
                    await _postLocalizationRepository.DeleteAsync(postLocalizationImageToDelete);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    ListOfPathsOfImagesToDelete.Add(amagePathToDelete);
                }

                // add new localization images
                foreach (var postLocalizationImagesApp in request.PostLocalizationImages)
                {
                    var language = await _languageRepository.GetByIdAsync(postLocalizationImagesApp.LanguageId);
                    PostLocalization postLocalizationToAdd = new PostLocalization();
                    postLocalizationToAdd.LanguageId = language.Id;
                    postLocalizationToAdd.PostId = existingPost.Id;
                    postLocalizationToAdd.PostLocalizationFieldType = (int)PostLocalizationFieldType.AssetPath;
                    var imagePathLocalization = await _fileService.UploadFileAsync(postLocalizationImagesApp.File);
                    postLocalizationToAdd.Value = imagePathLocalization;

                    await _postLocalizationRepository.AddAsync(postLocalizationToAdd);
                    postLocalizationToAdd.AddDomainEvent(new PostLocalizationCreatedEvent(postLocalizationToAdd));
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                await _unitOfWork.CommitAsync();

                //delete images from upload folder
                foreach (var pathOfImageToDelete in ListOfPathsOfImagesToDelete)
                {
                    await _fileService.DeleteFileAsync(pathOfImageToDelete);
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
