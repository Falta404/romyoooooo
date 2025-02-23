using QR_Domain.Entities;

namespace QR.Application.DTOs
{
    public record TokenResponse(string AccessToken, string RefreshToken);
}
