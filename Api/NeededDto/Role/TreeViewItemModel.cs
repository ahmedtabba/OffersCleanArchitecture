namespace Offers.CleanArchitecture.Api.NeededDto.Role;

public class TreeViewItemModel
{
    //public bool Expanded { get; set; }
    public string Text { get; set; }
    public string Id { get; set; }
    public bool Checked { get; set; }
    public bool Simi_Checked { get; set; }
    public List<TreeViewItemModel> Items { get; set; }

    public TreeViewItemModel()
    {
        Items = new List<TreeViewItemModel>();
    }
}
