using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Не указано Имя пользователя")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Длина должна быть от 5 до 30 символов")]
        [Display(Name = "Логин", Prompt = "Логин")]
        [RegularExpression("[0-9a-zA-Z]+")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [Display(Name = "Введите пароль", Prompt = "Пароль")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Длина должна быть от 6 до 30 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [Display(Name = "Повторите пароль", Prompt = "Пароль")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Длина должна быть от 6 до 30 символов")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
