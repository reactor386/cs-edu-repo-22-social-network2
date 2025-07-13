//-
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using SocialNetwork2.Models;
using SocialNetwork2.ViewModels;


namespace SocialNetwork2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;


    public HomeController(
        ILogger<HomeController> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }


    [Route("")]
    [Route("[controller]/[action]")]
    public IActionResult Index()
    {
        // return View();

        if (_signInManager.IsSignedIn(User)) 
        {
            return RedirectToAction("MyPage", "AccountManager");
        }
        else
        {
            return View(new MainViewModel());
        }
    }


    [Route("[action]")]
    public IActionResult Privacy()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
