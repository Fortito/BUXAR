using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using WebApplication6.Models;
using MailKit.Security;

namespace WebApplication6.Controllers
{
    public class PasswordChangeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public PasswordChangeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                
                ViewBag.Mesaj = "If this email is registered, a password reset link has been sent.";
                return View();
            }


            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string resetLink = Url.Action("ResetPassword", "PasswordChange", new
            {
                userId = user.Id,
                token = token
            }, Request.Scheme);


            try
            {

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("iqbal.memmedli.4224@gmail.com"));
                email.To.Add(MailboxAddress.Parse(model.Email));
                email.Subject = "Password Reset Request";

                var builder = new BodyBuilder
                {
                    HtmlBody = $@"
                                    <h2>Password Reset Request</h2>
                                    <p>Click the link below to reset your password:</p>
                                    <p><a href='{resetLink}' target='_blank'>{resetLink}</a></p>
                                    <p>If you have not made this request, please ignore this email.</p>",

                                            TextBody = $" Reset Password Link: {resetLink}"
                };

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("iqbal.memmedli.4224@gmail.com", "gsqx kwdg gpov tptg"); 
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);


                ViewBag.Mesaj = "The password reset link has been sent to your email address!";
            }
            catch (Exception ex)
            {
                ViewBag.Mesaj = "Email Error";
                ViewBag.HataDetay = ex.Message; 
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.Mesaj = "InValid User";
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                ViewBag.Mesaj = "Password Updated";
                return View();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}