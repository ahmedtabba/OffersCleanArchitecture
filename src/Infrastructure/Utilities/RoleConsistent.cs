namespace Offers.CleanArchitecture.Infrastructure.Utilities;

public static class RoleConsistent
{
    //public static class Operations
    //{
    //public static string CanAddUsers = Roles.SuperAdmin;
    //public static string CanBrowseUsers = Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.Tester;
    //public static string CanDeleteUsers = Roles.SuperAdmin;
    //public static string CanEditUsers = Roles.SuperAdmin + "," + Roles.Admin;
    //public static string CanResetUsersPasswords = Roles.SuperAdmin + "," + Roles.Admin;
    //}
    //public static class Roles
    //{
    //    public static string SuperAdmin = "Administrator";
    //    public static string Finance = "Finance";
    //    public static string PowerUser = "PowerUser";
    //    public static string Admin = "AdminManager";
    //    public static string Tester = "Tester";
    //}


    public class Country//done
    {
        public const string Delete = @"Country\Delete Countries";
        public const string Add = @"Country\Add Countries";
        public const string Edit = @"Country\Edit Countries";
        public const string BrowseTimeZones = @"Country\Browse TimeZones";
        public List<string> Roles = [Delete, Add, Edit, BrowseTimeZones];
    }

    public class Glossary//done
    {
        public const string Edit = @"Glossary\Edit Glossaries with Localization";
        public const string BrowseGlossariesForAdmin = @"Glossary\Browse Glossaries with Localization";
        public List<string> Roles = [Edit, BrowseGlossariesForAdmin];
    }

    public class Grocery//done
    {
        public const string Browse = @"Grocery\Browse Groceries with Localization";
        public const string Delete = @"Grocery\Delete Groceries";
        public const string Add = @"Grocery\Add Groceries with Localization";
        public const string Edit = @"Grocery\Edit Groceries and Localization";
        public const string AddToFavorite = @"Grocery\Add Grocery To Favorite";
        public const string RemoveFromFavorite = @"Grocery\Remove Grocery From Favorite";
        public const string BrowseFavorite = @"Grocery\Browse Favorite Groceries";
        public List<string> Roles = [Browse, Delete, Add, Edit, AddToFavorite, RemoveFromFavorite, BrowseFavorite];
    }
    public class Identity//done
    {
        public const string BrowseUsers = @"Identity\Browse Users";
        public const string Delete = @"Identity\Delete Users";
        public const string Add = @"Identity\Add Users";
        public const string Edit = @"Identity\Edit Users";
        public const string ResetMyPassword = @"Identity\Reset Password";
        public const string ResetPasswordByAdmin = @"Identity\Reset Password for other users";
        public List<string> Roles = [BrowseUsers, Delete, Add, Edit, ResetMyPassword , ResetPasswordByAdmin];
    }

    public class Language// done
    {
        public const string Delete = @"Language\Delete Languages";
        public const string Add = @"Language\Add Languages";
        public const string Edit = @"Language\Edit Languages";
        public List<string> Roles = [Delete, Add, Edit];
    }

    public class NotificationGroup// done
    {
        public const string Browse = @"Notification Group\Browse Notification Groups";
        public const string BrowseNotifications = @"Notification Group\Browse Notifications of application";
        public const string Delete = @"Notification Group\Delete Notification Groups";
        public const string Add = @"Notification Group\Add Notification Groups";
        public const string Edit = @"Notification Group\Edit Notification Groups";
        public List<string> Roles = [Browse, BrowseNotifications, Delete, Add, Edit];
    }

    public class OnboardingPage //done
    {
        public const string BrowseOnboardingPageWithLocalization = @"Onboarding Page\Browse Onboarding Pages with Localization";
        public const string Delete = @"Onboarding Page\Delete Onboarding Pages";
        public const string Add = @"Onboarding Page\Add Onboarding Pages";
        public const string Edit = @"Onboarding Page\Edit Onboarding Pages";
        public List<string> Roles = [BrowseOnboardingPageWithLocalization, Delete, Add, Edit];
    }

    public class Post// done
    {
        public const string BrowseForAdmin = @"Post\Browse Posts with Localization";
        public const string BrowseFavoritePosts = @"Post\Browse Posts according to favorite groceries";
        public const string Delete = @"Post\Delete Posts";
        public const string Add = @"Post\Add Posts";
        public const string Edit = @"Post\Edit Posts";
        public List<string> Roles = [BrowseForAdmin,BrowseFavoritePosts,Delete, Add, Edit];

    }
    public class UserGroup
    {
        public const string BrowseUserGroups = @"User Group\Browse User Groups";
        public const string Delete = @"User Group\Delete UserGroups";
        public const string Add = @"User Group\Add User Groups";
        public const string Edit = @"User Group\Edit User Groups";
        public const string BrowseRoles = @"User Group\Browse Roles";
        public List<string> Roles = [BrowseUserGroups, Delete, Add, Edit, BrowseRoles];
    }

    public class UserNotification
    {
        public const string Browse = @"User Notification\Browse User Notifications";
        public const string MakeAsRead = @"User Notification\Make User Notifications as Read";
        public const string MakeAsUnRead = @"User Notification\Make User Notifications as Unread";
        public List<string> Roles = [Browse, MakeAsRead,MakeAsUnRead];
    }

    public static Dictionary<string, List<string>> Groups = new Dictionary<string, List<string>>();

    static RoleConsistent()
    {
        Groups.Add("Countries", new List<string>() { "Country" });
        Groups.Add("Glossaries", new List<string>() { "Glossary" });
        Groups.Add("Groceries", new List<string>() { "Grocery" });
        Groups.Add("Identities", new List<string>() { "Identity" });
        Groups.Add("Languages", new List<string>() { "Language" });
        Groups.Add("Notification Groups", new List<string>() { "Notification Group" });
        Groups.Add("Onboarding Pages", new List<string>() { "Onboarding Page" });
        Groups.Add("Posts", new List<string>() { "Post" });
        Groups.Add("User Groups", new List<string>() { "User Group" });
        Groups.Add("User Notifications", new List<string>() { "User Notification" });
    }
}
