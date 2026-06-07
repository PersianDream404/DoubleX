using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Identity.Application.Contract.DTOs.Authentications;

public record LoginViewModel(
    [property: JsonPropertyName("UserName")] string UserName,
    [property: JsonPropertyName("Password")] string Password,
    [property: JsonPropertyName("Role")] int Role,
    [property: JsonPropertyName("RememberMe")] bool RememberMe,
    [property: JsonPropertyName("ReturnUrl")] string ReturnUrl = ""
);

public record LoginResultViewModel(int UserId, List<string> Roles);
public record ForgetPasswordViewModel([Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")] string UserName);
public record ResetPasswordViewModel(
    [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")] string UserName,
    [Required(ErrorMessage = "لطفا کد را وارد کنید")] string Code,
    [Required(ErrorMessage = "لطفا  رمز را وارد کنید")][DataType(DataType.Password)] string Password
    );

public record ChangePasswordViewModel(
    [ Required(ErrorMessage = "وارد کردن گذرواژه الزامی است")]
    [ MinLength(6, ErrorMessage = "گذرواژه باید حداقل 6 کاراکتر باشد")]
    [Display(Name = "گذرواژه جدید")]
    string Password,

    [ Required(ErrorMessage = "تکرار گذرواژه الزامی است")]
    //[ Compare("Password", ErrorMessage = "گذرواژه و تکرار آن مطابقت ندارند")]
    [ Display(Name = "تکرار گذرواژه جدید")]
    string Repassword,


    int UserId
);