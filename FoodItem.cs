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
    class FoodItem //FOODITEMS are on MENUS and they can be ORDERED
    {
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public double ItemPrice { get; set; }
        
        public string Customise { get; set; }

        public override string ToString()
        {
            return "Item name:" + ItemName + "Item description:" + ItemDesc + "Item price" + ItemPrice + "Customise" + Customise;
        }

        //constructor
        public FoodItem(string name, string desc, double price, string custom)
        {
             ItemName = name;
            ItemDesc = desc;
            ItemPrice = price;
            Customise = custom;

        }

    }

}
