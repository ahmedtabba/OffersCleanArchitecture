using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationGroup;
public class GetNotificationGroupQueryValidator : AbstractValidator<GetNotificationGroupQuery>
{
    private readonly ILogger<GetNotificationGroupQueryValidator> _logger;
    private readonly INotificationGroupRepository _notificationGroupRepository;

    public GetNotificationGroupQueryValidator(ILogger<GetNotificationGroupQueryValidator> logger,
                                              INotificationGroupRepository notificationGroupRepository)
    {
        _logger = logger;
        _notificationGroupRepository = notificationGroupRepository;

        RuleFor(n=>n.NotificationGroupId)
            .NotEmpty().WithMessage("NotificationGroup must has Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsNotificationGroupExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Get Notification Group", "Notification Group is not found");
                }
            });
    }
    public async Task<bool> IsNotificationGroupExisted(GetNotificationGroupQuery query)
    {
        return await _notificationGroupRepository.GetByIdAsync(query.NotificationGroupId) != null;
    }
}
