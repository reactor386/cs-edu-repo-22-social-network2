//-
using System;
using System.ComponentModel.DataAnnotations;


namespace SocialNetwork2.ViewModels;

public enum MonthsEnum
{
    [Display(Name = "Январь")]
    Jan = 1,
    [Display(Name = "Февраль")]
    Feb,
    [Display(Name = "Март")]
    March,
    [Display(Name = "Апрель")]
    April,
    [Display(Name = "Май")]
    May,
    [Display(Name = "Июнь")]
    June,
    [Display(Name = "Июль")]
    July,
    [Display(Name = "Август")]
    Aug,
    [Display(Name = "Сентябрь")]
    Sept,
    [Display(Name = "Октябрь")]
    Oct,
    [Display(Name = "Ноябрь")]
    Nov,
    [Display(Name = "Декабрь")]
    Dec
}
