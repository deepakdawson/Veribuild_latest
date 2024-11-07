export default class Validation {
    validation = false;
    static Messages = {
        RequiredMessage: 'This field is required.',
        Email: 'Please enter valid email',
        Phone: 'Pleas enter valid phone',
        Password: 'Password must contain at least one small, one capital,one number and one special symbol.',
        ConfirmPassword: 'Password and confirm password did not match.'
    };
    static email(email) {
        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        return reg.test(email);
    };
    static password(pass) {
        const reg = /^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$/;
        return reg.test(pass);
    };
}
Validation.prototype.validate = function (elements = []) {
    elements.forEach(element => {
        if (element.getAttribute('type') == 'email') {
            if (element.value == '') {
                this.message = ErrorMessages.Mandatory;
                this.validation = true;
                element.style.border = '2px solid red';
            } else if (!validateEmail(element.value)) {
                this.message = ErrorMessages.Email;
                element.style.border = '2px solid red';
                this.validation = true;
            } else {
                element.style.border = '1px solid #a7a7a7';
            }
        }
        else if (element.nodeName == "SELECT") {
            if (element.value == 'Choose Country' || element.value == 'Select Property' || element.value == 'Select Role' || element.value == 'Select Contract') {
                element.nextElementSibling.style.border = '2px solid red';
                this.message = ErrorMessages.Mandatory;
                this.validation = true;
            } else {
                element.nextElementSibling.style.border = '1px solid #a7a7a7';
            }
        }
        else if (element.nodeName == "TEXTAREA") {
            if (element.value == '') {
                element.style.border = '2px solid red';
                this.validation = true;
                this.message = ErrorMessages.Mandatory;
            } else {
                element.style.border = '1px solid #a7a7a7';
            }
        }
        else if (element.getAttribute('type') == 'number') {
            if (element.value == '') {
                //this.message = '<p>Please enter all required fields.</p>';
                element.style.border = '2px solid red';
                this.validation = true;
            } else if (element.value.length < 9 || element.value.length > 10) {
                this.message = ErrorMessages.PhoneNumber;
                element.style.border = '2px solid red';
                this.validation = true;
            } else {
                element.style.border = '1px solid #a7a7a7';
            }
        }
        else if (element.getAttribute('type') == 'password') {
            if (!validatePassword(element.value)) {
                //this.message = '<p>Please enter all required fields.</p>';
                this.validation = true;
                element.style.border = '2px solid red';
            } else {
                element.style.border = '1px solid #a7a7a7';
            }
        }
        else {
            if ('validationName' in element.dataset) {
                if (element.dataset.validationName == 'FirstName' && element.value == '') {
                    this.validation = true;
                    this.message = ErrorMessages.FirstName;
                    element.style.border = '2px solid red';
                } else {
                    element.style.border = '1px solid #a7a7a7';
                }
                if (element.dataset.validationName == 'LastName' && element.value == '') {
                    this.validation = true;
                    this.message = ErrorMessages.LastName;
                    element.style.border = '2px solid red';
                } else {
                    element.style.border = '1px solid #a7a7a7';
                }
                if ((element.dataset.validationName == 'PhoneNumber' && element.value == '') || (element.dataset.validationName == 'PhoneNumber' && element.value.length < 8)) {
                    this.validation = true;
                    this.message = ErrorMessages.PhoneNumber;
                    element.style.border = '2px solid red';
                } else {
                    element.style.border = '1px solid #a7a7a7';
                }
            }
            else {
                if (element.value == '') {
                    element.style.border = '2px solid red';
                    this.validation = true;
                    this.message = ErrorMessages.Mandatory;
                } else {
                    element.style.border = '1px solid #a7a7a7';
                }
            }
        }
    });
    if (this.validation) { showError(this.message); }
    return this.validation;
};