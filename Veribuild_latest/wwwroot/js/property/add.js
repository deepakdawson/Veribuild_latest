'use strict'
import HttpClient from '../utils/HttpClient.js';

let currentlat = 40.749933;
let currentlng = -73.98633;

let images = [];
let floorPdfs = [];
let videoThumb = 0;
let thumbIndex = 16325;

document.addEventListener('DOMContentLoaded', function () {
    window.initMap = initMap;
    window.removeUrl = removeUrl;
    window.previewVideoUrl = previewVideoUrl;
    window.inputFocusSelect = inputFocusSelect;
    window.backspaceRemove = backspaceRemove;
    initMap();
    mapUi();
});

function initMap() {
    const map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: currentlat, lng: currentlng },
        zoom: 15,
        mapTypeId: "roadmap"
    });
    map.setTilt(45);
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            (position) => {
                currentlat = position.coords.latitude;
                currentlng = position.coords.longitude;
                const pos = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude,
                };
                map.setCenter(pos);
            }
        );
    }
    const autocomplete = new google.maps.places.Autocomplete(document.getElementById("propertyaddress"));
    autocomplete.bindTo("bounds", map);
    //const infowindow = new google.maps.InfoWindow();
    //const infowindowContent = document.getElementById("infowindow-content");
    //infowindow.setContent(infowindowContent);
    //const marker = new google.maps.marker.AdvancedMarkerElement({
    //    map,
    //    position: { lat: currentlat, lng: currentlng },
    //    title: 'Uluru',
    //});

    const marker = new google.maps.Marker({
        map,
        anchorPoint: new google.maps.Point(currentlat, currentlng),
    });

    autocomplete.addListener("place_changed", () => {
        ///infowindow.close();
        marker.setVisible(false);
        const place = autocomplete.getPlace();
        if (!place.geometry || !place.geometry.location) {
            window.alert("No details available for input: '" + place.name + "'");
            return;
        }

        currentlat = place.geometry.location.lat();
        currentlng = place.geometry.location.lng();
        //document.getElementById("property_lat").value = place.geometry.location.lat();
        //document.getElementById("property_long").value = place.geometry.location.lng();
        //const fd = new FormData();
        //fd.append('address', document.getElementById('propertyaddress').value);
        //$.ajax({
        //    url: '/properties/matchProperty',
        //    data: fd,
        //    type: 'POST',
        //    processData: false,
        //    contentType: false,
        //    async: true,
        //    success: function (data) {
        //        if (data.code == 200) {
        //            $("#propertyunit").val(parseFloat(data.message) + 1);
        //        }
        //    }
        //})
        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(17);
        }
        marker.setPosition(place.geometry.location);
        marker.setVisible(true);
        //infowindowContent.children["place-name"].textContent = place.name;
        //infowindowContent.children["place-address"].textContent = place.formatted_address;
        //infowindow.open(map, marker);
    });
    autocomplete.bindTo("bounds", map);
}

function mapUi() {
    document.querySelector('#featureimageopener').addEventListener('click', function (e) {
        e.preventDefault();
        this.nextElementSibling?.click();
    });
    document.querySelector('#featureimage').addEventListener('change', function () {
        if (this.files.length > 0) {
            const ref = this;
            let fr = new FileReader();
            fr.onload = (e) => {
                console.log(e.target.result);
                ref.parentElement.querySelector('img').src = e.target.result;
            };
            fr.readAsDataURL(this.files[0]);
        }
    })
    document.querySelectorAll('img[data-role="other-image"]').forEach(img => {
        img.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector('#otherimageselector').click();
        });
    });
    document.querySelector('#btnUploadphotos').addEventListener('click', function () {
        document.querySelector('#otherimageselector').click();
    });
    document.querySelector('#otherimageselector').addEventListener('change', function (e) {
        if (this.files.length > 0) {
            Array.from(this.files).forEach(file => {
                if (images.length == 0) {
                    document.querySelector('#otherimagescontainer').innerHTML = '';
                }
                images.push(file);
                let fr = new FileReader();
                fr.onload = e => {
                    let img = `<div class="image-box">
                                <label class="w-100" for="img2">
                                    <img src="${e.target.result}" class="img-fluid cursor-point" data-role="other-image" />
                                </label>
                            </div>`;
                    document.querySelector('#otherimagescontainer').insertAdjacentHTML('beforeend', img);
                };
                fr.readAsDataURL(file);
            });
        }
    });
    document.querySelectorAll('img[data-role="pdf-select"]').forEach(img => {
        img.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector('#floorplanpdfselector').click();
        });
    });
    document.querySelector('#btnselectfloorplanpdf').addEventListener('click', function (e) {
        e.preventDefault();
        document.querySelector('#floorplanpdfselector').click();
    });
    document.querySelector('#floorplanpdfselector').addEventListener('change', function (e) {
        if (this.files.length > 0) {
            Array.from(this.files).forEach(file => {
                if (floorPdfs.length == 0) {
                    document.querySelector('#floorplanpdfcontainer').innerHTML = '';
                }
                createPDFThumbnails(file);
            });
        }
    });
    document.querySelector('#addmorevimeourl').addEventListener('click', function (e) {
        e.preventDefault();
        thumbIndex++;
        var inputDiv = `<div class="mt-3 position-relative"><span class="btnClose" onclick="removeUrl(this)"><i class="fa-solid fa-xmark"></i></span><input type="text" class="form-control" placeholder="Enter vimeo URL" onkeydown="backspaceRemove(event)" oninput="previewVideoUrl(this, 'vimeo')" data-id="input${thumbIndex}" onfocus="inputFocusSelect(this)"/></div>`;
        document.querySelector('#vimeoBoxURL').insertAdjacentHTML('beforeend', inputDiv);
    })
    document.querySelector('#addmoreyoutubeurl').addEventListener('click', function (e) {
        e.preventDefault();
        thumbIndex++;
        var inputDiv = `<div class="mt-3 position-relative"><span class="btnClose" onclick="removeUrl(this)"><i class="fa-solid fa-xmark"></i></span><input type="text" class="form-control" placeholder="Enter youtube URL" onkeydown="backspaceRemove(event)" oninput="previewVideoUrl(this, 'youtube')" data-id="input${thumbIndex}" onfocus="inputFocusSelect(this)"/></div>`;
        document.querySelector('#youtubeBoxURL').insertAdjacentHTML('beforeend', inputDiv);
    });
    document.querySelector('#viodeUpload').addEventListener('change', function (e) {
        document.querySelector('#uploadfromcomputerbox')?.remove();
        let html = `<div class="video-box" id="uploadfromcomputerbox">
                    <video src="${URL.createObjectURL(this.files[0])}" controls crossorigin="anonymous" style="width:100%; height:100%"></video>
                </div>`;
        if (videoThumb == 0) {
            document.querySelector('#videothumbcontainer').innerHTML = '';
        }
        videoThumb++;
        document.querySelector('#videothumbcontainer').insertAdjacentHTML('beforeend', html);
    });


    document.querySelector('#btn-saveproperty').addEventListener('click', function (e) {
        e.preventDefault();
        const ref = this;
        Swal.fire({
            title: 'Are you sure?',
            text: "Once saved, you will not be able to edit this property!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#08994B',
            cancelButtonColor: '#e8e8e8',
            confirmButtonText: 'OK'
        }).then(function (result) {
            if (result.isConfirmed) {
                if (!validate()) { return; }
                showLoader(LoaderMessages.Saving, 5000, 80);
                const fd = new FormData();
                fd.append('Address', document.getElementById('propertyaddress').value);
                fd.append('Lattitude', currentlat);
                fd.append('Longitude', currentlng);
                fd.append('Unit', document.getElementById('propertyunit').value);
                fd.append('Area', document.getElementById('propertyarea').value);
                fd.append('PropertyTypeId', document.getElementById('propertyTypes').value);
                fd.append('Bedroom', document.getElementById('propertyrooms').value);
                fd.append('EasyNumber', document.getElementById('propertyeasynumber').value);

                let vimeoUrls = [];
                let youtubeUrls = [];

                document.querySelectorAll('#vimeoBoxURL input').forEach(inp => {
                    if (isValidUrl(inp.value)) {
                        vimeoUrls.push(inp.value);
                    }
                });

                document.querySelectorAll('#youtubeBoxURL input').forEach(inp => {
                    if (isValidUrl(inp.value)) {
                        youtubeUrls.push(inp.value);
                    }
                });
                fd.append('MainImage', document.querySelector('#featureimage').files[0]);
                fd.append('YoutubeUrl', JSON.stringify(youtubeUrls));
                fd.append('VimeoeUrl', JSON.stringify(vimeoUrls));
                if (document.querySelector('#viodeUpload').files.length > 0) {
                    fd.append('Video', document.querySelector('#viodeUpload').files[0]);
                }
                images.forEach(img => {
                    fd.append('ImageFiles', img);
                });

                floorPdfs.forEach(file => {
                    fd.append('FloorPdfs', file);
                });
                let client = new HttpClient();
                client.post('/properties/add', fd, function (data) {
                    if (data.code == 200) {
                        showSuccessReload(data.message);
                    }
                    else {
                        showError(data.message);
                    }
                }, function (xhr) {
                    showError(ErrorMessages.Error500);
                }, ref);


            }
        });
    });
}
function backspaceRemove(event) {
    if (event.code == 'Backspace') {
        event.target.value = '';
        document.getElementById(`${event.target.getAttribute('data-id')}`)?.remove();
    }
}
function inputFocusSelect(context) {
    context.select();
}
function removeUrl(item) {
	item.parentElement.remove();
}
function previewVideoUrl(context, plateform = '') {
    if (context.value.length == 0) {
        document.getElementById(`${context.getAttribute('data-id')}`)?.remove();
    }
    if (plateform == 'youtube') {
        try {
            let url = new URL(context.value);
            if (url.searchParams.get('v')) {
                let videoUrl = 'https://www.youtube.com/embed/' + url.searchParams.get('v');
                let box = `<div class="video-box" id="${context.getAttribute('data-id')}">
                            <iframe src="${videoUrl}" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen style="width:100%; height:auto"></iframe>
                        </div>`;
                if (videoThumb == 0) {
                    document.querySelector('#videothumbcontainer').innerHTML = '';
                }
                videoThumb++;
                document.querySelector('#videothumbcontainer').insertAdjacentHTML('beforeend', box);
            }
        } catch (e) {
            console.error(e);
        }
    } else {
        try {
            let url = new URL(context.value);
            let videoUrl = 'https://player.vimeo.com/video/' + url.toString().match(/vimeo\.com.*(\/|\/\/)(.{0,})/).pop();
            let box = `<div class="video-box" id="${context.getAttribute('data-id')}">
                            <iframe src="${videoUrl}" title="Vimeo video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen style="width:100%; height:auto"></iframe>
                        </div>`;
            if (videoThumb == 0) {
                document.querySelector('#videothumbcontainer').innerHTML = '';
            }
            videoThumb++;
            document.querySelector('#videothumbcontainer').insertAdjacentHTML('beforeend', box);
        } catch (e) {
            console.error(e);
        }
    }
}
var createPDFThumbnails = function (file) {
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
                        let html = `<div class="document-box">
                                        <label class="w-100" for="doc1">
                                            <img class="img-fluid" src="${canvas.toDataURL()}" />
                                        </label>
                                    </div>`;
                        document.querySelector('#floorplanpdfcontainer').insertAdjacentHTML('beforeend', html);
                        floorPdfs.push(file);
                    });
                });
            });
        } catch (error) {
            console.error(error);
        }
    };
    fr.readAsArrayBuffer(file);
};
function validate() {
    if (document.getElementById('propertyaddress').value == '') {
        showError(ErrorMessages.Mandatory);
        return false;
    }
    if (document.getElementById('propertyunit').value == '') {
        showError(ErrorMessages.Mandatory);
        return false;
    }
    if (document.getElementById('propertyarea').value == '') {
        showError(ErrorMessages.Mandatory);
        return false;
    }
    if (document.getElementById('propertyTypes').value == '') {
        showError(ErrorMessages.Mandatory);
        return false;
    }
    if (document.getElementById('propertyrooms').value == '') {
        showError(ErrorMessages.Mandatory);
        return false;
    }
    if (document.querySelector('#featureimage').files.length == 0) {
        showError(ErrorMessages.FeatureImage);
        return false;
    }

    if (floorPdfs.length == 0) {
        showError(ErrorMessages.PropertyFloorPdf);
        return false;
    }
    return true;
}