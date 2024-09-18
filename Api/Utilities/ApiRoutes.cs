namespace Offers.CleanArchitecture.Api.Utilities;

public static class ApiRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    public const string Base = Root + "/" + Version;


    public static class Identity
    {
        public const string Login = Base + "/users/auth/login";//done
        public const string Delete = Base + "/users/{userId}";
        public const string Update = Base + "/users/{userId}";
        public const string Get = Base + "/users/{userId}";
        public const string GetAll = Base + "/users/";
        public const string Create = Base + "/users/";
        public const string Register = Base + "/users/auth/register";//done
        public const string SignUp = Base + "/users/auth/signUp";
        public const string ResetMyPassword = Base + "/users/password";
        public const string GetMyProfile = Base + "/users/profile";
        public const string ResetPassword = Base + "/users/{userId}/password";
        public const string GetJobRoles = Base + "/users/jobRoles";
    }

    public static class UserGroup
    {
        public const string Create = Base + "/groups";//done and test
        public const string Update = Base + "/groups/{groupId}";//done and test
        public const string GetAll = Base + "/groups";//done and test
        public const string Get = Base + "/groups/{groupId}";//done and test
        public const string Delete = Base + "/groups/{groupId}";//done and test
    }
    public static class Role
    {
        public const string GetAll = Base + "/roles";//done and test
    }

    public static class Manage
    {
        public const string EditProfile = Base + "/users/profile";
        public const string GetProfile = Base + "/users/profile";
    }

    public static class Glossary
    {
        public const string Update = Base + "/glossaries/{glossaryId}";
        public const string GetAllForAdmin = Base + "/glossaries";
        public const string Get = Base + "/glossaries/{glossaryId}";
    }

    public static class NotificationGroup
    {
        public const string Create = Base + "/notificationgroups";
        public const string Update = Base + "/notificationgroups/{notificationGroupId}";
        public const string GetAll = Base + "/notificationgroups";
        public const string Get = Base + "/notificationgroups/{notificationGroupId}";
        public const string Delete = Base + "/notificationgroups/{notificationGroupId}";
    }

    public static class UserNotification
    {
        public const string GetAll = Base + "/usernotifications";
        public const string MakeAsRead = Base + "/usernotifications/read/{userNotificationId}";
        public const string MakeAsUnRead = Base + "/usernotifications/unread/{userNotificationId}";
        public const string MakeAllAsRead = Base + "/usernotifications";
    }

    public static class Notification
    {
        public const string GetAll = Base + "/notifications";
        public const string GetNotificationObjectTypes = Base + "/notifications/notificationObjectTypes";
    }
    public static class Post
    {
        public const string GetAllByGrocery = Base + "/groceries/{groceryId?}/posts";//done+
        public const string GetAllForAdmin = Base + "/groceries/posts/admin";
        public const string GetAllPosts = Base + "/groceries/posts";//done+
        public const string GetAllFavoritePosts = Base + "/groceries/posts/favorites";//done
        public const string GetFilters = Base + "/posts/filters";
        public const string GetPostLocalizationFieldType = Base + "/posts/postLocalizationFieldType";
        public const string Get = Base + "/groceries/posts/{postId}";//done
        public const string GetForAdmin = Base + "/groceries/posts/admin/{postId}";
        public const string Create = Base + "/groceries/{groceryId}/posts";// done+
        public const string Update = Base + "/groceries/posts/{postId}";//done+
        public const string Delete = Base + "/groceries/posts/{postId}";//done+
    }

    public static class Grocery
    {
        public const string GetAll = Base + "/groceries";// done
        public const string GetAllForAdmin = Base + "/groceries/admin";
        public const string Get = Base + "/groceries/{groceryId}";// done
        public const string GetForAdmin = Base + "/groceries/admin/{groceryId}";
        public const string GetGroceryLocalizationFieldType = Base + "/groceries/groceryLocalizationFieldType";
        public const string Create = Base + "/groceries";// done
        public const string Update = Base + "/groceries/{groceryId}";// done
        public const string Delete = Base + "/groceries/{groceryId}";// done


        public const string AddToFavoraite = Base + "/groceries/{groceryId}/favorites";// done
        public const string RemoveFromFavoraite = Base + "/groceries/{groceryId}/favorites";// done
        public const string GetUserFavoraites = Base + "/groceries/favorites";// done
    }

    public static class Country
    {
        public const string GetAll = Base + "/countries";
        public const string Get = Base + "/countries/{countryId}";
        public const string Create = Base + "/countries";
        public const string Update = Base + "/countries/{countryId}";
        public const string Delete = Base + "/countries/{countryId}";
        public const string GetAllTimeZones = Base + "/countries/timezones";
    }

    public static class Language
    {
        public const string GetAll = Base + "/languages";
        public const string GetAllWithGlossaries = Base + "/languages/glossaries";
        public const string GetLanguageWithGlossaries = Base + "/languages/{languageCode}/glossaries";
        public const string Get = Base + "/languages/{languageId}";
        public const string Create = Base + "/languages";
        public const string Update = Base + "/languages/{languageId}";
        public const string Delete = Base + "/languages/{languageId}";
    }

    public static class OnboardingPage
    {
        public const string GetAll = Base + "/onboardingpages";
        public const string GetOnboardingPageLocalizationFieldType = Base + "/onboardingpages/onboardingPageLocalizationFieldType";
        public const string Get = Base + "/onboardingpages/{onboardingPageId}";
        public const string Create = Base + "/onboardingpages";
        public const string Update = Base + "/onboardingpages/{onboardingPageId}";
        public const string Delete = Base + "/onboardingpages/{onboardingPageId}";
    }

}
