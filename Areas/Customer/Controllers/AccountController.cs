using System.Security.Claims;
using CinemaProject.Services;
using CinemaProject.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Customer.Controllers;

[Area("Customer")]
public sealed class AccountController : Controller
{
    private readonly IAccountService _accounts;
    private readonly IProfileImageStorage _profileImages;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAccountService accounts,
        IProfileImageStorage profileImages,
        IEmailSender emailSender,
        ILogger<AccountController> logger)
    {
        _accounts = accounts;
        _profileImages = profileImages;
        _emailSender = emailSender;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginInputModel());
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginInputModel model,
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var account = await _accounts.ValidateCredentialsAsync(model.Email, model.Password, cancellationToken);
        if (account is null)
        {
            ModelState.AddModelError(string.Empty, "The email or password is incorrect.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Name, account.Name),
            new(ClaimTypes.Email, account.Email)
        };
        AddProfileImageClaim(claims, account.ProfileImage);
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = model.RememberMe });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterInputModel model,
        IFormFile? profileImage,
        CancellationToken cancellationToken)
    {
        if (profileImage is { Length: > 0 }
            && !await _profileImages.IsValidImageAsync(profileImage, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Choose a JPG, PNG, or WEBP image smaller than 3 MB.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? profileImagePath = null;
        if (profileImage is { Length: > 0 })
        {
            profileImagePath = await _profileImages.SaveAsync(profileImage, cancellationToken);
        }

        AccountRegistrationResult result;
        try
        {
            result = await _accounts.RegisterAsync(
                model.Name, model.Email, model.Password, profileImagePath, cancellationToken);
        }
        catch
        {
            _profileImages.Delete(profileImagePath);
            throw;
        }

        if (result == AccountRegistrationResult.EmailAlreadyRegistered)
        {
            _profileImages.Delete(profileImagePath);
            ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
            return View(model);
        }

        var account = await _accounts.ValidateCredentialsAsync(model.Email, model.Password, cancellationToken);
        if (account is null)
        {
            ModelState.AddModelError(string.Empty, "Your account was created. Please sign in.");
            return View("Login", new LoginInputModel { Email = model.Email });
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Name, account.Name),
            new(ClaimTypes.Email, account.Email)
        };
        AddProfileImageClaim(claims, account.ProfileImage);
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordInputModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var reset = await _accounts.CreatePasswordResetLinkAsync(model.Email, cancellationToken);
            if (reset is not null)
            {
                var resetUrl = Url.ActionLink(
                    action: nameof(ResetPassword),
                    controller: "Account",
                    values: new { area = "Customer", token = reset.Token },
                    protocol: Request.Scheme);

                if (!string.IsNullOrWhiteSpace(resetUrl))
                {
                    await _emailSender.SendPasswordResetLinkAsync(
                        reset.Email, reset.Name, resetUrl, cancellationToken);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not send a password reset email.");
        }

        ViewData["ResetRequestSent"] = true;
        return View(new ForgotPasswordInputModel());
    }

    [HttpGet]
    public IActionResult ResetPassword(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return View("ResetPasswordInvalid");
        }

        return View(new ResetPasswordInputModel { Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordInputModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var reset = await _accounts.ResetPasswordAsync(model.Token, model.Password, cancellationToken);
        if (!reset)
        {
            return View("ResetPasswordInvalid");
        }

        TempData["PasswordResetSuccess"] = "Your password has been updated. You can sign in now.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private static void AddProfileImageClaim(ICollection<Claim> claims, string? imagePath)
    {
        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            claims.Add(new Claim("profile_image", imagePath));
        }
    }
}
