﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeEvaluationSystem.Entity;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication
{
    public class PersonalAspNetUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MailingAddress { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string EmployeeID { get; set; }

        public static PersonalAspNetUserViewModel Convert(AspNetUser x)
        {
            if (x == null)
            {
                throw new Exception();
            }

            return new PersonalAspNetUserViewModel()
            {
                Id = x.Id,
                EmployeeID  = x.EmployeeID,
                Email = x.Email,
                EmailConfirmed = x.EmailConfirmed,
                FirstName = x.FirstName,
                LastName = x.LastName,
                MailingAddress = x.MailingAddress,
                PhoneNumber = x.PhoneNumber
            };
        }
    }
}