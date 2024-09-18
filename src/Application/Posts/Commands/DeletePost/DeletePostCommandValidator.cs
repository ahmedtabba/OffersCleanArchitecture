using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Commands.DeleteGrocery;

namespace Offers.CleanArchitecture.Application.Posts.Commands.DeletePost;
public class DeletePostCommandValidator : AbstractValidator<DeletePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<DeletePostCommandValidator> _logger;

    public DeletePostCommandValidator(IPostRepository postRepository, ILogger<DeletePostCommandValidator> logger)
    {
        _postRepository = postRepository;
        _logger = logger;
        RuleFor(p => p.postId)
            .NotEmpty().WithMessage("Post Id is requeired")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsPostExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Delete Post", "PostId must be correct");
                }
            });
    }

    public async Task<bool> IsPostExisted(DeletePostCommand command)
    {
        return await _postRepository.GetByIdAsync(command.postId) != null;
    }
}
