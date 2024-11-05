using Plivo.Resource.Message;
using Plivo;

namespace App.Foundation.Services
{
    public class MessageService
    {
        public static bool SendOtp(string code, string phone, string otp)
        {
            if (code.StartsWith('+'))
            {
                code = code.Replace("+", "");
            }
            if ((code == "61" && phone.StartsWith('0')) || (code == "61" && phone.Length > 9))
            {
                phone = phone[1..];
            }
            var PhoneWithCountry = code + phone;

            PlivoApi api = new("MAODIYMZRIYTLMNJDMMM", "ZWM5ZTk0ZTFjNmRkYzY3ZjBmNGZlY2U1ZDU3NDBm");
            MessageCreateResponse response = api.Message.Create(
                    src: "VeriBuild",
                    dst: new List<string> { PhoneWithCountry },
                    text: "Dear Customer, use code " + otp + " for your account. Never share your OTP with anyone. -from VeriBuild"
                    );

            return response.StatusCode == 202;
        }
    }
}
