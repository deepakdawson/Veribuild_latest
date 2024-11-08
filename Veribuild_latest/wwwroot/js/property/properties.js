'use strict'
document.addEventListener('DOMContentLoaded', function () {
    mapUi();
});

function mapUi() {
    document.querySelector('#searchbox').addEventListener('keyup', function (e) {
        if (e.key == 'Enter') {
            let url = new URL(window.location.href);
            url.searchParams.set('address', this.value)
            window.location.href = url.toString();
        }
    });
}