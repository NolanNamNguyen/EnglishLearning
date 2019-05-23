using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ELearningProject.Models;
using System.IO;

namespace ELearningProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            //var result = ;
            switch (await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false))
            {
                case SignInStatus.Success:
                    {
                        var user = await UserManager.FindAsync(model.Email, model.Password);
                        var x = await SignInAsync(user, returnUrl);
                        return x;
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        async Task<ActionResult> SignInAsync(ApplicationUser user, string returnUrl)
        {
            var roles = (await UserManager.GetRolesAsync(user.Id))[0];
            HttpCookie ckname;
            if (Response.Cookies["UserName"] != null)
            {
                Response.Cookies.Remove("UserName");
            }

            ckname = new HttpCookie("UserName");
            using (var db = new ApplicationDbContext())
            {
                ckname.Value = (from wu in db.Web_Users
                                join student in db.Students on wu.id equals student.web_User.id
                                where wu.UserID == user.Id
                                select wu.Name).FirstOrDefault();
            }
            Response.Cookies.Add(ckname);

            HttpCookie wuserid;
            if (Response.Cookies["WebUserID"] != null)
            {
                Response.Cookies.Remove("WebUserID");
            }

            wuserid = new HttpCookie("WebUserID");
            using (var db = new ApplicationDbContext())
            {
                wuserid.Value = (from wu in db.Web_Users
                                 where wu.UserID == user.Id
                                 select wu.id).FirstOrDefault().ToString();
            }
            Response.Cookies.Add(wuserid);

            if (roles == "Admin")
            {
                return RedirectToAction("AdminIndex", "Admin");
            }
            else if (roles == "Student")
            {
                using (var db = new ApplicationDbContext())
                {
                    int StudentID = (from wu in db.Web_Users
                                     join student in db.Students on wu.id equals student.web_User.id
                                     where wu.UserID == user.Id
                                     select student.id).FirstOrDefault();
                    var cookie = new HttpCookie("StudentID");
                    Response.Cookies.Remove("StudentID");
                    cookie.Value = StudentID.ToString();
                    Response.Cookies.Add(cookie);
                }
                return RedirectToAction("Index", "Student");
            }
            else if (roles == "Teacher")
            {
                using (var db = new ApplicationDbContext())
                {
                    int TeacherId = (from wu in db.Web_Users
                                     join teacher in db.Teachers on wu.id equals teacher.User.id
                                     where wu.UserID == user.Id
                                     select teacher.id).FirstOrDefault();
                    var cookie = new HttpCookie("TeacherId");
                    Response.Cookies.Remove("TeacherId");
                    cookie.Value = TeacherId.ToString();
                    Response.Cookies.Add(cookie);
                }
                return RedirectToAction("Index", "Teacher");
            }
            return RedirectToLocal(returnUrl);
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var temp = new RegisterViewModel()
            {
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };
            if (true)
            {
                var user = new ApplicationUser { UserName = temp.Email, Email = temp.Email };
                var result = await UserManager.CreateAsync(user, temp.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    await UserManager.SendEmailAsync(user.Id, "Welcome to ABC english",
                        "This is a confirmation of the email you register to our website");

                    var u = new Web_user()
                    {
                        Name = model.Name,
                        Birthday = model.Birthday,
                        UserID = user.Id,
                        UserImage = @"\Content\Images\default.jpg"
                    };

                    if (model.AsTeacher)
                    {
                        using (var db = new ApplicationDbContext())
                        {
                            var t = new Teacher()
                            {
                                User = u
                            };
                            db.Web_Users.Add(u);
                            db.Teachers.Add(t);
                            db.SaveChanges();
                        }

                        UserManager.AddToRole(user.Id, "Teacher");
                    }
                    else
                    {
                        using (var db = new ApplicationDbContext())
                        {
                            var s = new Student()
                            {
                                web_User = u
                            };
                            db.Web_Users.Add(u);
                            db.Students.Add(s);
                            db.SaveChanges();
                        }

                        UserManager.AddToRole(user.Id, "Student");
                    }

                    return await SignInAsync(user, "~/Home/Index");
                }
                AddErrors(result);

            }

            return RedirectToAction("About", "Home");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //Edit Profile Section

        public ActionResult EditProfile()
        {
            var WebUserID = int.Parse(Request.Cookies["WebUserID"].Value);
            ApplicationDbContext db = new ApplicationDbContext();
            EditProfileViewModel myProfile = new EditProfileViewModel();
            myProfile = (from wu in db.Web_Users
                         where wu.id == WebUserID
                         select new EditProfileViewModel()
                         {
                             UserName = wu.Name,
                             Id = wu.UserID,
                             ImagePath = wu.UserImage,
                         }).First();
            var IdenUser = UserManager.FindById(myProfile.Id);
            myProfile.Email = IdenUser.Email;

            return View("EditProfile", myProfile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult EditProfile(EditProfileViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var allowedExtensions = new[] {
            ".Jpg", ".png", ".jpg", "jpeg",".JPG",".PNG",".JPEG"
            };

            int webUserID = int.Parse(Request.Cookies["WebUserID"].Value);
            Web_user webUser = db.Web_Users.Find(webUserID);
            if (model.AvatarFile != null)
            {
                var filename = Path.GetFileName(model.AvatarFile.FileName);
                var extension = Path.GetExtension(model.AvatarFile.FileName);
                if (allowedExtensions.Contains(extension))
                {
                    if (System.IO.File.Exists(webUser.UserImage))
                    {
                        System.IO.File.Delete(webUser.UserImage);
                    }
                    string name = Path.GetFileNameWithoutExtension(filename);
                    string myImage = name + "_" + model.UserName + extension;
                    var savePath = Path.Combine(Server.MapPath("~/Content/ProfileImage"), myImage);
                    var imagePath = Path.Combine("/Content/ProfileImage/", myImage);
                    webUser.UserImage = imagePath;
                    model.AvatarFile.SaveAs(savePath);
                    webUser.Name = model.UserName;
                    db.SaveChanges();
                    return PartialView("_EditProfilePartial");
                }
                return PartialView("_WrongFileType");
            }
            else
            {
                webUser.Name = model.UserName;
                db.SaveChanges();
                return PartialView("_EditProfilePartial");
            }

            // action when user didnt have a profile image
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> EPChangePassword(EditProfileViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            int WebUserID = int.Parse(Request.Cookies["WebUserID"].Value);
            Web_user WebUser = db.Web_Users.Find(WebUserID);
            ApplicationUser IdenUser = await UserManager.FindByIdAsync(WebUser.UserID);
            if (UserManager.CheckPassword(IdenUser, model.CurrentPassword))
            {
                IdenUser.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                var result = await UserManager.UpdateAsync(IdenUser);
                if (result.Succeeded)
                {
                    return PartialView("_ChangePasswordPartial");
                }
            }
            return PartialView("_ChangePassFailed", model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> CheckPasswordEditProfile(string CurrentPassword)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            int webUserID = int.Parse(Request.Cookies["WebUserID"].Value);
            string UserID = (from wu in db.Web_Users where wu.id == webUserID select wu.UserID).First();
            var Idenuser = await UserManager.FindByIdAsync(UserID);
            bool check = await UserManager.CheckPasswordAsync(Idenuser, CurrentPassword);
            if (check == true)
            {
                return Json("right", JsonRequestBehavior.AllowGet);
            }
            else if (check == false)
            {
                return Json("wrong", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("something happend", JsonRequestBehavior.AllowGet);
            }

        }
        [AllowAnonymous]
        public PartialViewResult GeneraeChangePass()
        {

            return PartialView("_ChangePassSection");
        }


        //Edit Profile Section

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await UserManager.FindByEmailAsync(loginInfo.Email);
                    return await SignInAsync(user, returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var u = new Web_user()
                    {
                        Name = info.ExternalIdentity.Name,
                        Birthday = new DateTime(1970, 1, 1),
                        UserID = user.Id,
                        UserImage = @"/Content/Images/default.jpg"
                    };

                    if (model.AsTeacher)
                    {
                        using (var db = new ApplicationDbContext())
                        {
                            var t = new Teacher()
                            {
                                User = u
                            };
                            db.Web_Users.Add(u);
                            db.Teachers.Add(t);
                            db.SaveChanges();
                        }

                        UserManager.AddToRole(user.Id, "Teacher");
                    }
                    else
                    {
                        using (var db = new ApplicationDbContext())
                        {
                            var s = new Student()
                            {
                                web_User = u
                            };
                            db.Web_Users.Add(u);
                            db.Students.Add(s);
                            db.SaveChanges();
                        }

                        UserManager.AddToRole(user.Id, "Student");
                    }
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return await SignInAsync(user,returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}