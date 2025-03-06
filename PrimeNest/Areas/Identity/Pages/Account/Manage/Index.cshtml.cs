// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PrimeNest.DataAccess.Data;
using PrimeNest.Models;

namespace PrimeNest.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;

        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Required]
            public string Name { get; set; }
            public string ProfilePic { get; set; }

            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            public string About { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            // Cast user to your custom ApplicationUser if applicable
            var appUser = user as ApplicationUser;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = appUser?.Name,
                ProfilePic = appUser?.ProfilePic,
                StreetAddress = appUser?.StreetAddress,
                City = appUser?.City,
                State = appUser?.State,
                PostalCode = appUser?.PostalCode,
                About = appUser?.About
            };
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var appUser = user as ApplicationUser;
            bool isUpdated = false;

            // Handle Profile Picture Upload
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var file = files[0];

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string uploadsFolder = Path.Combine(webRootPath, "images", "User");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Update ProfilePic in the database
                string relativePath = "/images/User/" + fileName;
                if (appUser != null && appUser.ProfilePic != relativePath)
                {
                    appUser.ProfilePic = relativePath;
                    isUpdated = true;
                }
            }

            // Update Phone Number
            if (Input.PhoneNumber != await _userManager.GetPhoneNumberAsync(user))
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
                isUpdated = true;
            }

            // Update Full Name
            if (appUser != null && Input.Name != appUser.Name)
            {
                appUser.Name = Input.Name;
                isUpdated = true;
            }

            // Update About Section
            if (appUser != null && Input.About != appUser.About)
            {
                appUser.About = Input.About;
                isUpdated = true;
            }

            // Save changes to the database if any updates were made
            if (isUpdated)
            {
                var updateResult = await _userManager.UpdateAsync(appUser);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when updating your profile.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

    }
}
