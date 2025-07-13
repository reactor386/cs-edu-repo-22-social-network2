//-
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

using SocialNetwork2.Models;
using SocialNetwork2.ViewModels;
using SocialNetwork2.Extensions;
using SocialNetwork2.SignalR;


namespace SocialNetwork2.Controllers;

public class AccountManagerController : Controller
{
    private readonly ILogger<AccountManagerController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private IMapper _mapper;
    private IUnitOfWork _unitOfWork;

    // SignalR broadcasting
    private readonly IHubContext<ChatHub> _hubContext;
    // дополнительно в учебных целях создаем экземпляр клиента
    private readonly ChatClient _hubClient;


    public AccountManagerController(
        ILogger<AccountManagerController> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IHubContext<ChatHub> hubContext,
        ChatClient hubClient)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _hubClient = hubClient;
    }


    /*
    // GET: AccountManagerController
    public ActionResult Index()
    {
        return View();
    }
    */


    [Route("Login")]

    [HttpGet]
    public IActionResult Login()
    {
        return View("Home/Login");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl ?? string.Empty });
    }


    [Route("Login")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            User? user = null;
            try
            {
                user = await _userManager.FindByEmailAsync(model.Email);
            }
            catch { }

            // .PasswordSignInAsync: user.UserName ?? string.Empty, model.Password
            // .SignInAsync: user.Email ?? string.Empty, model.Password

            var result = await _signInManager.PasswordSignInAsync(
                user?.UserName ?? string.Empty, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // return RedirectToAction("Index", "Home");
                    return RedirectToAction("MyPage", "AccountManager");
                }
            }
            else
            {
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            }
        }

        return RedirectToAction("Index", "Home");
    }


    [Route("Logout")]
    [HttpPost]
    // [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    [Authorize]
    [Route("MyPage")]
    [HttpGet]
    public async Task<IActionResult> MyPage()
    {
        var user = base.User;

        User? result = await _userManager.GetUserAsync(user);

        if (result == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // SignalR
        // в учебных целях отправляем сообщение в консоль
        await _hubClient.StartAsync();
        // await _hubClient.SendMessage("usr", "msg");

        var model = new UserViewModel(result);

        model.Friends = await GetAllFriend(model.User);

        return View("User", model);
    }


    [Route("Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = base.User;

        User? result = await _userManager.GetUserAsync(user);

        var editmodel = _mapper.Map<UserEditViewModel>(result);

        return View("UserEdit", editmodel);
    }


    [Authorize]
    [Route("Update")]
    [HttpPost]
    public async Task<IActionResult> Update(UserEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // extensions method
            user.Convert(model);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("MyPage", "AccountManager");
            }
            else
            {
                return RedirectToAction("Edit", "AccountManager");
            }
        }
        else
        {
            ModelState.AddModelError("", "Некорректные данные");
            return View("UserEdit", model);
        }
    }




    // [Authorize]
    [Route("UserList")]
    [HttpGet]
    public async Task<IActionResult> UserList(string search)
    {
        var currentuser = base.User;

        if (currentuser is null || currentuser.Identity is null
            || !currentuser.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = await CreateSearch(search);
        return View("UserList", model);
    }


    [Route("AddFriend")]
    [HttpPost]
    public async Task<IActionResult> AddFriend(string id)
    {
        var currentuser = base.User;

        User? result = await _userManager.GetUserAsync(currentuser);

        var friend = await _userManager.FindByIdAsync(id);
        
        var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

        if (result != null && friend != null && repository != null)
        {
            // repository.AddFriend(result, friend);
            await repository.AddFriendAsync(result, friend);
        }

        return RedirectToAction("MyPage", "AccountManager");
    }


    [Route("DeleteFriend")]
    [HttpPost]
    public async Task<IActionResult> DeleteFriend(string id)
    {
        var currentuser = base.User;

        User? result = await _userManager.GetUserAsync(currentuser);

        var friend = await _userManager.FindByIdAsync(id);

        var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

        if (result != null && friend != null && repository != null)
        {
            // repository.DeleteFriend(result, friend);
            await repository.DeleteFriendAsync(result, friend);
        }

        return RedirectToAction("MyPage", "AccountManager");
    }


    private async Task<SearchViewModel> CreateSearch(string search)
    {
        var currentuser = base.User;

        User? result = await _userManager.GetUserAsync(currentuser);

        // var list = _userManager.Users.AsEnumerable().ToList();
        var list = await _userManager.Users.AsQueryable().ToListAsync();
        if (!string.IsNullOrEmpty(search))
        {
            list = list.Where(x => x.GetFullName().Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var withfriend = await GetAllFriend();

        var data = new List<UserWithFriendExtViewModel>();

        list.ForEach(x =>
        {
            var t = _mapper.Map<UserWithFriendExtViewModel>(x);
            t.IsFriendWithCurrent = withfriend.Any(y => y.Id == x.Id) || result?.Id == x.Id;
            data.Add(t);
        });

        var model = new SearchViewModel()
        {
            UserList = data
        };

        return model;
    }


    private async Task<List<User>> GetAllFriend(User user)
    {
        var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

        if (repository != null)
        {
            // return repository.GetFriendsByUser(user);
            return await repository.GetFriendsByUserAsync(user);
        }
        else
        {
            return new List<User>();
        }
    }


    private async Task<List<User>> GetAllFriend()
    {
        var user = base.User;

        User? result = await _userManager.GetUserAsync(user);

        var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

        if (result != null && repository != null)
        {
            // return repository.GetFriendsByUser(result);
            return await repository.GetFriendsByUserAsync(result);
        }
        else
        {
            return new List<User>();
        }
    }




    [Route("Chat")]
    [HttpGet]
    public async Task<IActionResult> Chat()
    {
        string id = base.Request.Query["id"].ToString();

        if (string.IsNullOrWhiteSpace(id))
        {
            return RedirectToAction("Index", "Home");
        }
        var model = await GenerateChat(id);

        return View("Chat", model);
    }


    [Route("Chat")]
    [HttpPost]
    public async Task<IActionResult> Chat(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return RedirectToAction("Index", "Home");
        }
        var model = await GenerateChat(id);

        return RedirectToAction("Chat", new { id = id });
    }


    private async Task<ChatViewModel> GenerateChat(string id)
    {
        var currentuser = User;

        User? result = await _userManager.GetUserAsync(currentuser);
        var friend = await _userManager.FindByIdAsync(id);

        var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

        var model = new ChatViewModel();

        if (result != null && friend != null && repository != null)
        {
            // var mess = repository.GetMessages(result, friend);
            var mess = await repository.GetMessagesAsync(result, friend);

            model.You = result;
            model.ToWhom = friend;
            model.History = mess.OrderBy(x => x.Id).ToList();
        }

        return model;
    }


    [Route("NewMessage")]
    [HttpPost]
    public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
    {
        var currentuser = User;

        User? result = await _userManager.GetUserAsync(currentuser);
        var friend = await _userManager.FindByIdAsync(id);

        var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

        if (result != null && friend != null && repository != null)
        {
            var item = new Message()
            {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            // repository.Create(item);
            await repository.CreateAsync(item);
        }

        var model = await GenerateChat(id);

        base.ModelState.Clear();

        // SignalR broadcasting
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "usr", "msg");

        return RedirectToAction("Chat", new { id = id });
    }




    [Route("GenerateBots")]
    [Route("GenerateBots({number})")]
    [HttpGet]
    public async Task<IActionResult> GenerateBots(
        [FromRoute] int? number)
    {
        var usergen = new GenetateUsers();
        var userlist = usergen.Populate(number ?? 35);

        foreach(var user in userlist)
        {
            var result = await _userManager.CreateAsync(user, "123456");

            if (!result.Succeeded)
                continue;
        }

        return RedirectToAction("Index", "Home");
    }
}
