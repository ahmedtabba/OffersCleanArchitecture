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
using Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;
using Offers.CleanArchitecture.Application.Identity.Queries.GetUsersWithPagination;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;
using Offers.CleanArchitecture.Domain.Events.GroceryEvents;
using Offers.CleanArchitecture.Domain.Events.PostEvents;
using Offers.CleanArchitecture.Domain.Events.PostLocalizationEvents;

namespace Offers.CleanArchitecture.Application.Posts.Commands.CreatePost;
public class CreatePostCommand : IRequest<Guid>
{
    public Guid GroceryId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool? IsActive { get; set; }
    public FileDto File { get; set; } = null!;
    public List<PostLocalizationApp> PostLocalizations { get; set; } = new List<PostLocalizationApp>();
    // Now we have Image for Post and images for PostLocalization, 
    // in future if we need to add video for Post and videos for PostLocalization we change PostLocalizationImages => PostLocalizationAssets
    // and handle the type of PostLocalizationAssetsApp.File property 
    public List<PostLocalizationAssetsApp> PostLocalizationImages  { get; set; } = new List<PostLocalizationAssetsApp>();
    public DateTime? PublishDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreatePostCommand, Post>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == null ? true : src.IsActive));
        }      
    }
}

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IFileService _fileService;
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<CreatePostCommandHandler> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILanguageRepository _languageRepository;
    private readonly IPostLocalizationRepository _postLocalizationRepository;
    private readonly IUser _user;
    private readonly IUserContext _userContext;
    private readonly ICountryRepository _countryRepository;

    public CreatePostCommandHandler(IMapper mapper,
                                    IPostRepository postRepository,
                                    IFileService fileService,
                                    IGroceryRepository groceryRepository,
                                    ILogger<CreatePostCommandHandler> logger,
                                    IUnitOfWorkAsync unitOfWork,
                                    ILanguageRepository languageRepository,
                                    IPostLocalizationRepository postLocalizationRepository,
                                    IUser user,
                                    IUserContext userContext,
                                    ICountryRepository countryRepository)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _fileService = fileService;
        _groceryRepository = groceryRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _languageRepository = languageRepository;
        _postLocalizationRepository = postLocalizationRepository;
        _user = user;
        _userContext = userContext;
        _countryRepository = countryRepository;
    }
    public async Task<Guid> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            //add standard Post
            var postToAdd = _mapper.Map<Post>(request);

            // convert datetime to UTC before saving in DB
            await PostHelper.ConvertDateTimeToUTC(postToAdd, _userContext, _countryRepository);

            var imagePath = await _fileService.UploadFileAsync(request.File);
            postToAdd.ImagePath = imagePath;
            var grocery = await _groceryRepository.GetByIdAsync(postToAdd.GroceryId);
            //grocery.Posts.Add(postToAdd);
            postToAdd.Grocery = grocery;
            await _postRepository.AddAsync(postToAdd);
            postToAdd.AddDomainEvent(new PostCreatedEvent(postToAdd));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //add Localization for Title,Description
            foreach (var postLocalization in request.PostLocalizations)
            {
                var language = await _languageRepository.GetByIdAsync(postLocalization.LanguageId);
                PostLocalization postLocalizationToAdd = new PostLocalization();
                postLocalizationToAdd.LanguageId = language.Id;
                postLocalizationToAdd.PostId = postToAdd.Id;

                postLocalizationToAdd.PostLocalizationFieldType = (int)postLocalization.FieldType;
                postLocalizationToAdd.Value = postLocalization.Value;

                await _postLocalizationRepository.AddAsync(postLocalizationToAdd);
                postLocalizationToAdd.AddDomainEvent(new PostLocalizationCreatedEvent(postLocalizationToAdd));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            //add Localization for images
            foreach (var postLocalizationImagesApp in request.PostLocalizationImages)
            {
                var language = await _languageRepository.GetByIdAsync(postLocalizationImagesApp.LanguageId);
                PostLocalization postLocalizationToAdd = new PostLocalization();
                postLocalizationToAdd.LanguageId = language.Id;
                postLocalizationToAdd.PostId = postToAdd.Id;
                postLocalizationToAdd.PostLocalizationFieldType = (int)PostLocalizationFieldType.AssetPath;
                var imagePathLocalization = await _fileService.UploadFileAsync(postLocalizationImagesApp.File);
                postLocalizationToAdd.Value = imagePathLocalization;

                await _postLocalizationRepository.AddAsync(postLocalizationToAdd);
                postLocalizationToAdd.AddDomainEvent(new PostLocalizationCreatedEvent(postLocalizationToAdd));
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            }
            await _unitOfWork.CommitAsync();
            return postToAdd.Id;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
