using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using WebApplication16.DataBase;
using WebApplication16.Models;

namespace WebApplication16.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ClassContext _classContext;


        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ClassContext classContext)
        {
            _classContext = classContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string phone)
        {
            AppUser appUser = new AppUser();
            appUser.UserName = username;
            appUser.PhoneNumber = phone;
            var result = await _userManager.CreateAsync(appUser, password);
            if (!result.Succeeded)
            {
                string error = "";
                foreach (var item in result.Errors)
                {
                    error += item;

                }
                ModelState.AddModelError("", error);

            }
            await _classContext.SaveChangesAsync();

            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {

                ModelState.TryAddModelError("", "Bele Bir Istifadeci Yoxdur");
                return View();


            }
            var results = await _signInManager.PasswordSignInAsync(user, password, true, false);
            if (results.Succeeded)
            {
                return RedirectToAction("MyAccount", "Account", new { Id = user.Id });
            }
            else
            {

                ModelState.AddModelError("", "Sifre yazlnisdir");
                return View();
            }

        }
        [Authorize]
        public IActionResult MyAccount(string Id)
        {

            AppUser appUser = (AppUser)_classContext.Users.FirstOrDefault(x => x.Id == Id);
            if (appUser == null)
            {

                ModelState.AddModelError("", "Musteri melumatlari tapilmadi");
                return RedirectToAction("Login", "Account");
            }



            return View(appUser);
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult ForgotPssword()
        {


            return View();
        }
        [HttpPost]

        public async Task<IActionResult> ForgotPssword(string username)
        {



            AppUser appUser = (AppUser)_classContext.Users.FirstOrDefault(x => x.UserName == username);
            string phonenumber = appUser.PhoneNumber.ToString();
            phonenumber = phonenumber.Substring(phonenumber.Length - 9);
            Random random = new Random();
            int randomint = random.Next(999, 9999);
            appUser.OTP = randomint.ToString();
            string urlAPI = $"https://heydaroghlu.com/api/Accounts/Sms?number={phonenumber}&message= Tesdiq Kodunuz: {appUser.OTP}";


            using (HttpClient client = new HttpClient())
            {

                HttpResponseMessage message = await client.GetAsync(urlAPI);
                string result = await message.Content.ReadAsStringAsync();
                return View();

            }
        }
    }
}
