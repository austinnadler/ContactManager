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
            Console.WriteLine("\n*** Address Book ***\n");
            bool validChoice = false;
            int choice;

            do 
            {
                Console.WriteLine("1. View Contacts\n2. Create New Contact\n3. Delete Contact\n4. Edit Contact\n5. Quit\n");
                Console.Write("Enter the number of your choice: ");
                choice = Convert.ToInt32(Console.ReadLine());
                try
                {
                    if(choice > 5 || choice < 1)
                    {
                        Console.WriteLine("Invalid choice, try again");
                    }
                    else
                    {
                        validChoice = true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Invalid choice, try again");
                }
            } while(!validChoice);

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
        } // end Start()

        public static void ListAllContacts(ref List<Contact> contacts) 
        {
            if(contacts.Count < 1)
            {
                Console.WriteLine("You have not created any contacts yet.");
            }
            else
            {
                Console.WriteLine();
                for(int i = 0; i < contacts.Count; i++) // Regular for loop because I want to show indecies for selection
                {
                    Contact contact = contacts[i];
                    Console.Write(i + ". " + contact.first + " " + contact.last + " " + contact.phone + " " + contact.email);
                    if(contact is InstructorContact ic)
                    {
                        Console.Write(" Office: " + ic.office);
                    }
                    Console.WriteLine();
                }
            }
        } // end ListAllContacts()

        public static void CreateNewContact(ref List<Contact> contacts)
        {
            string first, last, office, isInstr = "";
            Contact c = new Contact();

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
                c = new Contact(first, last);
            }
            else // if(isInstr == "y")
            {
                c = new InstructorContact(first, last);
            }

            c.phone = GetValidPhoneNumber();

            c.email = GetValidEmail();
                
            if(c is InstructorContact ic)
            {
                Console.Write("Office location: ");
                office = Console.ReadLine();
                ic.office = office;
                contacts.Add(ic);
            }
            else
            {
                contacts.Add(c);
            }
        } // end CreateNewContact()
        
        public static void EditContact(ref List<Contact> contacts)
        {
            bool validChoice = false;
            int choice;
            int maxFieldIndex;
            Contact contact = contacts[GetValidIndex(ref contacts, "edit")];

            if(contact is InstructorContact ic)
            {
                Console.WriteLine("\n0. First name: " + ic.first + "\n1. Last name: " + ic.last + "\n2. Phone #: " + ic.phone + "\n3. Email: " + ic.email + "\n4. Office: " + ic.office);
                maxFieldIndex = 4;
            }
            else
            {
                Console.WriteLine("\n0. First name: " + contact.first + "\n1. Last name: " + contact.last + "\n2. Phone #: " + contact.phone + "\n3. Email: " + contact.email);
                maxFieldIndex = 3;
            }

            do
            {
                Console.Write("\nWhat field of this customer do you want to edit?: ");
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
                    contact.phone = GetValidPhoneNumber();
                    break;
                case 3:
                    Console.Write("Enter the new email address: ");
                    contact.email = GetValidEmail();
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
                        contact.phone = GetValidPhoneNumber();
                        break;
                    case 3:
                        Console.Write("Enter the new email address: ");
                        contact.email = GetValidEmail();
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
        }

        /*------------------------------ File IO ------------------------------*/
        public static List<Contact> ReadContactsFromFile()
        {
            {
                return File.ReadAllLines("contacts.csv").Select(v => ReadLineFromFile(v)).ToList();
            }
        }

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
        }

        public static void WriteContactsToFile(ref List<Contact> contacts)
        {
            string list = "";
            for(int i = 0; i < contacts.Count; i++)
            {
                string type = "b,"; // Initialize type to basic for every contact.
                if(contacts[i] is InstructorContact)
                {
                    type = "i,";
                }
                list +=  type + contacts[i].toStringCSV() + "\n";
            }
            System.IO.File.WriteAllText("contacts.csv", list);
        }
        /*------------------------------ End File IO ------------------------------*/

        /*------------------------------ Utilities ------------------------------*/

        // Helper for DeleteContact & EditContact. Returns a valid index provided by user.
        public static int GetValidIndex(ref List<Contact> contacts, string action)
        {
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
        }

        public static string GetValidPhoneNumber()
        {
            string phone;
            bool validPhone = false;
            do
            {
                Console.Write("Phone number: ");
                phone = Console.ReadLine();
                validPhone = int.TryParse(phone, out int n);
                if(!validPhone || phone.Length != 10)
                {
                    Console.WriteLine("Invalid phone number. Try again.");
                }
                else
                {
                    validPhone = true;
                }
            } while(!validPhone || phone.Length != 10);
            return phone;
        }

        public static string GetValidEmail()
        {
            string email;
            bool validEmail = false;
            do
            {
                Console.Write("Email address: ");
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
        }

        public static void DeleteContact(ref List<Contact> contacts)
        {
            contacts.RemoveAt(GetValidIndex(ref contacts, "delete"));           
        }
        /*------------------------------ End Utilities ------------------------------*/
    }
}