using ContactManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Core.DTO
{
    //View Model class which contains the details of the fields in Register Page
    public class RegisterDTO
    {
        [Required(ErrorMessage ="Name can't be blank")]
        public string PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage ="Email address should be in valid format")]
        [Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage ="Email already exists")] //this will dynamically add JS script validation at the email box.
        public string Email {  get; set; }

        [Required(ErrorMessage = "Phone can't be blank")]
        [RegularExpression("^[0-9]*$",ErrorMessage ="Phone Number should contain only numbers")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password can't be blank")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password & Confirm Password don't match")]
        public string ConfirmPassword { get; set; }

        public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
    }
}
