using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Offers.CleanArchitecture.Application.Utilities;
public static class GlossaryConsistent
{
    public static class Main
    {
        public static KeyValuePair<string, string> Home  = new KeyValuePair<string, string>(key: "Home", value: "Home");
        public static KeyValuePair<string, string> AboutUs  = new KeyValuePair<string, string>(key: "About-us", value: "About us");
        public static KeyValuePair<string, string> Favorite = new KeyValuePair<string, string>(key: "Favorite", value: "Favorite");
        public static KeyValuePair<string, string> Profile = new KeyValuePair<string, string>(key: "Profile", value: "Profile");

        public static KeyValuePair<string, string> SearchStore = new KeyValuePair<string, string>(key: "Search-store", value: "Search Store");
        public static KeyValuePair<string, string> Search = new KeyValuePair<string, string>(key: "Search", value: "Search");
        public static KeyValuePair<string, string> SeeAll = new KeyValuePair<string, string>(key: "See-all", value: "See All");
        public static KeyValuePair<string, string> Notification = new KeyValuePair<string, string>(key: "Notification", value: "Notification");
        public static KeyValuePair<string, string> New = new KeyValuePair<string, string>(key: "New", value: "New");
        public static KeyValuePair<string, string> Today = new KeyValuePair<string, string>(key: "Today", value: "TODAY");
        public static KeyValuePair<string, string> Yesterday = new KeyValuePair<string, string>(key: "Yesterday", value: "YESTERDAY");
        public static KeyValuePair<string, string> MarkAllAsRead = new KeyValuePair<string, string>(key: "Mark-all-as-read", value: "Mark all as read");
        public static KeyValuePair<string, string> Groceries = new KeyValuePair<string, string>(key: "Groceries", value: "Groceries");
        public static KeyValuePair<string, string> AllGroceries = new KeyValuePair<string, string>(key: "All-groceries", value: "All Groceries");
        public static KeyValuePair<string, string> PopularGroceries = new KeyValuePair<string, string>(key: "Popular-groceries", value: "Popular Groceries");
        public static KeyValuePair<string, string> GroceriesStore = new KeyValuePair<string, string>(key: "Groceries-store", value: "Groceries store");
        public static KeyValuePair<string, string> AllPosts = new KeyValuePair<string, string>(key: "All-posts", value: "All Posts");
        public static KeyValuePair<string, string> All = new KeyValuePair<string, string>(key: "All", value: "All");
        public static KeyValuePair<string, string> Active = new KeyValuePair<string, string>(key: "Active", value: "Active");
        public static KeyValuePair<string, string> InActive = new KeyValuePair<string, string>(key: "In-active", value: "InActive");
        public static KeyValuePair<string, string> RecentlyAdded = new KeyValuePair<string, string>(key: "Recently-added", value: "Recently Added");
        public static KeyValuePair<string, string> SelectCountry = new KeyValuePair<string, string>(key: "Select-country", value: "Select Country");
        public static KeyValuePair<string, string> SelectLanguage = new KeyValuePair<string, string>(key: "Select-language", value: "Select Language");
        public static KeyValuePair<string, string> Back = new KeyValuePair<string, string>(key: "Back", value: "Back");
        public static KeyValuePair<string, string> GetStarted = new KeyValuePair<string, string>(key: "Get-started", value: "Get Started");
    }

    public static class Identity
    {
        public static KeyValuePair<string, string> Introduction = new KeyValuePair<string, string>(key: "Introduction", value: "To make favorite and receive new posts notifications you need to have an account");
        public static KeyValuePair<string, string> SignIn = new KeyValuePair<string, string>(key: "Sign-in", value: "Sign In");
        public static KeyValuePair<string, string> SignUp = new KeyValuePair<string, string>(key: "Sign-up", value: "Sign Up");
        public static KeyValuePair<string, string> ContinueWithoutAccount = new KeyValuePair<string, string>(key: "Continue-without-account", value: "Continue Without Account");
        public static KeyValuePair<string, string> WelcomeMissed = new KeyValuePair<string, string>(key: "Welcome-missed", value: "Hi! Welcome back, you’ve been missed");
        public static KeyValuePair<string, string> Email = new KeyValuePair<string, string>(key: "Email", value: "Email");
        public static KeyValuePair<string, string> EmailExample = new KeyValuePair<string, string>(key: "Email-example", value: "example@gmail.com");
        public static KeyValuePair<string, string> Password = new KeyValuePair<string, string>(key: "Password", value: "Password");
        public static KeyValuePair<string, string> ForgotPassword = new KeyValuePair<string, string>(key: "Forgot-password", value: "Forgot Password?");
        public static KeyValuePair<string, string> OrSignInWith = new KeyValuePair<string, string>(key: "Or-sign-in-with", value: "Or sign in with");
        public static KeyValuePair<string, string> DoNotHaveAnAccount = new KeyValuePair<string, string>(key: "Do-not-have-an-account", value: "Don’t have an account?");
        public static KeyValuePair<string, string> BasicInfo = new KeyValuePair<string, string>(key: "Basic-info", value: "Basic Info");
        public static KeyValuePair<string, string> DarkMode = new KeyValuePair<string, string>(key: "Dark-mode", value: "Dark Mode");
        public static KeyValuePair<string, string> FullName = new KeyValuePair<string, string>(key: "Full-name", value: "Full Name");
        public static KeyValuePair<string, string> DateOfBirth = new KeyValuePair<string, string>(key: "Date-of-birth", value: "Date of Birth");
        public static KeyValuePair<string, string> EmailAddress = new KeyValuePair<string, string>(key: "Email-address", value: "Email Address");
        public static KeyValuePair<string, string> PhoneNumber = new KeyValuePair<string, string>(key: "Phone-number", value: "Phone Number");
        public static KeyValuePair<string, string> Language = new KeyValuePair<string, string>(key: "Language", value: "Language");
        public static KeyValuePair<string, string> Country = new KeyValuePair<string, string>(key: "Country", value: "Country");
        public static KeyValuePair<string, string> ResetPassword = new KeyValuePair<string, string>(key: "Reset-password", value: "Reset password");
        public static KeyValuePair<string, string> ConfirmPassword = new KeyValuePair<string, string>(key: "Confirm-password", value: "Confirm Password");
        public static KeyValuePair<string, string> NewPassword = new KeyValuePair<string, string>(key: "New-password", value: "New Password");
        public static KeyValuePair<string, string> CreateNewPassword = new KeyValuePair<string, string>(key: "Create-new-password", value: "Create New Password");
        public static KeyValuePair<string, string> CreateNewPasswordMessage = new KeyValuePair<string, string>(key: "Create-new-password-message", value: "Your new password must be different from previously used password.");
        public static KeyValuePair<string, string> SaveChanges = new KeyValuePair<string, string>(key: "Save-changes", value: "Save changes");
        public static KeyValuePair<string, string> CreateAccount = new KeyValuePair<string, string>(key: "Create-account", value: "Create Account");
        public static KeyValuePair<string, string> CreateAccountMessage = new KeyValuePair<string, string>(key: "Create-account-message", value: "Fill your information below or register with your social account.");
        public static KeyValuePair<string, string> AgreeWith = new KeyValuePair<string, string>(key: "Agree-with", value: "Agree With");
        public static KeyValuePair<string, string> TermsCondition = new KeyValuePair<string, string>(key: "Terms-condition", value: "Terms & Condition");
        public static KeyValuePair<string, string> VerifyCode = new KeyValuePair<string, string>(key: "Verify-code", value: "Verify Code");
        public static KeyValuePair<string, string> VerifyCodeMessage = new KeyValuePair<string, string>(key: "Verify-code-message", value: "please enter the code we just sent to email");
        public static KeyValuePair<string, string> Verify = new KeyValuePair<string, string>(key: "Verify", value: "Verify");
        public static KeyValuePair<string, string> DidNotReceiveOTP = new KeyValuePair<string, string>(key: "Did-not-receive-OTP", value: "Didn’t receive OTP?");
        public static KeyValuePair<string, string> ResendCode = new KeyValuePair<string, string>(key: "Resend-code", value: "Resend code");
    }
}










