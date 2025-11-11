using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogic;

namespace TripSyncV2.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;

        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var (success, userId, error) = _userService.Login(Username, Password);

            if (success)
            {
                HttpContext.Session.SetInt32("UserId", userId.Value);
                HttpContext.Session.SetString("Name", Username);
                return RedirectToPage("/Index");
            }

            ErrorMessage = error;
            return Page();
        }
    }
}
