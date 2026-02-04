//==========================================================
// Student Number : S10273987F
// Student Name : Surapureddy Hasini
// Partner Name : Ng Jia Ying
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10273987F_PRG2Assignment
{
    class Restaurant //RESTAURANT stores multiple MENUS and SPECIALOFFERS and receives multiple ORDERS, create a menu list to store menu objects, create a specialoffer list to store specialoffer objects, create an order list to store order obkects
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantEmail { get; set; }



        public List<Menu> MenuList { get; set; } = new List<Menu>();
        public List<SpecialOffer> SpecialOfferList { get; set; } = new List<SpecialOffer>();
        public List<Order> OrderList { get; set; } = new List<Order>();

        public void DisplayOrders()
        {
            foreach (Order order in OrderList)
            {
                Console.WriteLine(order);
            }
        }


        public void DisplaySpecialOffers()
        {
            foreach (SpecialOffer offer in SpecialOfferList)
            {
                Console.WriteLine(offer);
            }
        }

        public void DisplayMenu()
        {
            foreach (Menu menu in MenuList)
            {
                Console.WriteLine(menu);
            }
        }


        public void AddMenu(Menu menu)
        {
            MenuList.Add(menu);
        }

        public bool RemoveMenu(Menu menu)
        {
            return MenuList.Remove(menu);
        }



        public override string ToString()
        {
            return $"Restaurant ID: {RestaurantId}\nRestaurant Name: {RestaurantName}\nRestaurant Email: {RestaurantEmail}";
        }

        //Constructor
        public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
        {
            RestaurantId = restaurantId;
            RestaurantName = restaurantName;
            RestaurantEmail = restaurantEmail;
        }

    }
}
