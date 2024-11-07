import HttpClient from '../utils/HttpClient.js';
import Validation from '../utils/validation.js';
let iconCounter = 1;

document.addEventListener('DOMContentLoaded', function () {
    window.removeCredential = removeCredential;
    window.makeThumb = makeThumb;
    createThumbnail();
    mapUi();
});

function mapUi() {
    document.getElementById('btn_addimage').addEventListener('click', function (e) {
        e.preventDefault();
        document.getElementById('profileimage').click();
    });
    document.getElementById('profileimage').addEventListener('change', function () {
        if (this.files.length > 0) {
            let fr = new FileReader();
            fr.onload = e => {
                document.querySelector('#profileimgplaceholder').src = e.target.result;
            };
            fr.readAsDataURL(this.files[0]);
        }
    });
    $("#usercountrycode").select2({
        templateResult: formatState,
        dropdownParent: $('#usercountrycode').parent('div'),
        templateSelection: formatState,
    });
    document.querySelector('#btn_saveprofile').addEventListener('click', function (e) {
        e.preventDefault();
        let list = [document.getElementById('userfirstname'), document.getElementById('userlastname')];
        let validatior = new Validation();
        if (validatior.validate(list)) {
            return;
        }
        showLoader(LoaderMessages.Updating, 3000, 80);
        const fd = new FormData();
        fd.append('FirstName', document.getElementById('userfirstname').value);
        fd.append('LastName', document.getElementById('userlastname').value);
        fd.append('AgencyName', document.getElementById('agencyName').value);
        fd.append('CountryCode', getCountryCode());
        fd.append('PhoneNumber', document.getElementById('userphonenumber').value);
        fd.append('CountryId', getCountryId());
        fd.append('Address', document.getElementById('userAddress').value);
        fd.append('Website', document.getElementById('userwebsite').value);
        fd.append('ProfileImage', getProfileImg());

        let client = new HttpClient();
        client.post('/setting/update', fd, function (data) {
            hideLoader(500);
            if (data.code == 200) {
                showSuccessReload(data.message);
            }
            else {
                showError(data.message);
            }
        }, function (xhr) {
            hideLoader(500);
            showError(LoaderMessages.Error500);
        }, this);
    });
    document.getElementById('btn_verify_otp').addEventListener('click', function (e) {
        e.preventDefault();
        if ((new Validation()).validate([document.getElementById('userphonenumber')])) {
            return;
        }
        showLoader(LoaderMessages.Updating, 3000, 80);
        const fd = new FormData();
        fd.append('PhoneCode', getCountryCode());
        fd.append('PhoneNumber', document.getElementById('userphonenumber').value);
        fd.append('PhoneCodeId', getCountryId());

        let client = new HttpClient();
        client.post('/setting/VerifyPhoneNumber', fd, function (data) {
            hideLoader(500);
            if (data.code == 200) {
                showSuccess(data.message);
                document.querySelector('#optcontainer').classList.remove('d-none');
            } else {
                showError(data.message);
            }
        }, function (xhr) {
            hideLoader(500);
            showError(ErrorMessages.Error500);
        }, this);
    });
    document.getElementById('btn_submit_otp').addEventListener('click', function (e) {
        e.preventDefault();
        e.preventDefault();
        if ((new Validation()).validate([document.getElementById('userphonenumber')])) {
            return;
        }
        showLoader(LoaderMessages.Updating, 3000, 80);
        const fd = new FormData();
        fd.append('PhoneCode', getCountryCode());
        fd.append('PhoneNumber', document.getElementById('userphonenumber').value);
        fd.append('PhoneCodeId', getCountryId());
        fd.append('Otp', document.getElementById('userphoneverifyotp').value);

        let client = new HttpClient();
        client.post('/setting/VerifyPhoneVerifyOTP', fd, function (data) {
            hideLoader(500);
            if (data.code == 200) {
                showSuccess(data.message);
                document.querySelector('#optcontainer').classList.add('d-none');
            } else {
                showError(data.message);
            }
        }, function (xhr) {
            hideLoader(500);
            showError(ErrorMessages.Error500);
        }, this);
    });

    // manage credentials
    document.querySelector('#openCredpdf').addEventListener('click', function (e) {
        e.preventDefault();
        this.nextElementSibling?.click();
    });
    document.querySelector('#add-pdf-box').addEventListener('click', function (e) {
        e.preventDefault();
        iconCounter++;
        let html = `<div class="col-sm-2 position-relative">
                        <span class="btnClose" onclick="removeCredential(this)"><i class="fa-solid fa-xmark"></i></span>
                        <div class="form-group">
                            <h5 class="file-upload-head">Credentials</h5>
                            <div class="file-wrapper">
                                <label class="w-100" for="credPdfplace${iconCounter}">
                                    <img src="/images/pdf-demo.svg" class="img-fluid cursor-point" id="credPdfplace${iconCounter}"/>
                                    <input class="drop-file" type="file" accept="application/pdf" onchange="makeThumb(this)" name="credentialfile">
                                </label>
                                <div class="title-field">
                                    <input type="text" class="form-control" placeholder="title" name="credentialtitle"/>
                                </div>
                            </div>
                        </div>
                    </div>`;
        document.querySelector('#credcontainer').insertAdjacentHTML('beforeend', html);
        document.getElementById(`credPdfplace${iconCounter}`).addEventListener('click', function (e) {
            e.preventDefault();
            this.nextElementSibling?.click();
        });
    });

    document.querySelector('#btn_savecredential').addEventListener('click', function (e) {
        e.preventDefault();
        let validate = true;
        document.getElementById('credcontainer').querySelectorAll('input[name="credentialtitle"]').forEach(input => {
            if (input.value == '')
                validate = false;
        });
        if (!validate) { showError(ErrorMessages.TitleEmpty); return; }
        showLoader(LoaderMessages.Updating, 3000, 80);
        let i = 1;
        const fd = new FormData();
        document.querySelectorAll('input[name="credentialfile"]').forEach(input => {
            if (input.files[0]) {
                i++;
                fd.append('file' + i, input.files[0]);
                fd.append('fileTitle' + i, input.parentElement.parentElement.querySelector('input[name="credentialtitle"]').value);
            }
        });
        fd.append('TotalFiles', i);
        let client = new HttpClient();
        client.post('/setting/SaveCredentials', fd, function (data) {
            if (data.code == 200) {
                showSuccessReload(data.message);
            }
            else {
                showError(data.message);
            }
        }, function (xhr) {
            showError(ErrorMessages.Error500);
        });


    });

}



function formatState(state) {
    if (!$(state.element).data('iso')) { return state.text; }
    var url = $(state.element).data('iso')
    var $state = $(
        `<div style="display: flex; align-items: center;">
                                        <div><img sytle="display: inline-block;" src="https://flagcdn.com/${url.toLowerCase()}.svg" style="height: 20px;width: 25px;" /></div>
                                        <div style="margin-left: 10px;">${state.text}</div>
                                    </div>`
    );
    return $state;
}
function getCountryCode() {
    return $('#usercountrycode').find('option:selected').data('code')
}
function getCountryId() {
    return $('#usercountrycode').val();
}
function getProfileImg() {
    let fi = document.getElementById("profileimage");
    if (fi.files.length > 0) {
        return fi.files[0];
    }
    return null;
}
function removeCredential(item, id) {
    if (id !== undefined) {
        deleteCredential(id);
    } else {
        item.parentElement.remove();
    }
}
function makeThumb(context) {
     if (context.files.length > 0) {
         if (context.files[0].size > 1024 * 1024 * 7) {
             showError(ErrorMessages.PdfSize);
             context.value = null;
             context.files = [];
             return;
         }


        pdfjsLib.GlobalWorkerOptions.workerSrc = '/lib/pdfjs/build/pdf.worker.js';
        let fr = new FileReader();
        fr.onload = e => {
            try {
                pdfjsLib.getDocument(new Uint8Array(e.target.result)).promise.then(p => {
                    p.getPage(1).then(page => {
                        var viewport = page.getViewport({ scale: 1, });
                        // Support HiDPI-screens.
                        var outputScale = 256 / viewport.width;
                        var width = Math.floor(viewport.width * outputScale);
                        var height = Math.floor(viewport.height * outputScale);

                        const canvas = document.createElement('canvas');
                        canvas.style.width = canvas.width = width;

                        var transform = outputScale !== 1
                            ? [outputScale, 0, 0, outputScale, 0, 0]
                            : null;

                        var renderContext = {
                            canvasContext: canvas.getContext('2d'),
                            transform: transform,
                            viewport: viewport
                        };

                        page.render(renderContext).promise.then(function () {
                            context.previousElementSibling.src = canvas.toDataURL();
                        });
                    });
                });
            } catch (error) {
                console.error(error);
            }
        };
        fr.readAsArrayBuffer(context.files[0]);
    }
}
function createThumbnail() {
    document.querySelectorAll('img[data-role="stored-cred"]').forEach(img => {
        pdfjsLib.getDocument(img.dataset.pdf).promise.then(function (doc) {
            return new Promise(function () {
                return doc.getPage(1).then(makeThumbSplit)
                    .then(function (canvas) {
                        img.src = canvas.toDataURL();
                    });
            });
        });

        img.addEventListener('click', (e) => {
            e.preventDefault();
            window.open(e.target.dataset.pdf, '_blank');
        });
    });
}
function makeThumbSplit(page) {
    // draw page to fit into 96x96 canvas
    var viewport = page.getViewport({ scale: 1 });
    var outputScale = 256 / viewport.width;
    var canvas = document.createElement("canvas");
    canvas.width = 180;
    canvas.height = 210;
    var transform = outputScale !== 1
        ? [outputScale, 0, 0, outputScale, 0, 0]
        : null;
    var renderContext = {
        canvasContext: canvas.getContext('2d'),
        transform: transform,
        viewport: viewport
    };
    return page.render(renderContext).promise.then(function () {
        return canvas;
    });
}
function deleteCredential(id) {
    showLoader(LoaderMessages.Updating, 3000, 80);
    let url = '/setting/DeleteCredential?id=' + (+id);
    let client = new HttpClient();
    client.deleteRequest(url, function (data) {
        hideLoader(1000);
        if (data.code == 200) {
            showSuccessReload(data.message);
        }
        else {
            showError(data.message);
        }
    }, function (xhr) {
        hideLoader(1000);
        console.log(xhr);
        showError(ErrorMessages.Error500);
    });
}