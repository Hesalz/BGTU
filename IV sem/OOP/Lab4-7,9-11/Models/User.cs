using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab4_5.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефон обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        [StringLength(20, ErrorMessage = "Телефон не должен превышать 20 символов")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsAdmin { get; set; } = false;

        [StringLength(10)]
        public string Language { get; set; } = "ru";
        [StringLength(10)]
        public string Theme { get; set; } = "Dark";
    }
}