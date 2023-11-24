using BotDetect.Web.Mvc;
using Data.EF;
using Data.Services;
using Facebook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TGClothes.Common;
using TGClothes.Models;

namespace TGClothes.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        // GET: User
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidationActionFilter("CaptchaCode", "RegisterCaptcha", "Captcha không đúng, vui lòng thử lại!")]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (_userService.CheckEmailExist(model.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                }
                else
                {
                    var user = new User();
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.Address = model.Address;
                    user.Password = Encryptor.MD5Hash(model.Password);
                    user.CreatedDate = DateTime.Now;
                    user.Status = true;

                    var result = _userService.Insert(user);
                    if (result > 0)
                    {
                        ViewBag.Success = "Đăng ký tài khoản thành công";
                        model = new RegisterModel();
                    }
                    else
                    {
                        ViewBag.Success = "Đăng ký tài khoản không thành công";
                    }
                }
            }
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.LoginByEmail(model.Email, Encryptor.MD5Hash(model.Password));
                if (result == 1)
                {
                    var user = _userService.GetUserByEmail(model.Email);
                    var userSession = new UserLogin();
                    userSession.Email = user.Email;
                    userSession.UserId = user.Id;
                    Session.Add(CommonConstants.USER_SESSION, userSession);

                    return Redirect("/");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản đang bị khóa.");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("", "Mật khẩu không chính xác.");
                }
                else
                {
                    ModelState.AddModelError("", "Đăng nhập thất bại.");
                }
            }
            return View(model);
        }

        public ActionResult LoginWithFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbSecretKey"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbSecretKey"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstName = me.first_name;
                string middleName = me.middle_name;
                string lastName = me.last_name;

                var user = new User();
                user.Email = email;
                user.UserName = email;
                user.Status = true;
                user.Name = firstName+ " " + middleName + " " + lastName;
                user.CreatedDate = DateTime.Now;

                var data = _userService.InsertForFacebook(user);
                if (data > 0)
                {
                    var userSession = new UserLogin();
                    userSession.Email = user.Email;
                    userSession.UserId = data;
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                }
            }
            return Redirect("/");
        }

        [HttpPost]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"], "TGClothes");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"];

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Tai khoan da duoc tao thanh cong";
                body = "chua lam";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset password";
                body = "Bạn vừa gửi link xác thực tài khoản, Hãy click vào link bên dưới để lấy lại mật khẩu<br>" +
                    "<a href=" + link + ">Reset password</a>";
            }

            MailMessage mc = new MailMessage(ConfigurationManager.AppSettings["FromEmailAddress"].ToString(), email);
            mc.Subject = subject;
            mc.Body = body;
            mc.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Timeout = 1000000;
            //smtp.Timeout = 1000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential(ConfigurationManager.AppSettings["FromEmailAddress"].ToString(), ConfigurationManager.AppSettings["FromEmailPassword"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mc);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {

            if (IsValidEmail(email))
            {
                string message = "";
                var user = _userService.GetUserByEmail(email);

                if (user != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(user.Email, resetCode, "ResetPassword");
                    user.ResetPasswordCode = resetCode;
                    _userService.Update(user);

                    message = "Reset password link đã được gửi đến email của bạn";
                }


                ViewBag.Message = message;
                return View();
            }
            else
            {
                TempData["mgss"] = "Email không hợp lệ";
                return View();
            }

        }

        public ActionResult ResetPassword(string id)
        {
            var user = _userService.GetUserByResetPasswordCode(id);
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByResetPasswordCode(model.ResetCode);
                if (user != null)
                {
                    user.Password = Encryptor.MD5Hash(model.NewPassword);
                    user.ResetPasswordCode = "";

                    _userService.Update(user);
                    message = "Cập nhập mật khẩu thành công";
                    ViewBag.Message = message;
                    return RedirectToAction("Login", "User");
                }
            }
            else
            {
                message = "Cập nhập mật khẩu thất bại";
            }
            ViewBag.Message = message;
            return View(model);
        }

        public ActionResult Logout()
        {
            Session[CommonConstants.USER_SESSION] = null;
            return Redirect("/");
        }

    }
}