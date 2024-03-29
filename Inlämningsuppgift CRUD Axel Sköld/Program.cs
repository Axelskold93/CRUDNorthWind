﻿using Microsoft.Data.SqlClient;

namespace Inlämningsuppgift_CRUD_Axel_Sköld
{

    class Program
    {
        static void Main(string[] args)
        {
            
            ManageDatabase manageDataBase = new ManageDatabase();
            MainMenu(manageDataBase);

        }
        private static void MainMenu(ManageDatabase manageDataBase)
        {
            
            while (true)
            {
                Console.Clear();
                int option = ShowMenu("Var god välj:", new[]
                {
                    "Add Customer",
                    "Add Order",
                    "Delete Customer",
                    "Update Employee",
                    "Sales By Country",
                    "Add New Customer And Order",
                    "Exit"
                });
                if (option == 0)
                {
                    manageDataBase.AddCustomer();
                }
                else if (option == 1)
                {
                    manageDataBase.AddOrder();
                }
                else if (option == 2)
                {
                    manageDataBase.DeleteCustomer();
                }
                else if (option == 3)
                {
                    manageDataBase.UpdateEmployee();
                }
                else if (option == 4)
                {
                    manageDataBase.ShowCountrySales();
                }
                else if (option == 5)
                {
                    manageDataBase.AddOrderAndNewCustomer();
                }
                else if (option == 6)
                {
                    Console.WriteLine("Until next time!");
                    Environment.Exit(0);
                }
            }
        }
        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || !options.Any())
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }


    }
}