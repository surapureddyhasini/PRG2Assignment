//==========================================================
// Student Number : S10273987F
// Student Name : Surapureddy Hasini
// Partner Name : Ng JiaYing
//==========================================================


using S10273987F_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace S10273987F_PRG2Assignment
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataDir = Path.Combine(baseDir, "csv_files");

            // Load restaurants from CSV
            List<Restaurant> restaurants =
                DataLoader.LoadRestaurants(Path.Combine(dataDir, "restaurants.csv"));

            // Load food items and assign them to restaurants
            DataLoader.LoadFoodItems(Path.Combine(dataDir, "fooditems - Copy.csv"), restaurants);
            
            // Count restaurants
            int restaurantCount = restaurants.Count;

            // Count food items
            int foodItemCount = 0;
            foreach (Restaurant r in restaurants)
            {
                foodItemCount += r.MenuList[0].FoodItemList.Count;
            }

            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
            Console.WriteLine($"{restaurantCount} restaurants loaded!");
            Console.WriteLine($"{foodItemCount} food items loaded!");
        }
    }
}
