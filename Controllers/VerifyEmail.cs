using System.Net;
using AllOkEmail.Enums;
using AllOkEmail.Extensions;
using AllOkEmail.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AllOkEmail.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class VerifyEmail : ControllerBase
    {
        private readonly IEmailValidationService _emailValidationService;
        private readonly ILogger<VerifyEmail> _logger;

        public VerifyEmail(ILogger<VerifyEmail> logger,IEmailValidationService emailValidationService)
        {
            _logger = logger;
            _emailValidationService = emailValidationService;
        }

        [HttpGet(Name="Verify Email V1")]
        public async Task<ApiResponse> V1(string email)
        {
            var response= await _emailValidationService.ValidateEmailAsync(email);
            if (!response.IsSuccess)
            {
                return new ApiResponse(false, response.StatusCode, nameof(response.Status),response.Message, email,email.Split('@')[1].ToLowerInvariant());
            }

            return new ApiResponse(true, HttpStatusCode.OK,nameof(Status.Success), "Validation Passed", email, email.Split('@')[1].ToLowerInvariant());
        }
        
    }
}
