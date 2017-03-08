using EmployeeEvaluationSystem.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class CategoryViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static CategoryViewModel Convert(Category x)
        {
            if(x == null)
            {
                return null;
            }

            return new CategoryViewModel
            {
                Id = x.ID,
                Name = x.Name,
                Description = x.Description
            };
        }

    }
}