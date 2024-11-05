namespace App.Foundation.Messages
{
    public class ErrorMessages
    {
        public const string InvlaidCredential = "Invalid Credential";
        public const string BlobUploadError = "Failed to upload blob";
        public const string QrCodeError = "Failed to generate qr code";
        public const string MaxSizeError = "Max file size reached";
        public const string AccountLocked = "Your account is locked. Please contact admin.";
        public const string PlivoMessageErrorMessage = "Phone number is not valid.";
        public const string Error500 = "Something went wrong!";
        public const string UserExistWithPhone = "A User with this phone number already exists.";
        public const string UserExist = "A User with this email or phone number already exists.";
        public const string OTPInvalid = "OTP is invalid";
        public const string PhoneNotVerified = "Please verify your phone.";
        public const string PasswordValidationError = "Password should contain atleast one upercase, one lowercase, letter, and one special character and one number.";
        public const string InvalidCurrentPassword = "Invalid current assword.";
        public const string RoleExist = "Role is already exist.";
        public const string RoleEmpty = "Role can not be empty.";
        public const string EventEmpty = "Event Name can not be empty.";

    }
}
