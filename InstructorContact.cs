using System;

namespace AddressBook
{
    public class InstructorContact : Contact
    {
        private string officeLocation;

        public InstructorContact()
        {}

        public InstructorContact(string first, string last) : base(first, last)
        {}

        public InstructorContact(string first, string last, string phone, string email) : base(first, last, phone, email)
        {}
        public InstructorContact(string first, string last, string phone, string email, string office) : base(first, last, phone, email)
        {
            officeLocation = office;
        }

        public InstructorContact(Contact contact) // for casting Contact ==> InstructorContact
        {
            firstName = contact.first;
            lastName = contact.last;
            phoneNumber = contact.phone;
            emailAddress = contact.email;
            officeLocation = "NO OFFICE LOCATION GIVEN";
        }

        public string office
        {
            get { return officeLocation; }
            set { officeLocation = value;}
        }

        public override string toString()
        {
            return base.toString() + " Office: " + officeLocation;
        }

        public override string toStringCSV()
        {
            return base.toStringCSV() + "," + officeLocation;
        }
    }
}