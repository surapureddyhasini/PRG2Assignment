//==========================================================
// Student Number : S10273987F
// Student Name : Surapureddy Hasini
// Partner Name : Ng Jia Ying 
//==========================================================

using S10273987F_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace S10273987F_PRG2Assignment
{
    class Program
    {

        static void Main(string[] args)
        {   //fix the path thing...
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataDir = Path.Combine(baseDir, "csv_files");


            //FEATURE 1 Load files (restaurants and food items)
            //Load restaurants from CSV
            List<Restaurant> restaurants =
                DataLoader.LoadRestaurants(Path.Combine(dataDir, "restaurants.csv")); //jy use dataloader to load 

            //Load food items and assign them to restaurants
            DataLoader.LoadFoodItems(Path.Combine(dataDir, "fooditems - Copy.csv"), restaurants);

            int restaurantCount = restaurants.Count;

            int foodItemCount = 0;
            foreach (Restaurant r in restaurants)
            {
                foodItemCount += r.MenuList[0].FoodItemList.Count;
            }


            //FEATURE 2 Load files (customers and orders)
            //Load customers from CSV (name,email)
            List<Customer> customers = new List<Customer>();
            Dictionary<string, Customer> customerEmail = new Dictionary<string, Customer>();

            string[] custLines = File.ReadAllLines(Path.Combine(dataDir, "customers.csv"));

            for (int i = 1; i < custLines.Length; i++)
            {
                string[] parts = custLines[i].Split(',');

                string name = parts[0].Trim();
                string email = parts[1].Trim();

                Customer c = new Customer(email, name);
                customers.Add(c);
                customerEmail[email] = c;
            }

            //Load orders from CSV(OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items)
            string[] orderLines = File.ReadAllLines(Path.Combine(dataDir, "orders - Copy.csv"));

            int ordersLoaded = 0;

            for (int i = 1; i < orderLines.Length; i++)
            {
                string[] parts = orderLines[i].Split(new char[] { ',' }, 10);


                int orderId = Convert.ToInt32(parts[0].Trim());
                string customerEmailcsv = parts[1].Trim();
                string restaurantIdcsv = parts[2].Trim();

                string deliveryDate = parts[3].Trim();
                string deliveryTime = parts[4].Trim();
                DateTime deliveryDateTimecsv = DateTime.Parse(deliveryDate + " " + deliveryTime);

                string deliveryAddresscsv = parts[5].Trim();
                DateTime createdDateTime = DateTime.Parse(parts[6].Trim());
                double totalAmount = Convert.ToDouble(parts[7].Trim());
                string status = parts[8].Trim();

                Customer customercsv = customerEmail[customerEmailcsv]; //jy this is find customer by email

                Restaurant restaurantcsv = null;                       //jy and this is find restuarant by id
                foreach (Restaurant r in restaurants)
                {
                    if (r.RestaurantId == restaurantIdcsv)
                    {
                        restaurantcsv = r;
                        break;
                    }
                }

                Order order = new Order(orderId, customercsv, restaurantcsv, //so i create order over here,set payment method and paid to nothing first
                                        deliveryDateTimecsv, deliveryAddresscsv,
                                        createdDateTime, status, totalAmount,
                                        "", false);

                //CHECK
                string itemsRaw = parts[9].Trim();

                if (itemsRaw.StartsWith("\"") && itemsRaw.EndsWith("\""))
                {
                    itemsRaw = itemsRaw.Substring(1, itemsRaw.Length - 2);
                }

                string[] itemParts = itemsRaw.Split('|');

                for (int k = 0; k < itemParts.Length; k++)
                {
                    string oneItem = itemParts[k].Trim();
                    string[] nameQty = oneItem.Split(',', 2);

                    string itemName = nameQty[0].Trim();
                    int qty = Convert.ToInt32(nameQty[1].Trim());

                    //Find matching FoodItem in the restaurant menu to get price and desc
                    double price = 0;
                    string desc = "";

                    List<FoodItem> menuItems = restaurantcsv.MenuList[0].FoodItemList;
                    for (int m = 0; m < menuItems.Count; m++)
                    {
                        if (menuItems[m].ItemName == itemName)
                        {
                            price = menuItems[m].ItemPrice;
                            desc = menuItems[m].ItemDesc;
                            break;
                        }
                    }

                    OrderedFoodItem ofi = new OrderedFoodItem(itemName, desc, price, "", qty);
                    order.AddOrderedFoodItem(ofi);
                }

                customercsv.AddOrder(order);
                restaurantcsv.restaurantOrderList.Add(order);

                ordersLoaded++;
            }

            //MAIN 
            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
            Console.WriteLine($"{restaurantCount} restaurants loaded!");
            Console.WriteLine($"{foodItemCount} food items loaded!");
            Console.WriteLine($"{customers.Count} customers loaded!");
            Console.WriteLine($"{ordersLoaded} orders loaded!");
            Console.WriteLine();


            Stack<Order> refundStack = new Stack<Order>(); //debug


            while (true)
            {
                Console.WriteLine();
                Console.Write(@"===== Gruberoo Food Delivery System =====
1. List all restaurants and menu items
2. List all orders
3. Create a new order
4. Process an order
5. Modify an existing order
6. Delete an existing order
7. Bulk process orders
8. Display total order amount
0. Exit
Enter your choice: ");

                int option = 0;

                while (true)
                {
                    try
                    {
                        option = Convert.ToInt32(Console.ReadLine());
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.Write("Invalid input. Please enter a number (0 to 8): ");
                    }
                    catch (OverflowException)
                    {
                        Console.Write("Invalid input. Please enter a number (0 to 8): ");
                    }
                }

                if (option == 0)
                {
                    break;
                }
                else if (option == 1)
                {
                    ListAllRestaurantsAndMenuItems(restaurants);
                }
                else if (option == 2)
                {
                    ListAllOrders(restaurants);
                }
                else if (option == 3)
                {
                    CreateNewOrder(dataDir, restaurants, customerEmail);
                }
                else if (option == 4)
                {
                    ProcessOrder(restaurants, refundStack);
                }
                else if (option == 5)
                {
                    ModifyExistingOrder(customerEmail);
                }
                else if (option == 6)
                {
                    DeleteExistingOrder(customerEmail, refundStack);
                }
                else if (option == 7)
                {
                    BulkProcessOrders(restaurants, refundStack);
                }
                else if (option == 8)
                {
                    DisplayTotalOrderAmount(restaurants, refundStack);
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter a number from 0 to 8.");
                }
            }


            //FEATURE 3 (List all restaurants and menu items)
            void ListAllRestaurantsAndMenuItems(List<Restaurant> restaurants)
            {
                Console.WriteLine();
                Console.WriteLine(@"All Restaurants and Menu Items
==============================");



                foreach (Restaurant r in restaurants)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");

                    foreach (FoodItem fi in r.MenuList[0].FoodItemList)
                    {
                        Console.WriteLine($"- {fi.ItemName}: {fi.ItemDesc} - ${fi.ItemPrice:F2}");
                    }
                }
            }

            //FEATURE 4 (List all orders with basic information)
            void ListAllOrders(List<Restaurant> restaurants)
            {
                Console.WriteLine();
                Console.WriteLine($"{"All Orders"}\n" + $"{"=========="}\n" + $"{"Order ID",-8} " + $"{"Customer",-12} " + $"{"Restaurant",-15} " + $"{"Delivery Date/Time",-20} " + $"{"Amount",-8} " + $"{"Status"}\n" +
                            $"{"--------",-8} " + $"{"----------",-12} " + $"{"-------------",-15} " + $"{"------------------",-20} " + $"{"------",-8} " + $"{"--------"}");

                foreach (Restaurant r in restaurants)
                {
                    foreach (Order o in r.restaurantOrderList) //jy this order list is for restaurant
                    {
                        Console.WriteLine($"{o.OrderId,-8} " + $"{o.Customer.CustomerName,-12} " + $"{o.Restaurant.RestaurantName,-15} " + $"{o.DeliveryDateTime,-20:dd/MM/yyyy HH:mm} " + $"${o.OrderTotal,-8:F2} " +$"{o.OrderStatus}"
                            
                        );
                    }
                }
            }

            //FEATURE 5 (Create a new order) 
            void CreateNewOrder(string dataDir, List<Restaurant> restaurants, Dictionary<string, Customer> customerByEmail)
            {
                Console.WriteLine();
                Console.WriteLine("Create New Order");
                Console.WriteLine("================");

                //Customer Email 
                string customerEmail = "";
                Customer customer = null;

                while (true)
                {
                    Console.Write("Enter Customer Email: ");
                    customerEmail = Console.ReadLine();

                    if (customerEmail == null) customerEmail = "";
                    customerEmail = customerEmail.Trim();

                    if (customerEmail == "")
                    {
                        Console.WriteLine("Email cannot be empty. Please try again.");
                        continue;
                    }

                    try
                    {
                        customer = customerByEmail[customerEmail];
                        break;
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine("Invalid customer email. Please try again.");
                    }
                }

                //Restaurant ID 
                string restaurantId = "";
                Restaurant restaurant = null;

                while (true)
                {
                    Console.Write("Enter Restaurant ID: ");
                    restaurantId = Console.ReadLine();

                    if (restaurantId == null) restaurantId = "";
                    restaurantId = restaurantId.Trim();

                    if (restaurantId == "")
                    {
                        Console.WriteLine("Restaurant ID cannot be empty. Please try again.");
                        continue;
                    }

                    restaurant = null;
                    for (int i = 0; i < restaurants.Count; i++)
                    {
                        if (restaurants[i].RestaurantId == restaurantId)
                        {
                            restaurant = restaurants[i];
                            break;
                        }
                    }

                    if (restaurant == null)
                    {
                        Console.WriteLine("Invalid restaurant ID. Please try again.");
                    }
                    else
                    {
                        break;
                    }
                }

                //Delivery Date 
                string deliveryDateStr = "";
                while (true)
                {
                    Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
                    deliveryDateStr = Console.ReadLine();

                    if (deliveryDateStr == null) deliveryDateStr = "";
                    deliveryDateStr = deliveryDateStr.Trim();

                    if (deliveryDateStr == "")
                    {
                        Console.WriteLine("Delivery date cannot be empty. Please try again.");
                        continue;
                    }

                    try
                    {
                        DateTime.ParseExact(deliveryDateStr, "dd/MM/yyyy", null);
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid delivery date format. Please use dd/mm/yyyy.");
                    }
                }

                //Delivery Time 
                string deliveryTimeStr = "";
                while (true)
                {
                    Console.Write("Enter Delivery Time (hh:mm): ");
                    deliveryTimeStr = Console.ReadLine();

                    if (deliveryTimeStr == null) deliveryTimeStr = "";
                    deliveryTimeStr = deliveryTimeStr.Trim();

                    if (deliveryTimeStr == "")
                    {
                        Console.WriteLine("Delivery time cannot be empty. Please try again.");
                        continue;
                    }

                    try
                    {
                        DateTime.ParseExact(deliveryTimeStr, "HH:mm", null);
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid delivery time format. Please use hh:mm (24-hour).");
                    }
                }

                //Delivery Address 
                string deliveryAddress = "";
                while (true)
                {
                    Console.Write("Enter Delivery Address: ");
                    deliveryAddress = Console.ReadLine();

                    if (deliveryAddress == null) deliveryAddress = "";
                    deliveryAddress = deliveryAddress.Trim();

                    if (deliveryAddress == "")
                    {
                        Console.WriteLine("Delivery address cannot be empty. Please try again.");
                    }
                    else
                    {
                        break;
                    }
                }

                // jy combine delivery date and time into DateTime 
                DateTime deliveryDateTime = DateTime.ParseExact(deliveryDateStr + " " + deliveryTimeStr, "dd/MM/yyyy HH:mm", null);

                //Display food items
                Console.WriteLine("Available Food Items:");
                List<FoodItem> availableItems = restaurant.MenuList[0].FoodItemList;

                for (int i = 0; i < availableItems.Count; i++)
                {
                    FoodItem fi = availableItems[i];
                    Console.WriteLine($"{i + 1}. {fi.ItemName} - ${fi.ItemPrice:F2}");
                }

                //Let user select items 
                List<OrderedFoodItem> chosenItems = new List<OrderedFoodItem>();

                while (true)
                {
                    int itemNo;

                    while (true)
                    {
                        Console.Write("Enter item number (0 to finish): ");
                        string itemInput = Console.ReadLine();

                        if (itemInput == null) itemInput = "";
                        itemInput = itemInput.Trim();

                        if (itemInput == "")
                        {
                            Console.WriteLine("Item number cannot be empty. Please try again.");
                            continue;
                        }

                        try
                        {
                            itemNo = Convert.ToInt32(itemInput);
                            break;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid item number. Please enter a number.");
                        }
                        catch (OverflowException)
                        {
                            Console.WriteLine("Invalid item number. Please enter a valid number.");
                        }
                    }

                    if (itemNo == 0)
                    {
                        if (chosenItems.Count == 0)
                        {
                            Console.WriteLine("You must select at least one item.");
                            continue;
                        }
                        break;
                    }

                    if (itemNo < 1 || itemNo > availableItems.Count)
                    {
                        Console.WriteLine("Invalid item number. Please choose from the list.");
                        continue;
                    }

                    int qty;
                    while (true)
                    {
                        Console.Write("Enter quantity: ");
                        string qtyInput = Console.ReadLine();

                        if (qtyInput == null) qtyInput = "";
                        qtyInput = qtyInput.Trim();

                        if (qtyInput == "")
                        {
                            Console.WriteLine("Quantity cannot be empty. Please try again.");
                            continue;
                        }

                        try
                        {
                            qty = Convert.ToInt32(qtyInput);
                            if (qty <= 0)
                            {
                                Console.WriteLine("Quantity must be at least 1. Please try again.");
                                continue;
                            }
                            break;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid quantity. Please enter a number.");
                        }
                        catch (OverflowException)
                        {
                            Console.WriteLine("Invalid quantity. Please enter a valid number.");
                        }
                    }

                    FoodItem selected = availableItems[itemNo - 1];

                    OrderedFoodItem ofi = new OrderedFoodItem(selected.ItemName, selected.ItemDesc, selected.ItemPrice, "", qty);

                    chosenItems.Add(ofi);
                }

                //Special request (Y/N)
                string addReq = "";
                while (true)
                {
                    Console.Write("Add special request? [Y/N]: ");
                    addReq = Console.ReadLine();

                    if (addReq == null) addReq = "";
                    addReq = addReq.Trim().ToUpper();

                    if (addReq == "Y" || addReq == "N")
                    {
                        break;
                    }

                    Console.WriteLine("Invalid input. Please enter Y or N.");
                }

                if (addReq == "Y")
                {
                    Console.Write("Enter special request: ");
                    string request = Console.ReadLine();
                    if (request == null) request = "";
                    request = request.Trim();

                    for (int i = 0; i < chosenItems.Count; i++)
                    {
                        chosenItems[i].Customise = request;
                    }
                }

                //Calculate totals
                double foodSubtotal = 0;
                for (int i = 0; i < chosenItems.Count; i++)
                {
                    foodSubtotal += chosenItems[i].SubTotal;
                }

                double deliveryFee = 5.00;
                double finalTotal = foodSubtotal + deliveryFee;

                Console.WriteLine($"Order Total: ${foodSubtotal:F2} + ${deliveryFee:F2} (delivery) = ${finalTotal:F2}");

                //Proceed to payment (Y/N)
                string proceed = "";
                while (true)
                {
                    Console.Write("Proceed to payment? [Y/N]: ");
                    proceed = Console.ReadLine();

                    if (proceed == null) proceed = "";
                    proceed = proceed.Trim().ToUpper();

                    if (proceed == "Y" || proceed == "N")
                    {
                        break;
                    }

                    Console.WriteLine("Invalid input. Please enter Y or N.");
                }

                if (proceed == "N")
                {
                    return;
                }

                //Payment method validation
                Console.WriteLine("Payment method:");
                string paymentMethod = "";
                while (true)
                {
                    Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
                    paymentMethod = Console.ReadLine();

                    if (paymentMethod == null) paymentMethod = "";
                    paymentMethod = paymentMethod.Trim().ToUpper();

                    if (paymentMethod == "CC" || paymentMethod == "PP" || paymentMethod == "CD")
                    {
                        break;
                    }

                    Console.WriteLine("Invalid payment method. Please enter CC, PP or CD.");
                }

                //Assign new Order ID 
                int maxOrderId = 0;
                for (int i = 0; i < restaurants.Count; i++)
                {
                    Restaurant r = restaurants[i];
                    for (int j = 0; j < r.restaurantOrderList.Count; j++)
                    {
                        if (r.restaurantOrderList[j].OrderId > maxOrderId)
                        {
                            maxOrderId = r.restaurantOrderList[j].OrderId;
                        }
                    }
                }
                int newOrderId = maxOrderId + 1;

                //Create order
                Order newOrder = new Order(newOrderId, customer, restaurant, deliveryDateTime, deliveryAddress);

                for (int i = 0; i < chosenItems.Count; i++)
                {
                    newOrder.AddOrderedFoodItem(chosenItems[i]);
                }

                newOrder.OrderTotal = finalTotal;
                newOrder.OrderPaymentMethod = paymentMethod;
                newOrder.OrderStatus = "Pending";
                newOrder.OrderPaid = true;

                customer.AddOrder(newOrder);
                restaurant.restaurantOrderList.Add(newOrder);

                //Build Items string for CSV 
                string itemsStr = "";
                for (int i = 0; i < chosenItems.Count; i++)
                {
                    if (i > 0) itemsStr += "|";
                    itemsStr += chosenItems[i].ItemName + ", " + chosenItems[i].QtyOrdered;
                }
                itemsStr = "\"" + itemsStr + "\"";

                string ordersPath = Path.Combine(dataDir, "orders.csv");
                string createdDateTimeStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                string newLine = newOrderId + "," + customerEmail + "," + restaurantId + "," + deliveryDateStr + "," + deliveryTimeStr + "," + deliveryAddress + "," + createdDateTimeStr + "," + finalTotal.ToString("0.##") + "," + newOrder.OrderStatus + "," + itemsStr;

                File.AppendAllText(ordersPath, Environment.NewLine + newLine);

                Console.WriteLine($"Order {newOrderId} created successfully! Status: {newOrder.OrderStatus}");
            }

            //FEATURE 6 (Process an order) //jy NOTE, SAMPLE SHOWS THAT PROCESSING ORDERS GOES THROUGH ORDERS FOR EVERY CUSTOMER
            void ProcessOrder(List<Restaurant> restaurants, Stack<Order> refundStack)
            {
                Console.WriteLine();
                Console.WriteLine("Process Order");
                Console.WriteLine("=============");

                //Restaurant ID 
                string inputRestaurantId = "";
                Restaurant selectedRestaurant = null;

                while (true)
                {
                    Console.Write("Enter Restaurant ID: ");
                    inputRestaurantId = Console.ReadLine();

                    if (inputRestaurantId == null) inputRestaurantId = "";
                    inputRestaurantId = inputRestaurantId.Trim();

                    if (inputRestaurantId == "")
                    {
                        Console.WriteLine("Restaurant ID cannot be empty. Please try again.");
                        continue;
                    }

                    selectedRestaurant = null;
                    for (int i = 0; i < restaurants.Count; i++)
                    {
                        if (restaurants[i].RestaurantId == inputRestaurantId)
                        {
                            selectedRestaurant = restaurants[i];
                            break;
                        }
                    }

                    if (selectedRestaurant == null)
                    {
                        Console.WriteLine("Invalid restaurant ID. Please try again.");
                    }
                    else
                    {
                        break;
                    }
                }

                //If there are no orders,show message 
                if (selectedRestaurant.restaurantOrderList.Count == 0)
                {
                    Console.WriteLine("No orders to process for this restaurant.");
                    return;
                }

                foreach (Order o in selectedRestaurant.restaurantOrderList)
                {
                    Console.WriteLine($"Order {o.OrderId}:");
                    Console.WriteLine($"Customer: {o.Customer.CustomerName}");
                    Console.WriteLine("Ordered Items:");

                    for (int i = 0; i < o.OrderedFoodItemList.Count; i++)
                    {
                        OrderedFoodItem item = o.OrderedFoodItemList[i];
                        Console.WriteLine($"{i + 1}. {item.ItemName} - {item.QtyOrdered}");
                    }

                    Console.WriteLine($"Delivery date/time: {o.DeliveryDateTime:dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"Total Amount: ${o.OrderTotal:F2}");
                    Console.WriteLine($"Order Status: {o.OrderStatus}");

                    //Action 
                    string action = "";
                    while (true)
                    {
                        Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
                        action = Console.ReadLine();

                        if (action == null) action = "";
                        action = action.Trim().ToUpper();

                        if (action == "")
                        {
                            Console.WriteLine("Input cannot be empty. Please enter C, R, S or D.");
                            continue;
                        }

                        if (action == "C" || action == "R" || action == "S" || action == "D")
                        {
                            break;
                        }

                        Console.WriteLine("Invalid input. Please enter C, R, S or D.");
                    }

                    if (action == "C")
                    {
                        if (o.OrderStatus == "Pending")
                        {
                            o.OrderStatus = "Preparing";
                            Console.WriteLine($"Order {o.OrderId} confirmed. Status: {o.OrderStatus}");
                        }
                        else
                        {
                            Console.WriteLine($"Order {o.OrderId} cannot be confirmed. Status: {o.OrderStatus}");
                        }
                    }
                    else if (action == "R")
                    {
                        if (o.OrderStatus == "Pending")
                        {
                            o.OrderStatus = "Rejected";
                            refundStack.Push(o);
                            Console.WriteLine($"Order {o.OrderId} rejected. Refund required.");
                        }
                        else
                        {
                            Console.WriteLine($"Order {o.OrderId} cannot be rejected. Status: {o.OrderStatus}");
                        }
                    }
                    else if (action == "S")
                    {
                        if (o.OrderStatus == "Cancelled")
                        {
                            Console.WriteLine($"Order {o.OrderId} skipped. Status: {o.OrderStatus}");
                        }
                        else
                        {
                            Console.WriteLine($"Order {o.OrderId} cannot be skipped. Status: {o.OrderStatus}");
                        }
                    }
                    else if (action == "D")
                    {
                        if (o.OrderStatus == "Preparing")
                        {
                            o.OrderStatus = "Delivered";
                            Console.WriteLine($"Order {o.OrderId} delivered. Status: {o.OrderStatus}");
                        }
                        else
                        {
                            Console.WriteLine($"Order {o.OrderId} cannot be delivered. Status: {o.OrderStatus}");
                        }
                    }

                    Console.WriteLine();
                }
            }

            //FEATURE 7 (Modify an existing order) 
            void ModifyExistingOrder(Dictionary<string, Customer> customerEmail)
            {
                Console.WriteLine();
                Console.WriteLine("Modify Order");
                Console.WriteLine("============");


                string email = "";
                Customer customer = null;

                while (true)
                {   //jy start with customer email
                    Console.Write("Enter Customer Email: ");
                    email = Console.ReadLine();


                    if (email == null)
                    {
                        Console.WriteLine("Email cannot be empty. Please try again.");
                        continue;
                    }
                    email = email.Trim();

                    try
                    {
                        customer = customerEmail[email]; //jy access the customeremail list to find the specific customer
                        break;
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine("Invalid customer email. Please try again.");
                    }
                }

                //jy then u want to see that SPECIFIC customer's PENDING orders
                Console.WriteLine("Pending Orders:");
                int pendingCount = 0;
                for (int i = 0; i < customer.customerOrderList.Count; i++)
                {
                    if (customer.customerOrderList[i].OrderStatus == "Pending")
                    {
                        Console.WriteLine(customer.customerOrderList[i].OrderId); //jy print the pending OrderID
                        pendingCount++;
                    }
                }

                if (pendingCount == 0)
                {
                    Console.WriteLine("No pending orders to modify.");
                    return;
                }

                //Order ID 
                int orderId = 0;
                Order target = null;

                while (true)
                {
                    Console.Write("Enter Order ID: ");
                    string orderInput = Console.ReadLine();


                    if (orderInput == null)
                    {
                        Console.WriteLine("Order ID cannot be empty. Please try again.");
                        continue;
                    }
                    orderInput = orderInput.Trim();


                    try
                    {
                        orderId = Convert.ToInt32(orderInput);//jy try to convert to int
                    }
                    catch (FormatException) //jy if fail to convert, do this
                    {
                        Console.WriteLine("Invalid Order ID format. Please enter a number.");
                        continue;
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Invalid Order ID. Please enter a valid number.");
                        continue;
                    }

                    target = null; //jy so the customer can hv many pending orders right, this target is the specific order i am looking at
                    for (int i = 0; i < customer.customerOrderList.Count; i++)
                    {
                        if (customer.customerOrderList[i].OrderId == orderId) //jy if the customer's order's orderID matches the orderID we choose,
                        {
                            target = customer.customerOrderList[i]; //jy that is my target
                            break;
                        }
                    }

                    if (target == null)
                    {
                        Console.WriteLine("Invalid Order ID. Please try again.");
                        continue;
                    }

                    if (target.OrderStatus != "Pending") //jy incase customer types in an order that isn't pending, but the order is still there right, so u dont want to allow access to NON pending orders
                    {
                        Console.WriteLine("This order cannot be modified because it is not Pending.");
                        continue;
                    }

                    break;
                }

                //Display order details
                Console.WriteLine("Order Items:");
                for (int i = 0; i < target.OrderedFoodItemList.Count; i++)
                {
                    OrderedFoodItem item = target.OrderedFoodItemList[i];
                    Console.WriteLine($"{i + 1}. {item.ItemName} - {item.QtyOrdered}");
                }

                Console.WriteLine("Address:");
                Console.WriteLine(target.DeliveryAddress);

                Console.WriteLine("Delivery Date/Time:");
                Console.WriteLine($"{target.DeliveryDateTime.Day}/{target.DeliveryDateTime.Month}/{target.DeliveryDateTime.Year}, {target.DeliveryDateTime:HH:mm}");

                //Modification choice 
                int choice = 0;
                while (true)
                {
                    Console.Write("Modify: [1] Items [2] Address [3] Delivery Time: "); //assuming items can only be added
                    string choiceInput = Console.ReadLine();


                    if (choiceInput == null)
                    {
                        Console.WriteLine("Choice cannot be empty. Please try again.");
                        continue;
                    }
                    choiceInput = choiceInput.Trim();


                    try
                    {
                        choice = Convert.ToInt32(choiceInput);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid choice. Please enter 1, 2 or 3.");
                        continue;
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Invalid choice. Please enter 1, 2 or 3.");
                        continue;
                    }

                    if (choice == 1 || choice == 2 || choice == 3)
                    {
                        break;
                    }

                    Console.WriteLine("Invalid choice. Please enter 1, 2 or 3.");
                }
                //MODIFY DELIVERY TIME
                if (choice == 3)
                {

                    string newTimeStr = ""; // jy new delivery time variable
                    while (true)
                    {
                        Console.Write("Enter new Delivery Time (hh:mm): ");
                        newTimeStr = Console.ReadLine();


                        if (newTimeStr == null)
                        {
                            Console.WriteLine("Delivery time cannot be empty. Please try again.");
                            continue;
                        }
                        newTimeStr = newTimeStr.Trim();

                        try
                        {
                            DateTime.ParseExact(newTimeStr, "HH:mm", null); //jy parseExact means must parse the exact time
                            break;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid delivery time format. Please use hh:mm (24-hour).");
                        }
                    }

                    DateTime oldDT = target.DeliveryDateTime;
                    DateTime newDT = DateTime.ParseExact(oldDT.ToString("dd/MM/yyyy") + " " + newTimeStr, "dd/MM/yyyy HH:mm", null);
                    target.DeliveryDateTime = newDT; //jy set order's deliverydatetime to my new date time

                    Console.WriteLine($"Order {target.OrderId} updated. New Delivery Time: {target.DeliveryDateTime:HH:mm}");
                }

                //MODIFY ADDRESS
                else if (choice == 2)
                {
                    //New address 
                    string newAddress = "";
                    while (true)
                    {
                        Console.Write("Enter new Delivery Address: ");
                        newAddress = Console.ReadLine();


                        if (newAddress == null)
                        {
                            Console.WriteLine("Delivery address cannot be empty. Please try again.");
                        }
                        else
                        {
                            break;
                        }
                        newAddress = newAddress.Trim();
                    }

                    target.DeliveryAddress = newAddress;
                    Console.WriteLine($"Order {target.OrderId} updated. New Delivery Address: {target.DeliveryAddress}");
                }
                //MODIFY ITEMS, ASSUMING U CAN ONLY ADD ITEMS FOR ORDERS THAT ARE PENDING
                else if (choice == 1)
                {
                    //jy create a list to put the old items inside
                    List<OrderedFoodItem> oldItems = new List<OrderedFoodItem>();
                    for (int i = 0; i < target.OrderedFoodItemList.Count; i++)
                    {
                        oldItems.Add(target.OrderedFoodItemList[i]);
                    }
                    double oldTotal = target.OrderTotal;

                    Console.WriteLine("Available Food Items:");
                    List<FoodItem> menuItems = target.Restaurant.MenuList[0].FoodItemList;

                    for (int i = 0; i < menuItems.Count; i++) //jy loop through list of fooditems and print them
                    {
                        Console.WriteLine($"{i + 1}. {menuItems[i].ItemName} - ${menuItems[i].ItemPrice:F2}");
                    }

                    // Add ON TOP of existing items (do not clear the list)
                    while (true)
                    {
                        int itemNumber;

                        while (true)
                        {
                            Console.Write("Enter item number (0 to finish): ");
                            string itemInput = Console.ReadLine();


                            if (itemInput == null)
                            {
                                Console.WriteLine("Item number cannot be empty. Please try again.");
                                continue;
                            }
                            itemInput = itemInput.Trim();

                            try
                            {
                                itemNumber = Convert.ToInt32(itemInput);
                                break;
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Invalid item number. Please enter a number.");
                            }
                            catch (OverflowException)
                            {
                                Console.WriteLine("Invalid item number. Please enter a valid number.");
                            }
                        }

                        if (itemNumber == 0)
                        {
                            // keep as-is; order already has existing items anyway
                            break;
                        }

                        if (itemNumber < 1 || itemNumber > menuItems.Count) //jy both negative numbers and numbers outside of range are invalid
                        {
                            Console.WriteLine("Invalid item number. Please choose from the list.");
                            continue;
                        }

                        int qty;
                        while (true)
                        {
                            Console.Write("Enter quantity: ");
                            string qtyInput = Console.ReadLine();

                            if (qtyInput == null)
                            {
                                Console.WriteLine("Quantity cannot be empty. Please try again.");
                                continue;
                            }
                            qtyInput = qtyInput.Trim();

                            try
                            {
                                qty = Convert.ToInt32(qtyInput);
                                if (qty <= 0)
                                {
                                    Console.WriteLine("Quantity must be at least 1. Please try again.");
                                    continue;
                                }
                                break;
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Invalid quantity. Please enter a number.");
                            }
                            catch (OverflowException)
                            {
                                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                            }
                        }
                        //jy after correct food item input,
                        FoodItem foodItemselected = menuItems[itemNumber - 1];

                        // jy CASE 1: if item already exists in ur current items, increase quantity only
                        bool found = false;
                        for (int i = 0; i < target.OrderedFoodItemList.Count; i++)
                        {
                            if (target.OrderedFoodItemList[i].ItemName == foodItemselected.ItemName)
                            {
                                target.OrderedFoodItemList[i].QtyOrdered += qty;
                                target.OrderedFoodItemList[i].ItemPrice = foodItemselected.ItemPrice;
                                target.OrderedFoodItemList[i].ItemDesc = foodItemselected.ItemDesc;
                                target.OrderedFoodItemList[i].CalculateSubtotal();
                                found = true; //jy true means it is in my current items
                                break;
                            }
                        }
                        //jy CASE 2: if item does not exist in ur current items, its a new item ure adding
                        if (!found)
                        {
                            OrderedFoodItem newFooditem = new OrderedFoodItem(foodItemselected.ItemName, foodItemselected.ItemDesc, foodItemselected.ItemPrice, "", qty);

                            target.AddOrderedFoodItem(newFooditem);
                        }
                    }

                    //jy recalculate total
                    double foodSubtotal = 0;
                    for (int i = 0; i < target.OrderedFoodItemList.Count; i++)
                    {
                        foodSubtotal += target.OrderedFoodItemList[i].SubTotal;
                    }

                    double deliveryFee = 5.00;
                    double newTotal = foodSubtotal + deliveryFee;

                    if (newTotal > oldTotal) //jy incase they didnt add anything, need this if statement
                    {
                        Console.WriteLine($"New Order Total: ${foodSubtotal:F2} + ${deliveryFee:F2} (delivery) = ${newTotal:F2}");

                        //Pay (Y/N)
                        string pay = "";
                        while (true)
                        {
                            Console.Write("Total increased. Proceed to payment? [Y/N]: ");
                            pay = Console.ReadLine();

                            if (pay == null) pay = "";
                            pay = pay.Trim().ToUpper();

                            if (pay == "Y" || pay == "N")
                            {
                                break;
                            }

                            Console.WriteLine("Invalid input. Please enter Y or N.");
                        }

                        if (pay == "N")
                        {
                            target.OrderedFoodItemList = oldItems;
                            target.OrderTotal = oldTotal;
                            Console.WriteLine($"Order {target.OrderId} not updated. Changes cancelled.");
                            return;
                        }

                        //Payment method validation
                        string method = "";
                        while (true)
                        {
                            Console.WriteLine("Payment method:");
                            Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
                            method = Console.ReadLine();

                            if (method == null) method = "";
                            method = method.Trim().ToUpper();

                            if (method == "CC" || method == "PP" || method == "CD")
                            {
                                break;
                            }

                            Console.WriteLine("Invalid payment method. Please enter CC, PP or CD.");
                        }

                        target.OrderPaymentMethod = method;
                        target.OrderPaid = true;
                        target.OrderTotal = newTotal;

                        Console.WriteLine($"Order {target.OrderId} updated. New Total: ${target.OrderTotal:F2}");
                    }
                    else
                    {
                        target.OrderTotal = newTotal;
                        Console.WriteLine($"Order {target.OrderId} updated. New Total: ${target.OrderTotal:F2}");
                    }
                }


            }

            //FEATURE 8 (Delete an existing order) //jy just means cancel, do not remove the order object from list, only pending can be cancelled
            void DeleteExistingOrder(Dictionary<string, Customer> customerByEmail, Stack<Order> refundStack)
            {
                Console.WriteLine();
                Console.WriteLine("Delete Order");
                Console.WriteLine("============");

                //email
                string email = "";
                Customer customer = null;

                while (true)
                {
                    Console.Write("Enter Customer Email: ");
                    email = Console.ReadLine();

                    if (email == null) email = "";
                    email = email.Trim();

                    if (email == "")
                    {
                        Console.WriteLine("Email cannot be empty. Please try again.");
                        continue;
                    }

                    try
                    {
                        customer = customerByEmail[email];
                        break;
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine("Invalid customer email. Please try again.");
                    }
                }

                //Display pending orders
                Console.WriteLine("Pending Orders:");
                int pendingCount = 0;
                for (int i = 0; i < customer.customerOrderList.Count; i++)
                {
                    if (customer.customerOrderList[i].OrderStatus == "Pending")
                    {
                        Console.WriteLine(customer.customerOrderList[i].OrderId);
                        pendingCount++;
                    }
                }

                if (pendingCount == 0)
                {
                    Console.WriteLine("No pending orders to cancel.");
                    return;
                }

                //Order ID 
                int orderId = 0;
                Order target = null;

                while (true)
                {
                    Console.Write("Enter Order ID: ");
                    string orderInput = Console.ReadLine();

                    if (orderInput == null) orderInput = "";
                    orderInput = orderInput.Trim();

                    if (orderInput == "")
                    {
                        Console.WriteLine("Order ID cannot be empty. Please try again.");
                        continue;
                    }

                    try
                    {
                        orderId = Convert.ToInt32(orderInput);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid Order ID format. Please enter a number.");
                        continue;
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Invalid Order ID. Please enter a valid number.");
                        continue;
                    }

                    target = null;
                    for (int i = 0; i < customer.customerOrderList.Count; i++)
                    {
                        if (customer.customerOrderList[i].OrderId == orderId)
                        {
                            target = customer.customerOrderList[i];
                            break;
                        }
                    }

                    if (target == null)
                    {
                        Console.WriteLine("Invalid Order ID. Please try again.");
                        continue;
                    }

                    if (target.OrderStatus != "Pending")
                    {
                        Console.WriteLine("This order cannot be cancelled because it is not Pending.");
                        continue;
                    }

                    break;
                }

                //Display sample information
                Console.WriteLine($"Customer: {target.Customer.CustomerName}");
                Console.WriteLine("Ordered Items:");
                for (int i = 0; i < target.OrderedFoodItemList.Count; i++)
                {
                    OrderedFoodItem item = target.OrderedFoodItemList[i];
                    Console.WriteLine($"{i + 1}. {item.ItemName} - {item.QtyOrdered}");
                }
                Console.WriteLine($"Delivery date/time: {target.DeliveryDateTime:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Total Amount: ${target.OrderTotal:F2}");
                Console.WriteLine($"Order Status: {target.OrderStatus}");

                //Confirm deletion (Y/N)
                string confirm = "";
                while (true)
                {
                    Console.Write("Confirm deletion? [Y/N]: ");
                    confirm = Console.ReadLine();

                    if (confirm == null) confirm = "";
                    confirm = confirm.Trim().ToUpper();

                    if (confirm == "")
                    {
                        Console.WriteLine("Input cannot be empty. Please enter Y or N.");
                        continue;
                    }

                    if (confirm == "Y" || confirm == "N")
                    {
                        break;
                    }

                    Console.WriteLine("Invalid input. Please enter Y or N.");
                }

                if (confirm == "Y")
                {
                    target.OrderStatus = "Cancelled";
                    refundStack.Push(target);

                    Console.WriteLine($"Order {target.OrderId} cancelled. Refund of ${target.OrderTotal:F2} processed.");
                }
                else // confirm == "N"
                {
                    Console.WriteLine($"Order {target.OrderId} not cancelled.");
                }
            }


            //ADVANCED FEATURE A (Bulk processing of unprocessed orders)
            void BulkProcessOrders(List<Restaurant> restaurants, Stack<Order> refundStack)
            {
                Console.WriteLine();
                Console.WriteLine("Bulk Process Orders");
                Console.WriteLine("===================");
                Console.WriteLine();

                DateTime now = DateTime.Now;

                //Count pending orders
                int totalPending = 0;
                foreach (Restaurant restaurant in restaurants)
                {
                    foreach (Order order in restaurant.restaurantOrderList)
                    {
                        if (order.OrderStatus == "Pending")
                        {
                            totalPending++;
                        }
                    }
                }

                Console.WriteLine($"Total Pending Orders: {totalPending}");
                Console.WriteLine();

                //Process each pending order
                int processed = 0;
                int preparing = 0;
                int rejected = 0;
                int totalOrders = 0;

                //Count total orders for percentage calculation
                foreach (Restaurant restaurant in restaurants)
                {
                    totalOrders += restaurant.restaurantOrderList.Count;
                }

                foreach (Restaurant restaurant in restaurants)
                {
                    foreach (Order order in restaurant.restaurantOrderList)
                    {
                        if (order.OrderStatus == "Pending")
                        {
                            //time difference
                            TimeSpan timeDiff = order.DeliveryDateTime - now;

                            //Check if delivery time less than 1 hour
                            if (timeDiff.TotalHours < 1)
                            {
                                //Reject order
                                order.OrderStatus = "Rejected";
                                refundStack.Push(order);
                                rejected++;
                            }
                            else
                            {
                                //Set as preparing
                                order.OrderStatus = "Preparing";
                                preparing++;
                            }
                            processed++;
                        }
                    }
                }

                //Display summary 
                Console.WriteLine($"Orders Processed: {processed}");
                Console.WriteLine($"Preparing: {preparing}");
                Console.WriteLine($"Rejected: {rejected}");

                if (totalOrders > 0)
                {
                    double percentage = (double)processed / totalOrders * 100;
                    Console.WriteLine($"Percentage of automatically processed orders: {percentage:F2}%");
                }
                else
                {
                    Console.WriteLine("Percentage of automatically processed orders: 0.00%");
                }
            }
            //ADVANCED FEATURE B (Display total order amount) 
            void DisplayTotalOrderAmount(List<Restaurant> restaurants, Stack<Order> refundStack)
            {
                Console.WriteLine();
                Console.WriteLine("Display Total Order Amount");
                Console.WriteLine("==========================");
                Console.WriteLine();

                double grandTotalOrderAmount = 0;
                double grandTotalRefunds = 0;
                double grandTotalGruberooCommission = 0;
                double grandTotalDeliveryFees = 0;

                foreach (Restaurant restaurant in restaurants)
                {
                    double restaurantOrderAmount = 0;
                    double restaurantRefunds = 0;
                    double restaurantGruberooCommission = 0;
                    double restaurantDeliveryFees = 0;

                    Console.WriteLine($"Restaurant: {restaurant.RestaurantName}");
                    Console.WriteLine("----------------------------------------");

                    //Delivered orders, total order amount and comission
                    foreach (Order order in restaurant.restaurantOrderList)
                    {
                        if (order.OrderStatus == "Delivered")
                        {
                            //OrderTotal includes delivery fee, so subtract $5 to get food subtotal
                            double foodSubtotal = order.OrderTotal - 5.00;
                            restaurantOrderAmount += foodSubtotal;

                            //Gruberoo earns 30% commission on food subtotal...
                            restaurantGruberooCommission += foodSubtotal * 0.30;

                            
                            restaurantDeliveryFees += 5.00;
                        }
                    }

                    Console.WriteLine($"Total Order Amount (Delivered): ${restaurantOrderAmount:F2}");
                    Console.WriteLine($"Gruberoo Commission (30%): ${restaurantGruberooCommission:F2}");

                    //jy refunded orders is for cancelled and rejected(use refundStack)
                    foreach (Order refunded in refundStack)
                    {
                        if (refunded.Restaurant != null && //dont subtract the refunds...
                            refunded.Restaurant.RestaurantId == restaurant.RestaurantId &&
                            (refunded.OrderStatus == "Cancelled" || refunded.OrderStatus == "Rejected"))
                        {
                            restaurantRefunds += refunded.OrderTotal;
                        }
                    }

                    Console.WriteLine($"Total Refunds (Cancelled/Rejected): ${restaurantRefunds:F2}");
                    Console.WriteLine();

                    grandTotalOrderAmount += restaurantOrderAmount;
                    grandTotalRefunds += restaurantRefunds;
                    grandTotalGruberooCommission += restaurantGruberooCommission;
                    grandTotalDeliveryFees += restaurantDeliveryFees;
                }

                Console.WriteLine("========================================");
                Console.WriteLine($"Total Order Amount (All Restaurants): ${grandTotalOrderAmount:F2}");
                Console.WriteLine($"Total Refunds (All Restaurants): ${grandTotalRefunds:F2}");
                Console.WriteLine($"Final Amount Gruberoo Earns (30% Commission): ${grandTotalGruberooCommission:F2}");
            }


        }
    }
}

