using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        [BindProperty]
        public RegisterViewModel Input{ get; set; }

        [BindProperty]
        public bool RegisterSuccess{get; set; }
        [BindProperty]
        public bool ErrorsExist { get; set; }

        [BindProperty]
        public IEnumerable<string> Errors { get; set; }


        public Index(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet(string returnUrl)
        {   
            Input = new RegisterViewModel{
                ReturnUrl = returnUrl
            };
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            //check apakah button tidak sama dengan register maka akan kembali ke home
            if(Input.Button != "register") return Redirect("~/");

            if(ModelState.IsValid)
            {
                //create new user
                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = Input.Email,
                    EmailConfirmed = true
                };

                //save new user
                var result = await _userManager.CreateAsync(user, Input.Password);

                if(result.Succeeded)
                {
                    await _userManager.AddClaimsAsync(user, new Claim[]
                    {
                         new Claim(JwtClaimTypes.Name, Input.FullName)
                    });

                    RegisterSuccess = true;
                }
                else
                {
                    ErrorsExist = true;
                    Errors = result.Errors.Select(x => $"{x.Description} ({x.Code})");
                    return Page();
                }
            }
            return Page();
        }
    }


}
