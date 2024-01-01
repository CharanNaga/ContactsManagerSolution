﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Core.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage ="Email can't be blank")]
        [EmailAddress(ErrorMessage="Email address should be in a valid format")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage ="Password can't be blank")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
