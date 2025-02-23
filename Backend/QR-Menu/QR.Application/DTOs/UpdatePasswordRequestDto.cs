using System.ComponentModel.DataAnnotations;

namespace QR.Application.DTOs
{
    public class UpdatePasswordRequestDto
    {
        [ComparePasswords("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
    }
}
