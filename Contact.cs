using System;
using System.Linq;

namespace AddressBook
{
    public class Contact
    {
        protected string firstName;
        protected string lastName;
        protected string phoneNumber;
        protected string emailAddress;

        public Contact() 
        {}
        /*-------------------------- Constructors --------------------------*/

        public Contact(string first, string last)
        {
            firstName = first;
            lastName = last;
        } // end 2 arg ctor()

        public Contact(string first, string last, string phoneNum) : this(first, last)
        {
            phone = phoneNum;
        } // end 3 arg ctor()

        public Contact(string first, string last, string phone, string email) : this(first, last, phone)
        {
            emailAddress = email;
        } // end 4 arg ctor

        /*-------------------------- End Constructors --------------------------*/

        /*-------------------------- Attributes --------------------------*/

        public string first
        {
            get { return firstName; }
            set { firstName = value; }
        } // end attribute first

        public string last
        {
            get { return lastName; }
            set { lastName = value; }
        } // end attribute last

        public string phone
        {
            get { return phoneNumber; }
            set 
            {
                if(value.Length != 10)
                {
                    throw new ArgumentException("Phone number must be 10 digits");
                }
                else
                {
                    if(!IsNumeric(value))
                    {
                        throw new ArgumentException("Phone number must only contain digits");
                    }
                    else
                    {
                        phoneNumber = value;
                    }
                }
            }
        } // end attribute phone

        public string email
        {
            get { return emailAddress; }
            set 
            {
                if(!value.Contains("@") || !value.Contains("."))
                {
                    throw new ArgumentException("Invalid email address");
                }
                else
                {
                    emailAddress = value;
                }
            }
        } // end attribute email

        /*-------------------------- End Attributes --------------------------*/

        /*---------- Utlities ----------*/
        private bool IsNumeric(string str)
        {
            return str.All(char.IsNumber);
        } // end IsNum

                // Virtual so that it can be overridden in subclasses to add more information
        public virtual string ToDisplayString()
        {
            return firstName + " " + lastName + " - Phone: " + "(" + phoneNumber.Substring(0,3) + ") " + phoneNumber.Substring(3, 3) + "-" + phoneNumber.Substring(6, 4) + " - Email: " + emailAddress;
        } // end ToDisplayString()

        public virtual string ToStringCSV()
        {
            return firstName + "," + lastName + "," + phoneNumber + "," + emailAddress;
        } // end ToStringCSV

        /*-------------------------- End Utilities --------------------------*/

    }
}
