using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TGClothes.Models
{
    public class RegisterModel
    {
        [Key]
        public long Id { get; set; }

        [Display(Name ="Tên người dùng")]
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(20, MinimumLength = 6,  ErrorMessage = "Độ dài mật khẩu ít nhất 6 ký tự")]
        public string Password { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage ="Mật khẩu xác nhận không trùng khớp")]
        public string ConfirmPassword { get; set; }
    }
}