// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Threading.Tasks;
using AspNetCore.ReCaptcha;
using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Aurelia.App.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<AureliaUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly ApplicationDbContext _aureliaDB;

        public PersonalDataModel(
            UserManager<AureliaUser> userManager,
            ILogger<PersonalDataModel> logger,
            ApplicationDbContext aureliaDB)
        {
            _userManager = userManager;
            _logger = logger;
            _aureliaDB = aureliaDB;
        }

        public async Task<IActionResult> OnGet()
        {
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDB.ProductCategories.ToList(), "Id", "Name");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
