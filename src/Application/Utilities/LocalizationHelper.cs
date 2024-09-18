using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossariesForAdminWithPagination;
using Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossary;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesForAdminWithPagination;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryForAdminQuery;
using Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPageForAdmin;
using Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPagesWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostForAdminQuery;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostQuery;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByFavoriteGroceriesWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsForAdminQueryWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsWithPagination;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Utilities;
public static class LocalizationHelper
{
    #region Post Localization
    public static async Task FillPostLocalizationsAndIsLiven(PaginatedList<GetPostsByGroceryWithPaginationDto> result, Guid userLanguageId, IPostLocalizationRepository postLocalizationRepository, IPostRepository postRepository)
    {
        foreach (var postDto in result.Items)
        {
            var postLocalizations = await postLocalizationRepository.GetAll()
            .Where(pl => pl.PostId == postDto.Id && pl.LanguageId == userLanguageId)
            .ToListAsync();

            if (postLocalizations.Count > 0)
            {
                postDto.Title = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title)!.Value
                    : postDto.Title;

                postDto.Description = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description)!.Value
                    : postDto.Description;

                postDto.ImagePath = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath)!.Value
                    : postDto.ImagePath;
            }
            postDto.IsLiven = await PostHelper.IsPostActive(postDto.Id, postRepository);
        }
    }

    public static async Task FillPostLocalizationsAndIsLiven(PaginatedList<GetPostsWithPaginationDto> result, Guid userLanguageId, IPostLocalizationRepository postLocalizationRepository,IPostRepository postRepository)
    {
        foreach (var postDto in result.Items)
        {
            // get all localization of post
            var postLocalizations = await postLocalizationRepository.GetAll()
            .Where(pl => pl.PostId == postDto.Id && pl.LanguageId == userLanguageId)
            .ToListAsync();
            // replace Title,Description,AssetPath with localization if existed
            if (postLocalizations.Count > 0)
            {
                postDto.Title = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title)!.Value
                    : postDto.Title;

                postDto.Description = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description)!.Value
                    : postDto.Description;

                postDto.ImagePath = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath)!.Value
                    : postDto.ImagePath;
            }
            // check if post IsLiven
            postDto.IsLiven = await PostHelper.IsPostActive(postDto.Id, postRepository);
        }
    }

    public static async Task FillPostLocalizations(GetPostDto postDto, Guid userLanguageId, IPostLocalizationRepository postLocalizationRepository)
    {
         var postLocalizations = await postLocalizationRepository.GetAll()
         .Where(pl => pl.PostId == postDto.Id && pl.LanguageId == userLanguageId)
         .ToListAsync();

         if (postLocalizations.Count > 0)
         {
             postDto.Title = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title) != null
                 ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title)!.Value
                 : postDto.Title;

             postDto.Description = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description) != null
                 ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description)!.Value
                 : postDto.Description;

             postDto.ImagePath = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath) != null
                 ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath)!.Value
                 : postDto.ImagePath;
         }
    }

    public static async Task FillPostLocalizations(GetPostForAdminDto dto, IPostLocalizationRepository postLocalizationRepository,IMapper mapper)
    {
        var postLocalizations = await postLocalizationRepository.GetAll()
        .Where(pl => pl.PostId == dto.Id)
        .ToListAsync();

        if (postLocalizations.Count > 0)
        {
            foreach (var postLocalization in postLocalizations)
            {
                var postLocalizationDto = mapper.Map<PostLocalizationForAdminDto>(postLocalization);
                dto.PostLocalizationDtos.Add(postLocalizationDto);
            }
        }
    }

    public static async Task FillPostLocalizationsAndIsLiven(PaginatedList<GetPostsByFavoriteGroceriesWithPaginationDto> result, Guid userLanguageId, IPostLocalizationRepository postLocalizationRepository,IPostRepository postRepository)
    {
        foreach (var postDto in result.Items)
        {
            var postLocalizations = await postLocalizationRepository.GetAll()
            .Where(pl => pl.PostId == postDto.Id && pl.LanguageId == userLanguageId)
            .ToListAsync();

            if (postLocalizations.Count > 0)
            {
                postDto.Title = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Title)!.Value
                    : postDto.Title;

                postDto.Description = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.Description)!.Value
                    : postDto.Description;

                postDto.ImagePath = postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath) != null
                    ? postLocalizations.FirstOrDefault(pl => pl.PostLocalizationFieldType == (int)PostLocalizationFieldType.AssetPath)!.Value
                    : postDto.ImagePath;
            }
            postDto.IsLiven = await PostHelper.IsPostActive(postDto.Id,postRepository);
        }
    }
    public static async Task FillPostLocalizationsAndIsLiven(PaginatedList<GetPostsForAdminWithPaginationDto> result, IPostLocalizationRepository postLocalizationRepository,IMapper mapper, IPostRepository postRepository)
    {
        foreach (var postWithFullLocalizationDto in result.Items)
        {
            // get all localization of current post
            var postLocalizations = await postLocalizationRepository.GetAll()
            .Where(pl => pl.PostId == postWithFullLocalizationDto.Id)
            .ToListAsync();
            // fill localization if existed
            if (postLocalizations.Count > 0)
            {
                foreach (var postLocalization in postLocalizations)
                {
                    var postLocalizationDto = mapper.Map<PostsLocalizationForAdminDto>(postLocalization);
                    postWithFullLocalizationDto.PostLocalizationDtos.Add(postLocalizationDto);
                }
            }
            // check if post IsLiven
            postWithFullLocalizationDto.IsLiven = await PostHelper.IsPostActive(postWithFullLocalizationDto.Id, postRepository);
        }
    }

    #endregion

    #region Grocery Localization
    public static async Task FillGroceryLocalizations(GetGroceryForAdminDto dto, IGroceryLocalizationRepository groceryLocalizationRepository,IMapper mapper)
    {
        // the data returned is from Db or from memory cache
        var groceryLocalizations = await groceryLocalizationRepository.GetAllByGroceryId(dto.Id);

        if (groceryLocalizations.Count > 0)
        {
            foreach (var groceryLocalization in groceryLocalizations)
            {
                var groceryLocalizationDto = mapper.Map<GroceryLocalizationForAdminDto>(groceryLocalization);
                dto.GroceryLocalizationDtos.Add(groceryLocalizationDto);
            }
        }
    }

    public static async Task FillGroceryLocalizations(PaginatedList<GetGroceriesForAdminWithPaginationDto> result, IGroceryLocalizationRepository groceryLocalizationRepository, IMapper mapper)
    {
        foreach (var groceryWithFullLocalizationDto in result.Items)
        {
            // the data returned is from Db or from memory cache
            var groceryLocalizations = await groceryLocalizationRepository.GetAllByGroceryId(groceryWithFullLocalizationDto.Id);

            if (groceryLocalizations.Count > 0)
            {
                foreach (var groceryLocalization in groceryLocalizations)
                {
                    var groceryLocalizationDto = mapper.Map<GroceriesLocalizationForAdminDto>(groceryLocalization);
                    groceryWithFullLocalizationDto.GroceryLocalizationDtos.Add(groceryLocalizationDto);
                }
            }
        }
    }

    #endregion

    #region OnboardingPage Localization
    public static async Task FillOnboardingPageLocalizations(GetOnboardingPageForAdminQueryDto dto, IOnboardingPageLocalizationRepository onboardingPageLocalizationRepository, IMapper mapper)
    {
        var onboardingPageLocalizations = await onboardingPageLocalizationRepository.GetAll()
            .Where(ol => ol.OnboardingPageId == dto.Id)
            .ToListAsync();

        if (onboardingPageLocalizations.Count > 0)
        {
            foreach (var onboardingPageLocalization in onboardingPageLocalizations)
            {
                var onboardingPageLocalizationDto = mapper.Map<OnboardingPageForAdminLocalizationDto>(onboardingPageLocalization);
                dto.LocalizationDtos.Add(onboardingPageLocalizationDto);
            }
        }
    }

    public static async Task FillOnboardingPageLocalizations(PaginatedList<GetOnboardingPagesWithPaginationDto> result, Guid userLanguageId,
                IOnboardingPageLocalizationRepository onboardingPageLocalizationRepository)
    {
        foreach (var onboardingPageDto in result.Items)
        {
            // get localization of onboardingPage and language
            var onboardingPageLocalizations = await onboardingPageLocalizationRepository.GetAll()
            .Where(ol => ol.OnboardingPageId == onboardingPageDto.Id && ol.LanguageId == userLanguageId)
            .ToListAsync();

            if (onboardingPageLocalizations.Count > 0)
            {
                // replace with localization if existed, or do nothing
                onboardingPageDto.Title = onboardingPageLocalizations.FirstOrDefault(ol => ol.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.Title) != null
                    ? onboardingPageLocalizations.FirstOrDefault(pl => pl.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.Title)!.Value
                    : onboardingPageDto.Title;

                onboardingPageDto.Description = onboardingPageLocalizations.FirstOrDefault(pl => pl.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.Description) != null
                    ? onboardingPageLocalizations.FirstOrDefault(pl => pl.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.Description)!.Value
                    : onboardingPageDto.Description;

                onboardingPageDto.AssetPath = onboardingPageLocalizations.FirstOrDefault(pl => pl.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.AssetPath) != null
                    ? onboardingPageLocalizations.FirstOrDefault(pl => pl.OnboardingPageLocalizationFieldType == (int)OnboardingPageLocalizationFieldType.AssetPath)!.Value
                    : onboardingPageDto.AssetPath;
            }
        }
        
    }

    #endregion

    #region Glossary Localization

    public static async Task FillGlossaryLocalizations(PaginatedList<GetGlossariesForAdminWithPaginationQueryDto> result, IGlossaryLocalizationRepository glossaryLocalizationRepository, IMapper mapper)
    {
        foreach (var glossaryDto in result.Items)
        {
            // get all glossary localization of specific glossary by its id and fill Dto with them 
            var glossaryLocalizations = await glossaryLocalizationRepository.GetGlossaryLocalizationByGlossaryIdAsync(glossaryDto.Id);

            if (glossaryLocalizations.Count > 0)
            {
                foreach (var glossaryLocalization in glossaryLocalizations)
                {
                    var glossaryLocalizationDto = mapper.Map<GlossaryLocalizationForAdminDto>(glossaryLocalization);
                    glossaryDto.GlossaryLocalizationDtos.Add(glossaryLocalizationDto);
                }
            }
        }
    }

    public static async Task FillGlossaryLocalizations(GetGlossaryForAdminQueryDto dto, IGlossaryLocalizationRepository glossaryLocalizationRepository, IMapper mapper)
    {
        // get all glossary localization of specific glossary by its id and fill Dto with them 
        var glossaryLocalizations = await glossaryLocalizationRepository.GetGlossaryLocalizationByGlossaryIdAsync(dto.Id);

        if (glossaryLocalizations.Count > 0)
        {
            foreach (var glossaryLocalization in glossaryLocalizations)
            {
                var glossaryLocalizationDto = mapper.Map<GetGlossaryLocalizationForAdminQueryDto>(glossaryLocalization);
                dto.GlossaryLocalizationDtos.Add(glossaryLocalizationDto);
            }
        }

    }
    #endregion
}
