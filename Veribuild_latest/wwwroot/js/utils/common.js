const allowedKeys = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "Backspace", "Tab", "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"];
const allowedKeysNum = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "Backspace", "Tab", "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"];
const regExp = /^\d+$/;
document.addEventListener('DOMContentLoaded', function () {
    initNumberFields();
});
function initNumberFields() {
    document.querySelectorAll('.number-field').forEach(input => {
        input.addEventListener('keypress', function (e) {
            if (allowedKeysNum.indexOf(e.key) == -1) {
                e.preventDefault();
            }
        });
    });
}
function formatDate(date) {
    let day = date.getDate() > 9 ? date.getDate() : '0' + date.getDate();
    let month = date.getMonth() + 1 >= 9 ? date.getMonth() + 1 : '0' + (date.getMonth() + 1);
    return day + '-' + month + '-' + date.getFullYear();;
}
function showError(msg) {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        html: msg,
    });
}
function showErrorReload(msg) {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        html: msg,
    }).then(function () {
        window.location.reload();
    });
}
function showSuccess(message, title = 'Success') {
    Swal.fire({
        icon: "success",
        title: title,
        html: message,
    });
}
function showSuccessReload(messagem, title = 'Success') {
    Swal.fire({
        icon: "success",
        title: title,
        html: messagem,
    }).then(() => {
        window.location.reload();
    });
}
function isValidUrl(url) {
    try {
        new URL(url);
        return true;
    } catch (e) {
        return false;
    }
}