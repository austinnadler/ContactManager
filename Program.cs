using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AddressBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            List<Contact> contacts = new List<Contact>();
            try
            {
                contacts = ReadContactsFromFile();
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("No contacts file found...");
            }
            Start(ref contacts);
            WriteContactsToFile(ref contacts);
        } // end Main()

        public static void Start(ref List<Contact> contacts) 
        {
            Console.WriteLine("\n*** Address Book ***");
            bool validChoice = false;
            int choice;
            do 
            {
                Console.WriteLine(  "\n1. View Contacts" +                
                                    "\n2. Create New Contact" + 
                                    "\n3. Delete Contact" + 
                                    "\n4. Edit Contact" + 
                                    "\n5. Quit\n");
                Console.Write("Enter the number of your choice: ");
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                    if(choice > 5 || choice < 1)
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
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid choice, try again");
                }
            } while(!validChoice);

            
        } // end Start()

        public static void ListAllContacts(ref List<Contact> contacts) 
        {
            if(contacts.Count < 1)
            {
                Console.WriteLine("You have not created any contacts yet.");
            }
            else
            {
                Contact contact;
                Console.WriteLine("------------------------------------------------------------------------------------");
                for(int i = 0; i < contacts.Count; i++) // Regular for loop because I want to show indecies for selection
                {
                    contact = contacts[i];
                    Console.Write(i + ". " + contact.ToDisplayString());
                    if(contact is InstructorContact ic)
                    {
                        Console.Write(" Office: " + ic.office);
                    }
                    Console.WriteLine("\n------------------------------------------------------------------------------------");
                }
            }
        } // end ListAllContacts()

        
        public static void CreateNewContact(ref List<Contact> contacts)
        { // Get information on the new contact and add them to the List<>. Changes are saved when the program exits.
            string first, last, office, isInstr = "";
            Contact contact;

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
        {
            contacts.RemoveAt(GetValidIndex(ref contacts, "delete"));           
        } // end DeleteContact()
        
        public static void EditContact(ref List<Contact> contacts)
        {
            bool validChoice = false;
            int choice;
            int maxFieldIndex; // needed in place of a boolean so that the choice can be validated based on the number of possible choices for that contact type
            Contact contact = contacts[GetValidIndex(ref contacts, "edit")];

            if(contact is InstructorContact ic)
            {
                Console.WriteLine(  "\n0. First name: " + ic.first + 
                                    "\n1. Last name: " + ic.last + 
                                    "\n2. Phone #: " + ic.phone + 
                                    "\n3. Email: " + ic.email + 
                                    "\n4. Office: " + ic.office);
                maxFieldIndex = 4;
            }
            else
            {
                Console.WriteLine(  "\n0. First name: " + contact.first +
                                    "\n1. Last name: " + contact.last + 
                                    "\n2. Phone #: " + contact.phone + 
                                    "\n3. Email: " + contact.email);
                maxFieldIndex = 3;
            }

            do
            {
                Console.Write("\nWhat field of this contact do you want to edit?: ");
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                    if(choice < 0 || choice > maxFieldIndex)
                    {
                        Console.WriteLine("Invalid choice. Try again.");
                    }
                    else
                    {
                        validChoice = true;
                    }

                    if(maxFieldIndex == 3)
                    {
                        EditBasicContact(contact, ref choice);
                    }
                    else // if(maxFieldIndex == 4)
                    {
                        EditInstructorContact(new InstructorContact(contact), ref choice);
                    }
                }
                catch(FormatException)
                {
                    Console.WriteLine("Invalid choice. Try again.");
                }
            } while(!validChoice);
        } // end EditContact()

        public static void EditBasicContact(Contact contact, ref int choice)
        {
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
                    Console.Write("Enter the new phone number: ");
                    contact.phone = GetValidPhoneNumber(true);
                    break;
                case 3:
                    Console.Write("Enter the new email address: ");
                    contact.email = GetValidEmail(true);
                    break;
                default:
                    break;
            }
        } // end EditBasicContact()

        public static void EditInstructorContact(InstructorContact contact, ref int choice)
        {
            if (!(contact is null))
            {
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
                        Console.Write("Enter the new phone number: ");
                        contact.phone = GetValidPhoneNumber(true);
                        break;
                    case 3:
                        Console.Write("Enter the new email address: ");
                        contact.email = GetValidEmail(true);
                        break;
                    case 4:
                        Console.Write("Enter the new office location: ");
                        contact.office = Console.ReadLine();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(contact));
            }
        } // EditInstructorContact()

        /*------------------------------ File IO ------------------------------*/
        public static List<Contact> ReadContactsFromFile()
        {
            {
                return File.ReadAllLines("contacts.csv").Select(v => ReadLineFromFile(v)).ToList();
            }
        } // ReadContactsFromFile()

        public static Contact ReadLineFromFile(string csvLine)
        {
            string[] values = csvLine.Split(',');
            string type = values[0];
            if(type == "b")
            {
                return new Contact(values[1], values[2], values[3], values[4]);
            }
            else // if (type == "i")
            {
                return new InstructorContact(values[1], values[2], values[3], values[4], values[5]);
            }
        } // ReadLineFromFile()

        public static void WriteContactsToFile(ref List<Contact> contacts)
        {
            string list = "";
            foreach(Contact contact in contacts)
            {
                string type = "b,"; // Initialize type to basic for every contact.
                if(contact is InstructorContact)
                {
                    type = "i,";
                }
                list +=  type + contact.ToStringCSV() + "\n";
            }
            System.IO.File.WriteAllText("contacts.csv", list);
        } // WriteContactsToFile()

        /*------------------------------ End File IO ------------------------------*/

        /*------------------------------ Utilities ------------------------------*/

        public static int GetValidIndex(ref List<Contact> contacts, string action)
        {// Helper for DeleteContact & EditContact. Returns a valid index provided by user.
            bool validIndex = false;
            bool isInt = false;
            int index = -1;
            ListAllContacts(ref contacts);
            do
            {
                Console.Write("\nEnter the index of the contact you want to " + action + ": ");
                try
                {
                    index = Convert.ToInt32(Console.ReadLine());
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
                catch(FormatException)
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
                if(!IsNum(phone) || phone.Length != 10)
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
                if(!email.Contains("@") || !email.Contains("."))
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

        public static bool IsNum(string str)
        {
            return str.All(char.IsNumber);
        } // end IsNum()

        
        /*------------------------------ End Utilities ------------------------------*/
    }
}