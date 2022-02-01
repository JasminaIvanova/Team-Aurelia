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
using Aurelia.App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
      

        public RegisterModel(
            UserManager<AureliaUser> userManager,
            IUserStore<AureliaUser> userStore,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AureliaUser> signInManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _signInManager = signInManager;
           
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


        public async Task OnGetAsync(){}

        public async Task<IActionResult> OnPostAsync()
        {
            
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
                
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    return RedirectToPage("/Account/Login");
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

    }
}
