using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostQuery;

public class GetPostQueryValidator : AbstractValidator<GetPostQuery>
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<GetPostQueryValidator> _logger;
    private readonly ILanguageRepository _languageRepository;
    private readonly ICountryRepository _countryRepository;

    public GetPostQueryValidator(IPostRepository postRepository,
                                 ILogger<GetPostQueryValidator> logger,
                                 ILanguageRepository languageRepository,
                                 ICountryRepository countryRepository)
    {
        _postRepository = postRepository;
        _logger = logger;
        _languageRepository = languageRepository;
        _countryRepository = countryRepository;
        RuleFor(p => p.postId)
              .NotEmpty().WithMessage("Post Id should passed")
              .CustomAsync(async (name, context, cancellationToken) =>
              {
                  if (!await IsPostExisted(context.InstanceToValidate))
                  {
                      context.AddFailure("PostId", "PostId must be correct");
                  }
              });

        RuleFor(p => p.LanguageId)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsLanguageIdParamAcceptable(context.InstanceToValidate))
                {
                    context.AddFailure("LanguageId", "LanguageId must be correct");
                }
            });

        RuleFor(p => p.CountryId)
            .NotEmpty().WithMessage("Country must be chosen or Id must be passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsCountryIdParamAcceptable(context.InstanceToValidate))
                {
                    context.AddFailure("CountryId", "Country must be chosen and country Id must be correct");
                }
            });

    }
    public async Task<bool> IsPostExisted(GetPostQuery query)
    {
        return await _postRepository.GetByIdAsync(query.postId) != null;
    }

    public async Task<bool> IsLanguageIdParamAcceptable(GetPostQuery query)
    {
        // LanguageId is acceptable if it is null(Guid.Empty) or valid
        if (query.LanguageId == Guid.Empty)
        {
            return true;
        }
        else
        {//check if LanguageId valid
            return await _languageRepository.GetByIdAsync(query.LanguageId) != null;
        }
    }

    public async Task<bool> IsCountryIdParamAcceptable(GetPostQuery query)
    {
        // CountryId is acceptable if it is valid
        if (query.CountryId == Guid.Empty)
        {
            return false;
        }
        else
        {//check if CountryId valid
            var country = await _countryRepository.GetByIdAsync(query.CountryId);
            if (country == null)
            {
                return false;
            }
            else
            {
                var post = await _postRepository.GetPostWithGroceryByPostId(query.postId);
                if (post == null)
                    return false;
                else
                    return country.Id == post.Grocery.CountryId;
            }
            //return await _countryRepository.GetByIdAsync(query.CountryId) != null;
        }
    }
}
