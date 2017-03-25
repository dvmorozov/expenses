using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SocialApps.Models;

namespace SocialApps.Controllers
{
    [Authorize]
    public class ManageController : PersonalizedController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            try 
            { 
                ViewBag.StatusMessage =
                    message == ManageMessageId.ChangePasswordSuccess ? "Your password was changed."
                    : message == ManageMessageId.SetPasswordSuccess ? "Password was set up."
                    : message == ManageMessageId.SetTwoFactorSuccess ? "Two-factor authentication provider was set up."
                    : message == ManageMessageId.Error ? "Error occured."
                    : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                    : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was deleted."
                    : "";

                var userId = User.Identity.GetUserId();
                var model = new IndexViewModel
                {
                    HasPassword = HasPassword(),
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                    Logins = await UserManager.GetLoginsAsync(userId),
                    BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
                };
                return View(model);
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "Index"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            try 
            { 
                ManageMessageId? message;
                var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    message = ManageMessageId.RemoveLoginSuccess;
                }
                else
                {
                    message = ManageMessageId.Error;
                }
                return RedirectToAction("ManageLogins", new { Message = message });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "RemoveLogin"));
            }
        }

        public ActionResult AddPhoneNumber()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "AddPhoneNumber"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            try 
            { 
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                // Создание и отправка маркера
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
                if (UserManager.SmsService != null)
                {
                    var message = new IdentityMessage
                    {
                        Destination = model.Number,
                        Body = "Ваш код безопасности: " + code
                    };
                    await UserManager.SmsService.SendAsync(message);
                }
                return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "AddPhoneNumber"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            try 
            { 
                await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", "Manage");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "EnableTwoFactorAuthentication"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            try
            {            
                await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", "Manage");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "DisableTwoFactorAuthentication"));
            }
        }

        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            try 
            {
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
                // Отправка SMS через поставщик SMS для проверки номера телефона
                return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "VerifyPhoneNumber"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            try
            { 
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
                }
                // Это сообщение означает наличие ошибки; повторное отображение формы
                ModelState.AddModelError("", "Can't check phone number");
                return View(model);
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "VerifyPhoneNumber"));
            }
        }

        public async Task<ActionResult> RemovePhoneNumber()
        {
            try 
            {
                var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
                if (!result.Succeeded)
                {
                    return RedirectToAction("Index", new { Message = ManageMessageId.Error });
                }
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "RemovePhoneNumber"));
            }
        }

        public ActionResult ChangePassword()
        {
            try 
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "ChangePassword"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            { 
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "ChangePassword"));
            }
        }

        public ActionResult SetPassword()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "SetPassword"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        if (user != null)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        }
                        return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    AddErrors(result);
                }

                // Это сообщение означает наличие ошибки; повторное отображение формы
                return View(model);
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "SetPassword"));
            }
        }

        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            try 
            {
                ViewBag.StatusMessage =
                    message == ManageMessageId.RemoveLoginSuccess ? "External login deleted."
                    : message == ManageMessageId.Error ? "Error occured."
                    : "";
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user == null)
                {
                    return View("Error");
                }
                var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
                var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
                ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
                return View(new ManageLoginsViewModel
                {
                    CurrentLogins = userLogins,
                    OtherLogins = otherLogins
                });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "ManageLogins"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            try
            {
                // Запрос перенаправления к внешнему поставщику входа для связывания имени входа текущего пользователя
                return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "LinkLogin"));
            }
        }

        public async Task<ActionResult> LinkLoginCallback()
        {
            try 
            {
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
                if (loginInfo == null)
                {
                    return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
                }
                var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
                return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Manage", "LinkLoginCallback"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}