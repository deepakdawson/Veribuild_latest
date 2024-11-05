'use strict'
import HttpClient from '../utils/HttpClient.js';
import Validation from '../utils/validation.js';

let currentlat = 40.749933;
let currentlng = -73.98633;

document.addEventListener('DOMContentLoaded', function () {
    window.initMap = initMap;
    initMap();
    mapUi();
});

function initMap() {
    const map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: currentlat, lng: currentlng },
        zoom: 15,
        mapTypeId: "satellite"
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


    document.querySelector('#btn-saveproperty').addEventListener('click', function () {

    });
}