using System.ComponentModel.DataAnnotations;

namespace TestCreationSystem.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "E-posta alanı zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre alanı zorunludur")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifre ve şifre tekrar eşleşmiyor")]
    [Display(Name = "Şifre Tekrar")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad alanı zorunludur")]
    [StringLength(100)]
    [Display(Name = "Ad")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı zorunludur")]
    [StringLength(100)]
    [Display(Name = "Soyad")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rol alanı zorunludur")]
    [Display(Name = "Rol")]
    public string Role { get; set; } = string.Empty;
}
