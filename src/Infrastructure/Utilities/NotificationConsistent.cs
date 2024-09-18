//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Offers.CleanArchitecture.Infrastructure.Utilities;
//public static class NotificationConsistent
//{
//    public class Post
//    {
//        public const string Add = @"Post\Add Post";
//        public const string Edit = @"Post\Edit Post";
//        public const string Delete = @"Post\Delete Post";
//    }

//    public class Grocery
//    {
//        public const string Add = @"Grocery\Add Grocery";
//        public const string AddToFavorite = @"Grocery\Add Grocery to favorite ";
//        public const string RemoveFromFavorite = @"Grocery\Remove Grocery from favorite ";
//        public const string Edit = @"Grocery\Edit Grocery";
//        public const string Delete = @"Grocery\Delete Grocery";
//    }

//    public static Dictionary<string, List<string>> Groups = new Dictionary<string, List<string>>();

//    static NotificationConsistent()
//    {
//        Groups.Add("Groceries", new List<string>() { "Grocery" });
//        Groups.Add("Posts", new List<string>() { "Post" });
//    }
//}
