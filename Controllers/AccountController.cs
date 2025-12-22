using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web_BanHang.Models;
using System.Threading.Tasks;

namespace Web_BanHang.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly Web_BanHang.Services.IEmailSender _emailSender;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, Web_BanHang.Services.IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");

                // Generate email confirmation token and send email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = System.Net.WebUtility.UrlEncode(token) }, Request.Scheme);
                var html = $"<p>Xin chào {user.UserName},</p><p>Vui lòng xác nhận email bằng cách click <a href=\"{callbackUrl}\">vào đây</a>.</p>";
                await _emailSender.SendEmailAsync(user.Email, "Xác nhận email - Web_BanHang", html);

                return RedirectToAction("RegisterConfirmation");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
        return View();
    }

    [HttpGet]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null) return RedirectToAction("Index", "Home");
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();
        var decoded = System.Net.WebUtility.UrlDecode(code);
        var result = await _userManager.ConfirmEmailAsync(user, decoded);
        return View(result.Succeeded ? "ConfirmEmail" : "ConfirmEmailFailed");
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, bool remember = false)
    {
        // Allow login by username or email
        var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        if (!user.EmailConfirmed)
        {
            ModelState.AddModelError(string.Empty, "Bạn cần xác nhận email trước khi đăng nhập.");
            return View();
        }

        await _signInManager.SignInAsync(user, isPersistent: remember);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}