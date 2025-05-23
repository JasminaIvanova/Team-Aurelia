﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Aurelia.App.Areas.Identity.Pages.Account.Manage
{
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<AureliaUser> _userManager;
        private readonly ILogger<Disable2faModel> _logger;
        private readonly ApplicationDbContext _aureliaDB;

        public Disable2faModel(
            UserManager<AureliaUser> userManager,
            ILogger<Disable2faModel> logger,
            ApplicationDbContext aureliaDB)
        {
            _userManager = userManager;
            _logger = logger;
            _aureliaDB = aureliaDB;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDB.ProductCategories.ToList(), "Id", "Name");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException($"Cannot disable 2FA for user as it's not currently enabled.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDB.ProductCategories.ToList(), "Id", "Name");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred disabling 2FA.");
            }

            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", _userManager.GetUserId(User));
            StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}
