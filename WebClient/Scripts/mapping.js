/// <reference path="jquery-1.8.2.min.js" />

var mapOptions;
var map;
var transitLayer;
var directionsService;
var directionsDisplay;

function preLoadMap() {
    directionsDisplay = new google.maps.DirectionsRenderer();
    directionsService = new google.maps.DirectionsService();
    mapOptions = {
        center: new google.maps.LatLng(52.8382, -2.327815),
        zoom: 8,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

    // only really TFL areas for now
    //transitLayer = new google.maps.TransitLayer();
    //transitLayer.setMap(map);

    var styleArray = [
      {
          featureType: "all",
          stylers: [
            { visibility: "off" }
          ]
      },
      {
          featureType: "landscape",
          stylers: [
              { visibility: "on" },
          ]
      },
      {
          featureType: "administrative",
          stylers: [
              { visibility: "on" },
          ]
      },
      {
          featureType: "water",
          stylers: [
              { visibility: "on" },
          ]
      },
      {
          featureType: "transit.line",
          stylers: [
              { visibility: "on" },
              { lightness: -65 },
              { saturation: 100 },
              { gamma: 1.8 },
              { hue: "#FF7B00" }
          ]
      },
      {
          featureType: "transit.station.rail",
          stylers: [
              { visibility: "on" },
              { lightness: -65 },
              { saturation: 100 },
              { gamma: 1.8 },
              { hue: "#FF7B00" }
          ]
      }
    ];

    map.setOptions({ styles: styleArray });

    directionsDisplay.setMap(map);
}

var markersArray = [];

function showMap(trainId) {
    clearMarkers();
    var classes = $("#" + trainId).attr('class').split(' ');

    for (var i = 0; i < classes.length; i++) {
        if (classes[i] == "info" || classes[i] == trainId)
            continue;

        $.ajax({
            type: "GET",
            url: "http://" + server + ":82/Stanox/",
            data: { id: classes[i] },
            dataType: "json",
            success: function (data) {
                if (data.Lat && data.Lon) {
                    marker = new google.maps.Marker({
                        position: new google.maps.LatLng(data.Lat, data.Lon),
                        icon: {
                            path: google.maps.SymbolPath.CIRCLE,
                            scale: 3
                        },
                        draggable: false,
                        map: map
                    });
                    markersArray.push(marker);
                }
            },
            async: false
        });
    }

    var latlngbounds = new google.maps.LatLngBounds();
    for(i in markersArray)
        latlngbounds.extend(markersArray[i].position);
    map.setCenter(latlngbounds.getCenter());
    map.fitBounds(latlngbounds);

    /*var request = {
        origin: markersArray[0].position,
        destination: markersArray[markersArray.length - 1].position,
        waypoints: [],
        provideRouteAlternatives: false,
        travelMode: google.maps.TravelMode.TRANSIT,
        unitSystem: google.maps.UnitSystem.IMPERIAL
    };

    for (i in markersArray) {
        if (i == 0 || (i == markersArray.length -1))
            continue;

        request.waypoints.push({
            location: markersArray[i].position,
            stopover: false
        });
    }

    directionsService.route(request, function (result, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(result);
        }
    });*/
}

function clearMarkers() {
    if (markersArray) {
        for (i in markersArray) {
            markersArray[i].setMap(null);
        }
        markersArray.length = 0;
    }
}