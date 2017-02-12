using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class CSVEmployeeViewModel
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MailingAddress { get; set; }
        public string PhoneNumber { get; set; }


        public RegisterViewModel Convert()
        {
            return new RegisterViewModel
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                MailingAddress = MailingAddress,
                PhoneNumber = PhoneNumber
            };
        }
    }

}