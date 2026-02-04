//==========================================================
// Student Number : S10273987F
// Student Name : Surapureddy Hasini
// Partner Name : Ng Jia Ying
//==========================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace S10273987F_PRG2Assignment
{
    class DataLoader // loads, reads and stores the data in the csv files + creates objects (gets called in the program.cs later)
    {
        public static List<Restaurant> LoadRestaurants(string filePath)
        {
            List<Restaurant> restaurants = new List<Restaurant>();

            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++) // skip header
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] parts = lines[i].Split(',');

                string restaurantId = parts[0].Trim();
                string name = parts[1].Trim();
                string email = parts[2].Trim();

                Restaurant r = new Restaurant(restaurantId, name, email);

                // each restaurant must have at least one menu
                r.AddMenu(new Menu(restaurantId, name));

                restaurants.Add(r);
            }

            return restaurants;
        }

        public static void LoadFoodItems(string filePath, List<Restaurant> restaurants)
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++) // skip header
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] parts = lines[i].Split(',');

                string restaurantId = parts[0].Trim();
                string itemName = parts[1].Trim();
                string desc = parts[2].Trim();
                double price = Convert.ToDouble(parts[3].Trim());

                // fooditems csv has no customise column
                FoodItem fi = new FoodItem(itemName, desc, price,"");

                // find restaurant + add into its first menu (Main Menu)
                foreach (Restaurant r in restaurants)
                {
                    if (r.RestaurantId == restaurantId)
                    {
                        r.MenuList[0].AddFoodItem(fi);
                        break;
                    }
                }
            }
        }
    }
}
