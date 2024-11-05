using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace App.Foundation.Common
{
    public class PasswordUtil
    {
        public static readonly string EmailValidation_Regex = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

        public static readonly Regex EmailValidation_Regex_Compiled = new(EmailValidation_Regex, RegexOptions.IgnoreCase);

        public static readonly string EmailValidation_Regex_JS = $"/{EmailValidation_Regex}/";
        private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

        public static bool ValidatePassword(string password)
        {
            Regex regex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", RegexOptions.Compiled);
            return regex.IsMatch(password);
        }


        /// <summary>
        /// Checks if the given e-mail is valid using various techniques
        /// </summary>
        /// <param name="email">The e-mail address to check / validate</param>
        /// <param name="useRegEx">TRUE to use the HTML5 living standard e-mail validation RegEx, FALSE to use the built-in validator provided by .NET (default: FALSE)</param>
        /// <param name="requireDotInDomainName">TRUE to only validate e-mail addresses containing a dot in the domain name segment, FALSE to allow "dot-less" domains (default: FALSE)</param>
        /// <returns>TRUE if the e-mail address is valid, FALSE otherwise.</returns>
        public static bool IsValidEmailAddress(string email, bool useRegEx = false, bool requireDotInDomainName = false)
        {
            var isValid = useRegEx
                ? email is not null && EmailValidation_Regex_Compiled.IsMatch(email)
                : new EmailAddressAttribute().IsValid(email);

            if (isValid && requireDotInDomainName)
            {
                var arr = email!.Split('@', StringSplitOptions.RemoveEmptyEntries);
                isValid = arr.Length == 2 && arr[1].Contains('.');
            }
            return isValid;
        }
    }
}
