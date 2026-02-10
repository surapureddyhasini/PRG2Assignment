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
    class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderPaymentMethod { get; set; }
        public bool OrderPaid { get; set; }

        public Customer Customer { get; set; }
        public Restaurant Restaurant { get; set; }

        // List to store ordered food items
        public List<OrderedFoodItem> OrderedFoodItemList { get; set; }

        // Constructor
        // When creating from file: pass in the orderId from CSV
        // When creating new order: pass in the next available orderId (which is the generated one right...)
        public Order(int orderId, Customer customer, Restaurant restaurant,
                     DateTime deliveryDateTime, string deliveryAddress)
        {
            this.OrderId = orderId;
            this.Customer = customer;
            this.Restaurant = restaurant;
            this.DeliveryDateTime = deliveryDateTime;
            this.DeliveryAddress = deliveryAddress;
            this.OrderDateTime = DateTime.Now;  // Set to current date/time when order is created
            this.OrderTotal = 0;
            this.OrderStatus = "Pending";
            this.OrderPaymentMethod = "";
            this.OrderPaid = false;
            this.OrderedFoodItemList = new List<OrderedFoodItem>();
        }

        // Overloaded constructor for loading from CSV (includes orderDateTime and status)
        public Order(int orderId, Customer customer, Restaurant restaurant,
                     DateTime deliveryDateTime, string deliveryAddress,
                     DateTime orderDateTime, string orderStatus, double orderTotal,
                     string paymentMethod, bool orderPaid)
        {
            this.OrderId = orderId;
            this.Customer = customer;
            this.Restaurant = restaurant;
            this.DeliveryDateTime = deliveryDateTime;
            this.DeliveryAddress = deliveryAddress;
            this.OrderDateTime = orderDateTime;
            this.OrderTotal = orderTotal;
            this.OrderStatus = orderStatus;
            this.OrderPaymentMethod = paymentMethod;
            this.OrderPaid = orderPaid;
            this.OrderedFoodItemList = new List<OrderedFoodItem>();
        }

        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            OrderedFoodItemList.Add(item);
        }

        public bool RemoveOrderedFoodItem(OrderedFoodItem item)
        {
            return OrderedFoodItemList.Remove(item);
        }

        public void DisplayOrderedFoodItems()
        {
            if (OrderedFoodItemList.Count == 0)
            {
                Console.WriteLine("No items in this order.");
                return;
            }

            Console.WriteLine("Ordered Items:");
            int count = 1;
            foreach (OrderedFoodItem item in OrderedFoodItemList)
            {
                Console.WriteLine($"{count}. {item.ItemName} - {item.QtyOrdered} x ${item.ItemPrice:F2} = ${item.SubTotal:F2}");
                if (!string.IsNullOrEmpty(item.Customise))
                {
                    Console.WriteLine($"   Special Request: {item.Customise}");
                }
                count++;
            }
        }

        // Calculate total, sum of all food items + $5 delivery fee (this is in descriptor), the 30% that goes to Gruberoo is implemented in program.cs later
        public double CalculateOrderTotal()
        {
            double total = 0;

            foreach (OrderedFoodItem item in OrderedFoodItemList)
            {
                total += item.SubTotal;
            }

            total += 5.00; // delivery fee

            OrderTotal = total;
            return OrderTotal;
        }

        public override string ToString()
        {
            return $"{OrderId,-8} {Customer.CustomerName,-12} {Restaurant.RestaurantName,-15} " +
                   $"{DeliveryDateTime:dd/MM/yyyy HH:mm,-20} ${OrderTotal:F2,-8} {OrderStatus}";
        }
    }
}