using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Posts.Queries;
public class PostBaseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
    [JsonPropertyName("assetPath")]
    public string ImagePath { get; set; } = null!;
    public Guid GroceryId { get; set; }
    public string GroceryName { get; set; } = null!;
    public DateTime? PublishDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    // represent if post is active and datetime now is between start and end date of post
    public bool IsLiven { get; set; }
}
