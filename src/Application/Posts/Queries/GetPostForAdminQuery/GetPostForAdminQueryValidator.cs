using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostForAdminQuery;
public class GetPostForAdminQueryValidator : AbstractValidator<GetPostForAdminQuery>
{
    private readonly ILogger<GetPostForAdminQueryValidator> _logger;
    private readonly IPostRepository _postRepository;

    public GetPostForAdminQueryValidator(ILogger<GetPostForAdminQueryValidator> logger,
                                         IPostRepository postRepository)
    {
        _logger = logger;
        _postRepository = postRepository;

        RuleFor(p => p.PostId)
             .NotEmpty().WithMessage("Post Id should passed")
             .CustomAsync(async (name, context, cancellationToken) =>
             {
                 if (!await IsPostExisted(context.InstanceToValidate))
                 {
                     context.AddFailure("PostId", "PostId must be correct");
                 }
             });
    }

    public async Task<bool> IsPostExisted(GetPostForAdminQuery query)
    {
        return await _postRepository.GetByIdAsync(query.PostId) != null;
    }
}
