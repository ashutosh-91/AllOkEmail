using AllOkEmail.Helper;
using AllOkEmail.Services.Interface;
using DnsClient;
using DnsClient.Protocol;
using System.Net;
using System.Text.RegularExpressions;
using AllOkEmail.Enums;
using AllOkEmail.Extensions;

namespace AllOkEmail.Services
{
    public class EmailValidationService : IEmailValidationService
    {
        private readonly ILookupClient _lookUpClient;
        private IEnumerable<IPAddress> _mxIpAddresses;

        public EmailValidationService(ILookupClient lookupClient)
        {
            _lookUpClient = lookupClient;
            _mxIpAddresses = new List<IPAddress>();
        }

        public async Task<ApiResponse> ValidateEmailAsync(string email)
        {
            email = email.ToLowerInvariant();
            var domain = email.Split('@')[1];
            var syntaxValidation = BasicSyntaxValidation(email);
            if (!syntaxValidation.IsSuccess)
            {
                return syntaxValidation;
            }
            var mxRecordLookup = await MxRecordLookUpAsync(domain);
            if (!mxRecordLookup.IsSuccess)
            {
                return mxRecordLookup;
            }
            var isBlackListed = await IsBlackListedBySpamhausAsync(_mxIpAddresses);
            if (!isBlackListed.IsSuccess)
            {
                return isBlackListed;
            }
            var isDisposable = await DisposableEmailValidationAsync(domain);
            if (!isDisposable.IsSuccess)
            {
                return isDisposable;
            }

            return new ApiResponse(true, "Valid Email", HttpStatusCode.OK, Status.Success, SubStatus.Valid);

        }

        /// <summary>
        /// Looks up the Mx record if its exists then we query for Type A to do a bounce check
        /// TODO:Bounce check for IPv6
        /// </summary>
        /// <param name="domain">The domain to be verified for against the MX Record lookup and it address bounce validity</param>
        /// <returns>An <see cref="ApiResponse"/></returns>
        private async Task<ApiResponse> MxRecordLookUpAsync(string domain)
        {
            try
            {
                var result = await _lookUpClient.QueryAsync(domain, QueryType.MX);
                if (result is not null)
                {
                    foreach (var record in result.Answers.OfType<MxRecord>())
                    {

                        var address = record.Exchange.Value;
                        var hasNotBounced = await _lookUpClient.QueryAsync(address, QueryType.A);
                        _mxIpAddresses = hasNotBounced.Answers.OfType<ARecord>().Select(r => r.Address);
                        if (_mxIpAddresses.Any())
                        {
                            return new ApiResponse(true, "Mx exists", HttpStatusCode.OK, Status.Success, SubStatus.Valid);
                        }
                    }
                }
            }
            catch (DnsResponseException ex)
            {
                Console.WriteLine("Exception" + ex.Message);
            }

            return new ApiResponse(false, "Mx does not exists", HttpStatusCode.BadRequest, Status.InValid, SubStatus.MxNotFound); ;
        }
        private ApiResponse BasicSyntaxValidation(string email)
        {

            var isValid = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return isValid ? new ApiResponse(true, "Is a proper email", HttpStatusCode.OK, Status.Success, SubStatus.Valid) : new ApiResponse(false, "Is not a proper email", HttpStatusCode.BadRequest, Status.InValid, SubStatus.InValidEmail);

        }

        private async Task<ApiResponse> IsBlackListedBySpamhausAsync(IEnumerable<IPAddress> ipAddresses)
        {

            foreach (var ip in ipAddresses)
            {
                var reversedIp = string.Join(".", ip.ToString().Split('.').Reverse());
                var query = $"{reversedIp}.zen.spamhaus.org";
                var blacklistCheck = await _lookUpClient.QueryAsync(query, QueryType.A);
                if (blacklistCheck.Answers.Any())
                {
                    return new ApiResponse(false,message:"Failure", HttpStatusCode.BadRequest, Status.InValid, SubStatus.Blacklisted);
                }
            }
            return new ApiResponse(true, "Success", HttpStatusCode.OK, Status.Success, SubStatus.Valid);
        }
        private async Task<ApiResponse> DisposableEmailValidationAsync(string domain)
        {
            try
            {
                return IsBlocklisted(domain)
                    ? new ApiResponse(false, "Is a Disposable Email", HttpStatusCode.BadRequest, Status.InValid,
                        SubStatus.Disposable)
                    : new ApiResponse(true, "Is a proper email", HttpStatusCode.OK, Status.Success, SubStatus.Valid);
            }
            catch (Exception ex)
            {

            }

            return new ApiResponse(false, "Invalid Request");

        }


        private static bool IsBlocklisted(string domain) => BlockListedEmail.EmailBlockList.Value.Contains(domain);







    }
}
