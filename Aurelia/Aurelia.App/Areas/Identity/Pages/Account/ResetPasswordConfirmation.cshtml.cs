﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Aurelia.App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aurelia.App.Areas.Identity.Pages.Account
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _aureliaDB;
        public ResetPasswordConfirmationModel(ApplicationDbContext aureliaDB)
        {
            _aureliaDB = aureliaDB;
        }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public void OnGet()
        {
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDB.ProductCategories.ToList(), "Id", "Name");
        }
    }
}
