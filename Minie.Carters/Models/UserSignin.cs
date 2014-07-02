using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Minie.Carters.Models
{
    public class UserSignin
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Password { get; set; }
    }

    public class UserSignup
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Senha não coincide")]
        public string ConfirmPassword { get; set; }
    }
}