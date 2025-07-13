//-
using System;
using System.ComponentModel.DataAnnotations;


namespace SocialNetwork2.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "*")]
    [Display(Name = "Имя")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "*")]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "*")]
    [Display(Name = "Email")]
    public string EmailReg { get; set; } = null!;

    [Required(ErrorMessage = "*")]
    [Display(Name = "Год")]
    public int Year { get; set; }

    [Required(ErrorMessage = "*")]
    [Display(Name = "Месяц")]
    public int Month { get; set; }

    [Required(ErrorMessage = "*")]
    [Display(Name = "День")]
    public int Date { get; set; }

    [Required(ErrorMessage = "*")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
    public string PasswordReg { get; set; } = null!;

    [Required(ErrorMessage = "*")]
    [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    public string PasswordConfirm { get; set; } = null!;

    /*
    [Required(ErrorMessage = "*")]
    [Display(Name = "Никнейм")]
    public string Login { get; set; } = null!;
    */

    // RegisterViewModel
    // UserEditViewModel
    public string Login => EmailReg;
}
