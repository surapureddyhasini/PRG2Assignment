using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace S10273987F_PRG2Assignment
{
    class OrderedFoodItem : FoodItem // ORDEREDFOODITEM is a FOODITEM with order specific details
    {
        public int QtyOrdered { get; set; }
        public double SubTotal { get; set; }

        // Constructor
        public OrderedFoodItem(string name, string desc, double price, string custom, int quantity)
            : base(name, desc, price, custom)
        {
            QtyOrdered = quantity;
            SubTotal = price * quantity;
        }

        public double CalculateSubtotal()
        {
            SubTotal = ItemPrice * QtyOrdered;
            return SubTotal;
        }

    }
}

