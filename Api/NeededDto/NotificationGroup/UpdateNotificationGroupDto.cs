namespace Offers.CleanArchitecture.Api.NeededDto.NotificationGroup;

public class UpdateNotificationGroupDto
{
    public string Name { get; set; } = null!;
    public List<Guid> NotificationsIds { get; set; } = new List<Guid>();
}
