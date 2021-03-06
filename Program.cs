﻿using System;
using System.IO;                    // File IO
using System.Collections.Generic;   // List<T>
using System.Linq;                  // LINQ queries.
using System.Globalization;         // CultureInfo class for case insensitive comparison in LINQ queries.

namespace AddressBook
{ // Test
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            List<Contact> contacts = new List<Contact>();
            try
            {
                contacts = ReadContactsFromFile();
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("No contacts file found...");
            }
            ListAllContacts(ref contacts);
            Start(ref contacts);
            WriteContactsToFile(ref contacts);
            Console.ForegroundColor = ConsoleColor.White;
        } // end Main()

        public static void Start(ref List<Contact> contacts) 
        { // Start the program. This method is outside of Main() so that it can be called at the end of a process.
            Console.WriteLine("\n*** Address Book ***");
            bool validChoice = false;
            string choiceStr;
            int choice;
            do 
            {
                Console.WriteLine(  "\n1. View Contacts" +                
                                    "\n2. Create New Contact" + 
                                    "\n3. Delete Contact" + 
                                    "\n4. Edit Contact" + 
                                    "\n5. Search Contacts" +
                                    "\n6. Quit\n");
                Console.Write("Enter the number of your choice: ");
                choiceStr = Console.ReadLine();
                if(IsNumeric(choiceStr))
                {
                    choice = Convert.ToInt32(choiceStr);
                    if(choice < 1 || choice > 6)
                    {
                        Console.WriteLine("Invalid choice, try again");
                    }
                    else
                    {
                        validChoice = true;
                    }

                    switch(choice) 
                    {
                        case 1:
                            ListAllContacts(ref contacts);
                            Start(ref contacts);
                            break;
                        case 2:
                            CreateNewContact(ref contacts);
                            ListAllContacts(ref contacts);
                            Start(ref contacts);
                            break;
                        case 3:
                            DeleteContact(ref contacts);
                            ListAllContacts(ref contacts);
                            Start(ref contacts);
                            break;
                        case 4:
                            EditContact(ref contacts);
                            Start(ref contacts);
                            break;
                        case 5:
                            SearchContacts(ref contacts);
                            Start(ref contacts);
                            break;
                        case 6:
                            break;
                        default:
                            break;
                    }
                }
                else 
                {
                    Console.WriteLine("Invalid Choice. Try again.");
                }
            } while(!validChoice);

            
        } // end Start()

        public static void ListAllContacts(ref List<Contact> contacts) 
        { // Print out all contacts using their ToDisplayString().
            contacts.Sort((x, y) => x.last.CompareTo(y.last));
            if(contacts.Count < 1)
            {
                Console.WriteLine("You have not created any contacts yet4.");
            }
            else
            {
                Console.WriteLine("\n------------------------------------------------------------------------------------");
                for(int i = 0; i < contacts.Count; i++) // Regular for loop because I want to show indecies for selection
                {
                    Console.Write(i + ". " + contacts[i].ToDisplayString());
                    Console.WriteLine("\n------------------------------------------------------------------------------------");
                }
            }
        } // end ListAllContacts()

        
        public static void CreateNewContact(ref List<Contact> contacts)
        { // Get information on the new contact and add them to the List<>. Changes are saved when the program exits.
            string first, last, office, isInstr = "";
            Contact contact;

            Console.WriteLine("\n*** Creating New Contact ***");
            Console.Write("\nFirst name: ");
            first = Console.ReadLine();
            Console.Write("Last name: ");
            last = Console.ReadLine();

            do
            {
                Console.Write("Is instructor? (y/n): ");
                isInstr = Console.ReadLine().ToLower();
            } while(isInstr != "y" && isInstr != "n");

            if(isInstr == "n")
            {
                contact = new Contact(first, last);
            }
            else // if(isInstr == "y")
            {
                contact = new InstructorContact(first, last);
            }

            contact.phone = GetValidPhoneNumber(false);

            contact.email = GetValidEmail(false);
                
            if(contact is InstructorContact ic)
            {
                Console.Write("Office location: ");
                office = Console.ReadLine();
                ic.office = office;
                contacts.Add(ic);
            }
            else
            {
                contacts.Add(contact);
            }
        } // end CreateNewContact()

        public static void DeleteContact(ref List<Contact> contacts)
        { // Delete a contact at a valid index.
            int contactIndex = GetValidIndex(ref contacts, "delete");

            if(contactIndex == -1) 
            {
                return;
            }
            contacts.RemoveAt(contactIndex);           
        } // end DeleteContact()
        
        public static void EditContact(ref List<Contact> contacts)
        { // Begin the process of editing a contact. Gets a valid index using GetValidIndex() and determines what kind of contact is being dealt with. It then passes the process to the appropriate method.
            bool validChoice = false;
            string choiceStr;
            int choice;
            int maxFieldIndex; // needed in place of a boolean so that the choice can be validated based on the number of possible choices for that contact type.
            int contactIndex = GetValidIndex(ref contacts, "edit");

            if(contactIndex == -1) 
            {
                return;
            }

            Contact contact = contacts[contactIndex];

            if(contact is InstructorContact ic)
            {
                Console.WriteLine(  "\n0. First name: " + ic.first + 
                                    "\n1. Last name: " + ic.last + 
                                    "\n2. Phone #: " + ic.phone + 
                                    "\n3. Email: " + ic.email + 
                                    "\n4. Office: " + ic.office +
                                    "\n-1. Cancel");
                maxFieldIndex = 4;
            }
            else
            {
                Console.WriteLine(  "\n0. First name: " + contact.first +
                                    "\n1. Last name: " + contact.last + 
                                    "\n2. Phone #: " + contact.phone + 
                                    "\n3. Email: " + contact.email +
                                    "\n-1. Cancel");
                maxFieldIndex = 3;
            }

            do
            {
                Console.Write("\nWhat field of this contact do you want to edit?: ");
                choiceStr = Console.ReadLine();
                if(IsNumeric(choiceStr))
                {
                    choice = Convert.ToInt32(choiceStr);

                    if(choice == -1) 
                    {
                        return;
                    }

                    if(choice < 0 || choice > maxFieldIndex)
                    {
                        Console.WriteLine("Invalid choice. Try again.");
                    }
                    else
                    {
                        validChoice = true;
                        if(maxFieldIndex == 3)
                        {
                            EditBasicContact(contact, ref choice);
                        }
                        else // if(maxFieldIndex == 4)
                        {
                            EditInstructorContact(new InstructorContact(contact), ref choice);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Try again.");
                }
            } while(!validChoice);
        } // end EditContact()

        public static void EditBasicContact(Contact contact, ref int choice)
        { // Allow editing of an Contact. Appropriate fields are error checked, while others are not.
            switch(choice)
            {
                case 0:
                    Console.Write("Enter the new first name: ");
                    contact.first = Console.ReadLine();
                    break;
                case 1:
                    Console.Write("Enter the new last name: ");
                    contact.last = Console.ReadLine();
                    break;
                case 2: 
                    contact.phone = GetValidPhoneNumber(true);
                    break;
                case 3:
                    contact.email = GetValidEmail(true);
                    break;
                default:
                    break;
            }
        } // end EditBasicContact()

        public static void EditInstructorContact(InstructorContact contact, ref int choice)
        { // Allow editing of an InstructorContact. Appropriate fields are error checked, while others are not.
             switch (choice)
                {
                    case 0:
                        Console.Write("Enter the new first name: ");
                        contact.first = Console.ReadLine();
                        break;
                    case 1:
                        Console.Write("Enter the new last name: ");
                        contact.last = Console.ReadLine();
                        break;
                    case 2:
                        contact.phone = GetValidPhoneNumber(true);
                        break;
                    case 3:
                        contact.email = GetValidEmail(true);
                        break;
                    case 4:
                        Console.Write("Enter the new office location: ");
                        contact.office = Console.ReadLine();
                        break;
                    default:
                        break;
                }
        } // end EditInstructorContact()

        public static void SearchContacts(ref List<Contact> contacts)
        { // Search for contacts based on users choice of field.
            bool found = false;
            string searchAgain;
            string choiceStr;
            int choice;
            do
            {
                Console.Write(  "\nSearch Options" +
                                "\n1. Search by name (first or last)" +
                                "\n2. Search by phone number" +
                                "\n3. Search by email address" + 
                                "\n4. Cancel" +
                                "\n\nEnter your choice: ");
                choiceStr = Console.ReadLine();
                if(IsNumeric(choiceStr))
                {
                    choice = Convert.ToInt32(choiceStr);
                    if(choice > 0 && choice < 5)
                    {
                        switch(choice)
                        {
                            case 1:
                                SearchByName(ref contacts);
                                break;
                            case 2:
                                SearchByPhone(ref contacts);
                                break;
                            case 3: 
                                SearchByEmail(ref contacts);
                                break;
                            case 4:
                                return;
                            default:
                                break;
                        }

                        do
                        {
                            Console.Write("\nDo you want to search again? (y/n): ");
                            searchAgain = Console.ReadLine();
                            if(searchAgain == "n")
                            {
                                found = true;
                            }
                        } while(searchAgain != "y" && searchAgain != "n");
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Try again.");
                }
            } while(!found);
        } // end SearchContacts()

        public static void SearchByName(ref List<Contact> contacts)
        { // Display the results of QueryByName() to show all contacts whose first or last name contains the provided input.
            Console.Write("\nEnter the name to search by: ");
            string name = Console.ReadLine();
            List<Contact> matches = QueryByName(ref contacts, name);
            Console.WriteLine("\n------------------------------------------------------------------------------------");
            if(matches.Count > 0)
            {
                foreach(Contact match in matches)
                {
                    Console.WriteLine(match.ToDisplayString());
                    Console.WriteLine("------------------------------------------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("No contacts first or last name contains " + name);
                Console.WriteLine("------------------------------------------------------------------------------------\n");
            }
        } // end SearchByName()

        public static void SearchByPhone(ref List<Contact> contacts)
        { // Display the results of QueryByPhone() to show all contacts whose phone number contains the provided input.
            Console.Write("\nEnter the phone number to search by: ");
            string phone = Console.ReadLine();
            List<Contact> matches = QueryByPhone(ref contacts, phone);
            Console.WriteLine("\n------------------------------------------------------------------------------------");
            if(matches.Count > 0)
            {
                foreach(Contact contact in matches)
                {
                    Console.WriteLine(contact.ToDisplayString());
                    Console.WriteLine("------------------------------------------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("No contacts phone number contains " + phone);
                Console.WriteLine("------------------------------------------------------------------------------------");
            }
        } // end SearchByPhone

        public static void SearchByEmail(ref List<Contact> contacts)
        { // Display the results of QueryByEmail() to show all contacts whose email address contains the provided input.
            Console.Write("\nEnter the email to search by: ");
            string email = Console.ReadLine();
            List<Contact> matches = QueryByEmail(ref contacts, email);
            Console.WriteLine("\n------------------------------------------------------------------------------------");
            if(matches.Count > 0)
            {
                foreach(Contact contact in matches)
                {
                    Console.WriteLine(contact.ToDisplayString());
                    Console.WriteLine("------------------------------------------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("No contacts email address contains " + email);
                Console.WriteLine("------------------------------------------------------------------------------------");
            }
        }

        /*------------------------------ File IO ------------------------------*/
        public static List<Contact> ReadContactsFromFile()
        { // Read all contacts from contact.csv and passes each individual line (skipping the header line) to ReadLineFromFile() to be stored in an object. Uses LINQ Select() and Skip().
            {
                return File.ReadAllLines("contacts.csv").Select(v => ReadLineFromFile(v)).Skip(1).ToList();
            }
        } // end ReadContactsFromFile()

        public static Contact ReadLineFromFile(string csvLine)
        { // Helper for ReadContactsFromFile(). Reads a single line from contacts.csv and stores each of them in an object.
            string[] values = csvLine.Split(',');
            string type = values[0]; // First index that will be read from the file is the contact type.
            if(type == "b")
            { // Start reading actual values at index 1.
                return new Contact(values[1], values[2], values[3], values[4]);
            }
            else // if (type == "i")
            {
                return new InstructorContact(values[1], values[2], values[3], values[4], values[5]);
            }
        } // end ReadLineFromFile()

        public static void WriteContactsToFile(ref List<Contact> contacts)
        { // Write the entire List<> to contacts.csv. If one exists it's overwritten.
            string list = "Type,First Name,Last Name,Phone number,Email address,Office\n";
            foreach(Contact contact in contacts)
            {
                string type = "b,"; // Initialize type to basic for every contact.
                if(contact is InstructorContact)
                {
                    type = "i,";
                }
                list += type + contact.ToStringCSV() + "\n";
            }
            System.IO.File.WriteAllText("contacts.csv", list);
        } // end WriteContactsToFile()

        /*------------------------------ End File IO ------------------------------*/

        /*------------------------------- Utilities- ------------------------------*/

        public static int GetValidIndex(ref List<Contact> contacts, string action)
        { // Helper for DeleteContact & EditContact. Returns a valid index provided by user. Returns -1 if user wants to cancel.
            bool validIndex = false;
            bool isInt = false;
            string indexStr;
            int index = -1;
            ListAllContacts(ref contacts);
            do
            {
                Console.Write("\nEnter the index of the contact you want to " + action + " (or -1 to cancel): ");
                indexStr = Console.ReadLine();
                if(IsNumeric(indexStr)) 
                {
                    index = Convert.ToInt32(indexStr);

                    if(index == -1) 
                    {
                        return -1;
                    }

                    isInt = true;
                    if(isInt && index > -1 && index < contacts.Count)
                    {
                        validIndex = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid index. Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid index. Try again.");
                }
            } while(!validIndex);
            return index;
        } // end GetValidIndex()

        public static string GetValidPhoneNumber(bool updating)
        { // bool updating = true if changing, false if providing original
            string phone;
            bool validPhone = false;
            do
            {
                if(!updating)
                {
                    Console.Write("Phone number: ");
                }
                else
                {
                    Console.Write("Enter the new phone number: ");
                }
                phone = Console.ReadLine();
                if(!IsNumeric(phone) || phone.Length != 10)
                {
                    Console.WriteLine("Invalid phone number. Try again.");
                }
                else
                {
                    validPhone = true;
                }
            } while(!validPhone || phone.Length != 10);
            return phone;
        } // end GetValidPhoneNumber()
        public static string GetValidEmail(bool updating)
        { // bool updating = true if changing, false if providing original
            string email;
            bool validEmail = false;
            do
            {
                if(!updating)
                {
                    Console.Write("Email address: ");
                }
                else
                {
                    Console.Write("Enter the new email address: ");
                }
                email = Console.ReadLine();
                if(!email.Contains("@") || !email.Contains(".")) // Only validates that the email contains '@' and '.'. Makes testing easier
                {
                    Console.WriteLine("Invalid email address. Try again");
                }
                else 
                {
                    validEmail = true;
                }
            } while(!validEmail);
            return email;
        } // end GetValidEmail()

        public static bool IsNumeric(string str)
        { // Return true if the provided string is comprised of all integers, false otherwise. Used to avoid using stanard exceptions as much as possible.
            if(str == "-1") // For cancellation. No other negative values are used in this program so if the value provided is a negative other than -1, return false.
            {
                return true;
            } 
            else
            {
                return str.All(char.IsNumber);
            }
        } // end IsNumeric()
        /*------------------------------ End Utilities ------------------------------*/

        /*------------------------------ LINQ Searches ------------------------------*/
        // LINQ ref: Pro C# 7 Ch. 12
        public static List<Contact> QueryByName(ref List<Contact> contacts, string value)
        {   // Use LINQ to return a list of contacts whose first or last name contains the value.
            CultureInfo culture = new CultureInfo("en-US", false); 
            return  
            ( 
                from c in contacts 
                where 
                (
                    culture.CompareInfo.IndexOf(c.first, value, CompareOptions.IgnoreCase) >= 0 
                    || culture.CompareInfo.IndexOf(c.last, value, CompareOptions.IgnoreCase) >= 0
                )
                orderby c.last, c.first 
                select c
            ).ToList<Contact>();
        } // end QueryByName()

        public static List<Contact> QueryByPhone(ref List<Contact> contacts, string value)
        { // Use LINQ to return a list of contacts whose phone number contains the value
            return  
            ( 
                from c in contacts
                where c.phone.Contains(value)
                orderby c.last, c.first
                select c
            ).ToList<Contact>();
        } // end QueryByPhone()

        public static List<Contact> QueryByEmail(ref List<Contact> contacts, string value)
        { // Use LINQ to return a list of contacts whose email address contains the value
            CultureInfo culture = new CultureInfo("en-US", false);
            return  
            (
                from c in contacts
                where
                (
                    culture.CompareInfo.IndexOf(c.email, value, CompareOptions.IgnoreCase) >= 0
                )
                orderby c.last, c.first
                select c
            ).ToList<Contact>();
        } // end QueryByEmail()
        /*------------------------------ End LINQ Searches ------------------------------*/
    }
}