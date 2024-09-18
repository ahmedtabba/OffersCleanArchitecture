using Newtonsoft.Json;

namespace Offers.CleanArchitecture.Api.NeededDto.UserGroup;

public class UpdateGroupCommandDto
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "roles")]
    public List<string> Roles { get; set; }
}
