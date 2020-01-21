using System;

namespace AddressBook
{
    public class InstructorContact : Contact
    {
        private string officeLocation;

        /*-------------------------- Constructors --------------------------*/

        public InstructorContact()
        {}

        public InstructorContact(string first, string last) : base(first, last)
        {} // end 2 arg ctor

        public InstructorContact(string first, string last, string phone) : base(first, last, phone)
        {} // end 3 arg ctor
        public InstructorContact(string first, string last, string phone, string email) : base(first, last, phone, email)
        {} // end 4 arg ctor
        public InstructorContact(string first, string last, string phone, string email, string office) : base(first, last, phone, email)
        {
            officeLocation = office;
        } // end 5 arg ctor

        public InstructorContact(Contact contact) // for casting Contact ==> InstructorContact
        {
            firstName = contact.first;
            lastName = contact.last;
            phoneNumber = contact.phone;
            emailAddress = contact.email;
            officeLocation = "Null!";
        } // end conversion ctor

        /*-------------------------- End Constructors --------------------------*/

        /*-------------------------- Attributes --------------------------*/

        public string office
        {
            get { return officeLocation; }
            set { officeLocation = value;}
        } // end office attribute

         /*-------------------------- End Attributes --------------------------*/

         /*-------------------------- Utilities --------------------------*/

        public override string ToDisplayString()
        {
            return base.ToDisplayString() + " Office: " + officeLocation;
        } // end ToDisplayString()

        public override string ToStringCSV()
        {
            return base.ToStringCSV() + "," + officeLocation;
        } // end ToStringCSV()
        /*-------------------------- End Utilities --------------------------*/

    }
}