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
using E_Vote_System.Models;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace E_Vote_System.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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
            Session["RegisterViewModelComplete"] = null;
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            Session["RegisterViewModelComplete"] = null;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            Utils.LogDebug(result);

            string message = "";
            switch (result)
            {                
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    message = Utils.GenerateToastError("Your account has been locked");
                    TempData["Message"] = message;
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    message = Utils.GenerateToastError("Please verify your account before proceeding via the code send to your email address during registration");
                    TempData["Message"] = message;
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            Session["RegisterViewModelComplete"] = null;
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
            Session["RegisterViewModelComplete"] = null;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
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
            Session["RegisterViewModelComplete"] = null;
            return View(new RegisterViewModelStart());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModelStart model)
        {
            Session["RegisterViewModelComplete"] = null;
            try
            {

                if (ModelState.IsValid)
                {

                    ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);


                    if (user != null)
                    {
                        TempData["Message"] = Utils.GenerateToastError("A user with this email address already exists");
                    }else if(model.Email == Configs.AdminUserEmail)
                    {
                        bool userCreated = await RegisterAdmin(model.Email);

                        if (!userCreated)
                        {
                            TempData["Message"] = Utils.GenerateToastError("Failed to create admin user. Please check the logs.");
                        }
                        else
                        {
                            TempData["Message"] = Utils.GenerateToastSuccess("Administrator account has been created successfully.");

                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        VoterModel voter = dbContext.VoterModels.FirstOrDefault(v => v.EmailAddress == model.Email);

                        if (voter == null)
                        {
                            TempData["Message"] = Utils.GenerateToastError("Sorry you cannot proceed with your registration. The given Email address could not be found in the voters list.");
                        }
                        else
                        {

                            RegisterViewModelComplete modelComplete = new RegisterViewModelComplete
                            {
                                Email = model.Email,
                                FirstName = voter.FirstName,
                                LastName = voter.LastName
                            };

                            Session["RegisterViewModelComplete"] = modelComplete;

                            TempData["Message"] = Utils.GenerateToastSuccess("Voter information for your submitted email address has been retrieved successfully. Please proceed to complete your registration.");

                            return RedirectToAction("RegisterViewModelComplete");
                            //return View("RegisterComplete", modelComplete);
                        }
                    }

                }
                else
                {
                    TempData["Message"] = Utils.GenerateToastError("Please enter a valid email address");
                }

            }
            catch (Exception e)
            {
                TempData["Message"] = Utils.GenerateToastError(e.Message);
                Utils.LogException(e);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task<bool> RegisterAdmin(string emailAddress)
        {
            bool flag = false;

            try
            {

                if(emailAddress == Configs.AdminUserEmail)
                {
                    var user = new ApplicationUser { UserName = Configs.AdminUserEmail, Email = Configs.AdminUserEmail, FirstName = Configs.AdminUserFirstName, LastName = Configs.AdminUserLastName, Address = Configs.AdminUserAddress };
                    user.EmailConfirmed = true;
                    user.PhoneNumberConfirmed = true;
                    user.LockoutEnabled = false;
                    user.DateCreated = DateTime.Now;
                    
                    var result = await UserManager.CreateAsync(user, Configs.AdminUserPassword);

                    if (!result.Succeeded)
                    {
                        string error = string.Join(",", result.Errors.ToArray());

                        Utils.LogErrors("Admin Creation - " + error);

                        return false;
                    }

                    IdentityResult identityResult = UserManager.AddToRole(user.Id, "Administrator");

                    if (!identityResult.Succeeded)
                    {
                        string error = string.Join(",", identityResult.Errors.ToArray());

                        Utils.LogErrors("Admin Role Addition - " + error);

                        return false;
                    }

                    flag = identityResult.Succeeded;

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

            }catch(Exception e)
            {
                Utils.LogException(e);
            }

            return flag;
        }
        
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterOriginal(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    List<IdentityRole> roles = new List<IdentityRole> {  };



                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Address = model.Address };

                    if (string.Equals(user.Email, Configs.AdminUserEmail)) {
                        user.EmailConfirmed = true;
                        user.PhoneNumberConfirmed = true;
                        user.LockoutEnabled = false;
                    }
                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        try
                        {
                            if(string.Equals(user.Email, Configs.AdminUserEmail))
                            {
                                

                                UserManager.AddToRole(user.Id, "Administrator");
                                ViewBag.Message = "You have been registered successfully as an administrator.";


                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                UserManager.AddToRole(user.Id, "Voter");
                                ViewBag.Message = "You have been registered successfully as a voter. Check your email and confirm your account, you must be confirmed "
                                    + "before you can log in.";
                            }

                        }
                        catch (Exception e)
                        {
                            Utils.LogException(e);
                        }

                        ViewBag.Message = ViewBag.Message ?? "You have been registered successfully";



                        TempData["Message"] = Utils.GenerateToastSuccess("Registration successful", "Account Registration");

                        //TempData["Message"] = Utils.GenerateToastSuccess("Your account has been created successfully", "Account Registration");
                        return View("Info");
                    }

                    AddErrors(result);
                }
            }
            catch(Exception e)
            {
                TempData["Message"] = Utils.GenerateToastError(e.Message);
                Utils.LogException(e);
            }
            

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> RegisterStart(RegisterViewModelStart model)
        {
            Session["RegisterViewModelComplete"] = null;
            try
            {

                if (ModelState.IsValid)
                {

                    ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);

                    if(user != null)
                    {
                        ViewBag.Message = Utils.GenerateToastError("A user with this email address already exists");
                    }
                    else
                    {
                        VoterModel voter = dbContext.VoterModels.FirstOrDefault(v => v.EmailAddress == model.Email);

                        if (voter == null)
                        {
                            ViewBag.Message = Utils.GenerateToastError("Sorry you cannot proceed with your registration. The given Email address could not be found in the voters list.");
                        }
                        else
                        {

                            RegisterViewModelComplete modelComplete = new RegisterViewModelComplete
                            {
                                Email = model.Email,
                                FirstName = voter.FirstName,
                                LastName = voter.LastName
                            };

                            ViewBag.Message = Utils.GenerateToastSuccess("Voter information for your submitted email address has been retrieved successfully. Please proceed to complete your registration.");

                            return PartialView("_RegisterComplete", modelComplete);
                        }
                    }

                }
                else
                {
                    ViewBag.Message = Utils.GenerateToastError("Please enter a valid email address");
                }

            }catch(Exception e)
            {
                ViewBag.Message = Utils.GenerateToastError(e.Message);
                Utils.LogException(e);
            }

            return PartialView("_RegisterStart", model);
        }

        [AllowAnonymous]
        public ActionResult RegisterViewModelComplete()
        {

            if(Session["RegisterViewModelComplete"] == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            try
            {

                RegisterViewModelComplete modelComplete = (RegisterViewModelComplete)Session["RegisterViewModelComplete"];

                return View(modelComplete);

            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(e.Message);
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterViewModelComplete(RegisterViewModelComplete model)
        {

            if(Session["RegisterViewModelComplete"] == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            try
            {

                if (ModelState.IsValid)
                {

                    VoterModel voter = dbContext.VoterModels.FirstOrDefault(v => v.EmailAddress == model.Email);

                    if(voter != null)
                    {

                        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = voter.FirstName, LastName = voter.LastName, Address = voter.Address };

                        if (string.Equals(user.Email, Configs.AdminUserEmail))
                        {
                            user.EmailConfirmed = true;
                            user.PhoneNumberConfirmed = true;
                            user.LockoutEnabled = false;
                        }

                        user.DateCreated = DateTime.Now;

                        var result = await UserManager.CreateAsync(user, model.Password);

                        if (result.Succeeded)
                        {


                            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            string code = Utils.RandomString(6);

                            user.OTP = code;

                            await UserManager.UpdateAsync(user);

                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id}, protocol: Request.Url.Scheme);
                            //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please use the following code to activate your account: "+code);
                            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "<p>You voter account has been created successfully.</p>" +
                                "<p>Please use the following code to activate your account: " + code + "</p> " +
                                "<p>Use this link to go to the confirmation page: <a href=\"" + callbackUrl + "\">Account Confirmation Page</a></p>");

                            try
                            {
                                if (string.Equals(user.Email, Configs.AdminUserEmail))
                                {
                                    UserManager.AddToRole(user.Id, "Administrator");
                                    ViewBag.Message = "You have been registered successfully as an administrator.";


                                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    UserManager.AddToRole(user.Id, "Voter");

                                    try
                                    {

                                        voter.UserId = user.Id;
                                        dbContext.Entry(voter).State = System.Data.Entity.EntityState.Modified;
                                        await dbContext.SaveChangesAsync();

                                    }catch(Exception e)
                                    {
                                        Utils.LogException(e);
                                    }

                                    ViewBag.Message = "You have been registered successfully as a voter. An email has been send to your email address inorder to activate your account";
                                }

                            }
                            catch (Exception e)
                            {
                                Utils.LogException(e);
                            }

                            ViewBag.Message = ViewBag.Message ?? "You have been registered successfully";



                            TempData["Message"] = Utils.GenerateToastSuccess("Registration successful", "Account Registration");

                            //TempData["Message"] = Utils.GenerateToastSuccess("Your account has been created successfully", "Account Registration");
                            return View("Info");
                        }

                        AddErrors(result);

                        



                    }
                    else
                    {
                        TempData["Message"] = Utils.GenerateToastError("Voter with the given email address could not be found in the voter records");
                    }

                }

            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(e.Message);
            }

            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId)
        {
            Session["RegisterViewModelComplete"] = null;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            ViewBag.userId = userId;

            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            Session["RegisterViewModelComplete"] = null;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            if (code == null)
            {
                TempData["Message"] = Utils.GenerateToastError("Please enter a valid activation code");
                return View();
            }

            ApplicationUser user = dbContext.Users.Find(userId);

            if(user == null)
            {
                TempData["Message"] = Utils.GenerateToastError("Invalid user. Not found");
                return View();
            }


            if(user.OTP == code)
            {
                user.EmailConfirmed = true;
                user.OTP = null;

                dbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await dbContext.SaveChangesAsync();

                //await UserManager.UpdateAsync(user);

                TempData["Message"] = Utils.GenerateToastSuccess("Your account has been confirmed successfully. Please login to proceed");
                return RedirectToAction("Login");
            }
            else
            {
                TempData["Message"] = Utils.GenerateToastError("Invalid code. Please enter a valid activation code");
                return View();
            }

            return View();
        }
        
        /*[AllowAnonymous]
        public async Task<ActionResult> ConfirmEmailOriginal(string userId, string code)
        {
            Session["RegisterViewModelComplete"] = null;
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }*/

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            Session["RegisterViewModelComplete"] = null;
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            Session["RegisterViewModelComplete"] = null;
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
            Session["RegisterViewModelComplete"] = null;
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            Session["RegisterViewModelComplete"] = null;
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            Session["RegisterViewModelComplete"] = null;
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

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            Session["RegisterViewModelComplete"] = null;
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            Session["RegisterViewModelComplete"] = null;
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            Session["RegisterViewModelComplete"] = null;
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
            Session["RegisterViewModelComplete"] = null;
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
                    return RedirectToLocal(returnUrl);
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
                return RedirectToAction("Index", "Manage");
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
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
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