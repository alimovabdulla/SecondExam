using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Xml.Linq;
using WebApplication16.DataBase;
using WebApplication16.DTOs;
using WebApplication16.Helper.Sms;
using WebApplication16.Models;

namespace WebApplication16.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ClassContext _classContext;
        SmsService _smsService = new SmsService();

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
        public async Task<IActionResult> Register(RegisterDTO register)
        {

            AppUser appUser = new AppUser();
            appUser.FullName = register.Name + "" + register.Surname;
            appUser.UserName = register.Email;
            appUser.PhoneNumber = register.PhoneNumber;


            appUser.LockoutEnabled = true;
            await _classContext.SaveChangesAsync();

            appUser.OTP = await _smsService.Send(appUser.PhoneNumber);
            var result = await _userManager.CreateAsync(appUser, register.Password);
            if (!result.Succeeded)
            {
                string error = "";
                foreach (var item in result.Errors)
                {
                    error += item;
                }
                ModelState.AddModelError("", error);
            }
            else if (result.Succeeded)
            {
 

                return RedirectToAction("AcceptAccount", "Account");
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
            if (user.LockoutEnabled == true)
            {
                ModelState.AddModelError("", "Hesabiniz Activ Deyil");
                return View();
            }
            var results = await _signInManager.PasswordSignInAsync(user, password, true, false);
            if (results.Succeeded)
            {
                return RedirectToAction("MyAccount", "Account", new { Id = user.Id });
            }
            else
            {

                ModelState.AddModelError("", "Sifre yalnisdir");
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
        [HttpGet]
        public IActionResult AcceptAccount()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AcceptAccount(string phone, string otp)
        {
            AppUser appUser = (AppUser)await _classContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if (appUser == null)
            {
                ModelState.AddModelError("", "Telefon Nomresi sehfdir");
                return View();
            }

            if (appUser.OTPtimer != DateTime.MinValue && appUser.OTPtimer < DateTime.UtcNow.AddHours(4))
            {
                appUser.OTPtimer = DateTime.MinValue;
                appUser.CheckOTP = 0;
                await _classContext.SaveChangesAsync();
            }

            if (appUser.CheckOTP >= 2)
            {

                await _classContext.SaveChangesAsync();
                ModelState.AddModelError("", "1 deqiqe sonra yeniden cehd edin");
                return View();
            }

            // Check if the OTP timer is still active (1 minute lock period)
            if (appUser.OTPtimer != DateTime.MinValue && appUser.OTPtimer > DateTime.UtcNow.AddHours(4))
            {
                ModelState.AddModelError("", "1 deqiqe sonra yeniden cehd edin");
                return View();
            }

            if (appUser.OTP == otp)
            {
                appUser.LockoutEnabled = false;
                appUser.CheckOTP = 0;
                await _classContext.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            else
            {
                appUser.CheckOTP++;
                if (appUser.CheckOTP >= 2)
                {
                    // Start the lockout timer
                    appUser.OTPtimer = DateTime.UtcNow.AddHours(4).AddMinutes(1);
                }
                await _classContext.SaveChangesAsync();
                ModelState.AddModelError("", "OTP yanlışdır");
            }

            return View();
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
        //[HttpPost]

        //public async Task<IActionResult> ForgotPssword(string username)
        //{



        //    AppUser appUser = (AppUser)_classContext.Users.FirstOrDefault(x => x.UserName == username);
        //    string phonenumber = appUser.PhoneNumber.ToString();
        //    phonenumber = phonenumber.Substring(phonenumber.Length - 9);

        //}
    }
}
