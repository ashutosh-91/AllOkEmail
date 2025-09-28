using AllOkEmail.Extensions;

namespace AllOkEmail.Services.Interface
{
    public interface IEmailValidationService
    {
        public Task<ApiResponse> ValidateEmailAsync(string email);
    }
}
