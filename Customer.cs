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
    class Customer //CUSTOMER places multiple ORDERS, create an order list to store order objects
    {
        public string EmailAddress { get; set; }
        public string CustomerName { get; set; }

        public List<Order> customerOrderList { get; set; } = new List<Order>();

        public void AddOrder(Order order)
        {
            customerOrderList.Add(order);
        }

        public void DisplayAllOrders()
        {
            foreach (Order order in customerOrderList)
            {
                Console.WriteLine(order);
            }
        }

        public bool RemoveOrder(Order order)
        {
            return customerOrderList.Remove(order);
        }


        public override string ToString()
        {
            return $"Customer Name: {CustomerName}\nEmail Address: {EmailAddress}";
        }

        public Customer(string emailAddress, string customerName)
        {
            EmailAddress = emailAddress;
            CustomerName = customerName;

        }
    }

}