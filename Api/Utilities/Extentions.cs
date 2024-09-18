using Offers.CleanArchitecture.Api.NeededDto.Glossary;
using Offers.CleanArchitecture.Api.NeededDto.Grocery;
using Offers.CleanArchitecture.Api.NeededDto.OnboardingPage;
using Offers.CleanArchitecture.Api.NeededDto.Post;
using Offers.CleanArchitecture.Api.NeededDto.Role;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Localization;
using Offers.CleanArchitecture.Application.NotificationGroups.Queries;
using Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotifications;
using Offers.CleanArchitecture.Application.UserNotifications.Queries.GetAllUserNotifications;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Enums;
using Offers.CleanArchitecture.Infrastructure.Identity;
using Offers.CleanArchitecture.Infrastructure.Utilities;

namespace Offers.CleanArchitecture.Api.Utilities;

public static class Extentions
{
    #region FileDto
    public static List<FileDto> ToFileDtoList(this List<IFormFile> files)
    {
        return files.Select(file => new FileDto
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Content = file.OpenReadStream()
        }).ToList();
    }

    public static FileDto ToFileDto(this IFormFile file)
    {
        return new FileDto
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Content = file.OpenReadStream()
        };
    }

    #endregion

    #region Guid
    public static Guid ToGuid(this string guidString)
    {
        var isValidGuid = Guid.TryParse(guidString, out var guid);
        if(isValidGuid)
            return guid;
        else
            throw new Exception("Guid passed is invalid");
    }
    #endregion

    #region Localization

    public static List<GroceryLocalizationApp> ToGroceryLocalizationAppList(this GroceryLocalizationDto[] groceryLocalizationDtos)
    {
        var res = new List<GroceryLocalizationApp>();
        foreach (var item in groceryLocalizationDtos)
        {
            var isValidEnum = Enum.TryParse(item.FieldType.ToString(),false, out GroceryLocalizationFieldType enumParsed) 
                              && Enum.IsDefined(typeof(GroceryLocalizationFieldType), enumParsed);
            if (!isValidEnum)
                throw new Exception("Some field types of grocery localization are invalid");
            var temp = new GroceryLocalizationApp
            {
                LanguageId = item.LanguageId,
                FieldType = enumParsed,
                Value = item.Value,
            };
            res.Add(temp);
        }


        return res;
    }


    public static List<PostLocalizationApp> ToPostLocalizationAppList(this PostLocalizationDto[] postLocalizationDtos)
    {
        var res = new List<PostLocalizationApp>();
        foreach (var item in postLocalizationDtos)
        {
            var isValidEnum = Enum.TryParse(item.FieldType.ToString(), false, out PostLocalizationFieldType enumParsed)
                              && Enum.IsDefined(typeof(PostLocalizationFieldType), enumParsed);
            if (!isValidEnum)
                throw new Exception("Some field types of grocery localization are invalid");
            var temp = new PostLocalizationApp
            {
                LanguageId = item.LanguageId,
                FieldType = enumParsed,
                Value = item.Value,
            };
            res.Add(temp);
        }


        return res;
    }

    public static List<PostLocalizationAssetsApp> ToPostLocalizationAssetsAppList(this PostLocalizationAssetsDto[] postLocalizationImagesDtos)
    {
        var res = new List<PostLocalizationAssetsApp>();
        foreach (var item in postLocalizationImagesDtos)
        {
            var temp = new PostLocalizationAssetsApp
            {
                LanguageId = item.LanguageId,
                File = item.Asset.ToFileDto(),
            };
            res.Add(temp);
        }
        return res;
    }

    public static List<OnboardingPageLocalizationApp> ToOnboardingPageLocalizationAppList(this OnboardingPageLocalizationDto[] onboardingPageLocalizationDtos)
    {
        var res = new List<OnboardingPageLocalizationApp>();
        foreach (var item in onboardingPageLocalizationDtos)
        {
            var isValidEnum = Enum.TryParse(item.FieldType.ToString(), false, out OnboardingPageLocalizationFieldType enumParsed)
                              && Enum.IsDefined(typeof(OnboardingPageLocalizationFieldType), enumParsed);
            if (!isValidEnum)
                throw new Exception("Some field types of OnboardingPage localization are invalid");
            var temp = new OnboardingPageLocalizationApp
            {
                LanguageId = item.LanguageId,
                FieldType = enumParsed,
                Value = item.Value,
            };
            res.Add(temp);
        }
        return res;
    }

    public static List<OnboardingPageLocalizationAssetApp> ToOnboardingPageLocalizationAssetsAppList(this OnboardingPageLocalizationAssetsDto[] onboardingPageLocalizationAssetsDtos)
    {
        var res = new List<OnboardingPageLocalizationAssetApp>();
        foreach (var item in onboardingPageLocalizationAssetsDtos)
        {
            var temp = new OnboardingPageLocalizationAssetApp
            {
                LanguageId = item.LanguageId,
                Asset = item.Asset.ToFileDto(),
            };
            res.Add(temp);
        }
        return res;
    }

    public static List<GlossaryLocalizationApp> ToGlossaryLocalizationAppList(this GlossaryLocalizationDto[] glossaryLocalizationDtos)
    {
        var res = new List<GlossaryLocalizationApp>();
        foreach (var item in glossaryLocalizationDtos)
        {
            var temp = new GlossaryLocalizationApp
            {
                LanguageId = item.LanguageId,
                Value = item.Value,
            };
            res.Add(temp);
        }

        return res;
    }
    #endregion

    #region JobRole

    public static JobRole ToJobRole (this int intJobRole)
    {
        var isValidEnum = Enum.TryParse(intJobRole.ToString(), false, out JobRole enumParsed)
                              && Enum.IsDefined(typeof(JobRole), enumParsed);
        if (!isValidEnum)
            throw new Exception("JobRole parameter is invalid");
        return enumParsed;
    }

    #endregion

    #region TreeView

    public static List<TreeViewItemModel> BuildTree(this List<IApplicationRole> roles, List<IApplicationRole> selectedRoles = null, bool isGroomingEnabled = true)
    {
        List<TreeViewItemModel> result = new List<TreeViewItemModel>();

        foreach (var role in roles.OrderBy(c => c.Name))
        {
            var nodes = !string.IsNullOrEmpty(role.Description) ? role.Description.Split('\\') : role.Name.Split('\\');
            var currentLevel = result.SingleOrDefault(c => c.Text == nodes.First());

            if (currentLevel == null)
            {
                currentLevel = new TreeViewItemModel() { Text = nodes.First()/*, Expanded = false */};
                result.Add(currentLevel);
            }

            foreach (var node in nodes.Skip(1))
            {
                var treeItem = getItemByText(currentLevel, node);
                if (treeItem == null)
                {
                    treeItem = new TreeViewItemModel() { Text = node, Id = string.Empty };
                    currentLevel.Items.Add(treeItem);
                }
                currentLevel = treeItem;
            }

            currentLevel.Id = role.Id;
            if (selectedRoles != null)
                currentLevel.Checked = selectedRoles.Any(d => d.Id == role.Id);
        }

        if (isGroomingEnabled)
            return groomingRole(result);
        else
            return result;
    }

    public static List<TreeViewItemModel> BuildTree(this List<GetNotificationsDto> notifications, List<GetNotificationsDto> selectedNotification = null, bool isGroomingEnabled = true)
    {
        List<TreeViewItemModel> result = new List<TreeViewItemModel>();

        foreach (var notification in notifications.OrderBy(c => c.Name))
        {
            var nodes = !string.IsNullOrEmpty(notification.Name) ? notification.Name.Split('\\') : notification.Name.Split('\\');
            var currentLevel = result.SingleOrDefault(c => c.Text == nodes.First());

            if (currentLevel == null)
            {
                currentLevel = new TreeViewItemModel() { Text = nodes.First()/*, Expanded = false */};
                result.Add(currentLevel);
            }

            foreach (var node in nodes.Skip(1))
            {
                var treeItem = getItemByText(currentLevel, node);
                if (treeItem == null)
                {
                    treeItem = new TreeViewItemModel() { Text = node, Id = string.Empty };
                    currentLevel.Items.Add(treeItem);
                }
                currentLevel = treeItem;
            }

            currentLevel.Id = notification.Id.ToString();
            if (selectedNotification != null)
                currentLevel.Checked = selectedNotification.Any(d => d.Id == notification.Id);
        }

        if (isGroomingEnabled)
            return groomingNotification(result);
        else
            return result;
    }


    public static List<TreeViewItemModel> groomingRole(List<TreeViewItemModel> tree)
    {
        List<TreeViewItemModel> result = new List<TreeViewItemModel>();
        foreach (var group in RoleConsistent.Groups)
        {
            TreeViewItemModel node = new TreeViewItemModel();
            // node.Expanded = true;
            node.Text = group.Key;
            node.Id = null;
            //node.HtmlAttributes.Add("class", "box-title");

            foreach (var groupItem in group.Value)
            {
                node.Items.Add(tree.Where(c => c.Text == groupItem).FirstOrDefault());
                node.Items[node.Items.Count - 1].Checked = node.Items[node.Items.Count - 1].Items.All(x => x.Checked);
                node.Items[node.Items.Count - 1].Simi_Checked = node.Items[node.Items.Count - 1].Items.Any(x => x.Checked);
            }

            result.Add(node);
            result[result.Count - 1].Checked = result[result.Count - 1].Items.All(x => x.Checked);
            result[result.Count - 1].Simi_Checked = result[result.Count - 1].Items.Any(x => x.Simi_Checked);
        }

        return result;
    }

    public static List<TreeViewItemModel> groomingNotification(List<TreeViewItemModel> tree)
    {
        List<TreeViewItemModel> result = new List<TreeViewItemModel>();
        foreach (var group in NotificationConsistent.Groups)
        {
            TreeViewItemModel node = new TreeViewItemModel();
            // node.Expanded = true;
            node.Text = group.Key;
            node.Id = null;
            //node.HtmlAttributes.Add("class", "box-title");

            foreach (var groupItem in group.Value)
            {
                node.Items.Add(tree.Where(c => c.Text == groupItem).FirstOrDefault());
                node.Items[node.Items.Count - 1].Checked = node.Items[node.Items.Count - 1].Items.All(x => x.Checked);
                node.Items[node.Items.Count - 1].Simi_Checked = node.Items[node.Items.Count - 1].Items.Any(x => x.Checked);
            }

            result.Add(node);
            result[result.Count - 1].Checked = result[result.Count - 1].Items.All(x => x.Checked);
            result[result.Count - 1].Simi_Checked = result[result.Count - 1].Items.Any(x => x.Simi_Checked);
        }

        return result;
    }

    private static TreeViewItemModel getItemByText(TreeViewItemModel root, string text)
    {
        foreach (var item in root.Items)
            if (item.Text == text)
                return item;
            else
            {
                var subItem = getItemByText(item, text);
                if (subItem != null)
                    return subItem;
            }

        return null;
    }

    public static List<string> ToStringList(this List<Guid> guids)
    {
        var result = new List<string>();
        foreach (var item in guids)
        {
            result.Add(item.ToString());
        }
        return result;    

    }
    #endregion

    #region PostFilter

    public static PostFilter ToPostFilter(this int? intPostFilter)
    {
        var isValidEnum = Enum.TryParse(intPostFilter.ToString(), false, out PostFilter enumParsed)
                              && Enum.IsDefined(typeof(PostFilter), enumParsed);
        if (!isValidEnum)
            return PostFilter.All;
        return enumParsed;
    }

    #endregion
}
