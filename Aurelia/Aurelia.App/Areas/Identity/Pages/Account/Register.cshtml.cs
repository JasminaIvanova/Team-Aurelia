// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Aurelia.App.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AureliaUser> _signInManager;
        private readonly UserManager<AureliaUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<AureliaUser> _userStore;
        private readonly IUserEmailStore<AureliaUser> _emailStore;
        private readonly ApplicationDbContext _aureliaDB;
        private readonly IEmailSender _emailSender;


        public RegisterModel(
            UserManager<AureliaUser> userManager,
            IUserStore<AureliaUser> userStore,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AureliaUser> signInManager,
            ApplicationDbContext aureliaDB,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailStore = GetEmailStore();
            _aureliaDB = aureliaDB;
            _emailSender = emailSender;
           
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

    
        public class InputModel
        {
          
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required] 
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First name")]
            public string FirstName { get; set; }
            [Required]
            [Display(Name = "Second name")]
            public string SecondName { get; set; }
            [Required]
            [Display(Name = "Last name")]
            public string LastName { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDB.ProductCategories.ToList(), "Id", "Name");
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDB.ProductCategories.ToList(), "Id", "Name");

            if (ModelState.IsValid)
            {
                AureliaUser user = new AureliaUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.SecondName = Input.SecondName;
                user.Email = Input.Email;
                

                if (_userManager.Users.Count() == 0)
                {
                    await _userManager.AddToRoleAsync(user, "SuperAdmin");
                }
                else 
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    
                }

               

                await _userStore.SetUserNameAsync(user , Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AureliaUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AureliaUser)}'");


            }
        }

        private IUserEmailStore<AureliaUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AureliaUser>) _userStore;
        }

    }
}
