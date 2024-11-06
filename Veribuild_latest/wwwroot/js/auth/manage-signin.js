'use strict';
import HttpClient from '../utils/HttpClient.js'; 
import Validation from '../utils/validation.js';

let isEmailPassword = false;
document.addEventListener('DOMContentLoaded', function () {
    init();
});

function init() {
    $('#mobile1, #mobile2').intlTelInput({
        autoHideDialCode: true,
        formatOnDisplay: true,
        initialCountry: "au",
        nationalMode: false,
        separateDialCode: true,
        preferredCountries : ['au']
    });

    $("#updateEmail").on('click', function () {
        $(this).parent().find("form .form-group:nth-child(1), form .form-group:nth-child(2)").slideToggle();
        $(this).parent().find("form .form-group:nth-child(3), form .form-group:nth-child(4)").slideToggle();
        var lbltext = $(this).find("h6").find("label");
        if (lbltext == "Email") {
            lbltext.text("Mobile");
            isEmailPassword = false;
        } else {
            lbltext.text("Email");
            isEmailPassword = true;
        }
    });

    $("#updateOTP").on('click', function () {
        $(this).parent().find("form .form-group:nth-child(odd)").slideToggle();
        $(this).parent().find("form .form-group:nth-child(even)").slideToggle();
        var lbltext = $(this).find("h6").find("label");
        if (lbltext == "Email") {
            lbltext.text("Mobile");
            isEmailPassword = false;
        } else {
            lbltext.text("Email");
            isEmailPassword = true;
        }
    });

    document.querySelectorAll('input[data-role="password-validation"]').forEach(inp => {
        inp.addEventListener('input', function () {
            this.parentElement.querySelector('span[role="alert"]').innerHTML = '';
        });
    });

    document.querySelectorAll('input[data-role="email-validation"]').forEach(inp => {
        inp.addEventListener('input', function () {
            if (!Validation.email(document.querySelector('#useremail_pass').value)) {
                this.parentElement.querySelector('span[role="alert"]').innerHTML = ErrorMessages.Email;
            } else {
                this.parentElement.querySelector('span[role="alert"]').innerHTML = '';
            }
        });
    });

    document.getElementById('btn_signinwithpassword').addEventListener('click', function () {
        let isValid = true;
        if (isEmailPassword) {
            if (document.querySelector('#useremail_pass').value.length == 0) {
                document.querySelector('#email_error_msg').innerHTML = ErrorMessages.EmailEmpty;
                isValid = false;
            }
            if (!Validation.email(document.querySelector('#useremail_pass').value)) {
                document.querySelector('#email_error_msg').innerHTML = ErrorMessages.Email;
                isValid = false;
            }
            if (document.querySelector('#userpassword_pass').value.length == 0) {
                document.querySelector('#password_err_msg').innerHTML = ErrorMessages.Passowrd;
                isValid = false;
            }
        } else {
            if (document.querySelector('#mobile2').value.length == 0) {
                document.querySelector('#user_phone_err_msg').innerHTML = ErrorMessages.PhoneNumber;
                isValid = false;
            }
            if (document.querySelector('#userpassword_phone').value.length == 0) {
                document.querySelector('#password_phone_err_msg').innerHTML = ErrorMessages.Passowrd;
                isValid = false;
            }
        }
        if (isValid) {
            showLoader(LoaderMessages.Signingin, 1000, 80);
            let fd = new FormData();
            fd.append('IsEmailLogin', isEmailPassword);
            fd.append('Email', document.querySelector('#useremail_pass').value);
            fd.append('Password', isEmailPassword ? document.querySelector('#userpassword_pass').value : document.querySelector('#userpassword_phone').value);
            fd.append('PhoneNumber', document.querySelector('#mobile2').value);
            fd.append('PhoneCode', '+' + $("#mobile2").intlTelInput("getSelectedCountryData").dialCode);

            let client = new HttpClient();
            client.post('/login/ValidateSignin', fd, function (data) {
                hideLoader(500);
                if (data.code == 200) {
                    window.location.href = data.message;
                }
                if (data.code == 400) {
                    if (isEmailPassword) {
                        document.querySelector('#password_err_msg').innerHTML = data.message;
                    }
                    else {
                        document.querySelector('#password_phone_err_msg').innerHTML = data.message;
                    }
                    //showError(data.message);
                }
            }, function (xhr) {
                hideLoader(500);
                showError(ErrorMessages.Error500);
            }, this);
        }
        
    });

}
