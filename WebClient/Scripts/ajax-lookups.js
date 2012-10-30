

function fetchStanox(stanox) {
    if (!stanox || stanox.length == 0)
        return;

    $.getJSON("http://" + server + ":82/Stanox/", { id: stanox }, function (data) {
        var html = "";
        if (data.StationName) {
            html = data.StationName;
        } else {
            html = data.Tiploc;
        }
        if (data.CRS) {
            html += "(" + data.CRS + ")";
        }
        $(".stanox-" + stanox).html(html);
        $(".stanox-" + stanox).tooltip({
            title: stanox
        });
    });
}

function loadLocation(stanox) {
    if (!stanox || stanox.length == 0)
        return;

    currentLocation.locationStanox(stanox);
    $.getJSON("http://" + server + ":82/Stanox/", { id: stanox }, function (data) {
        currentLocation.locationTiploc(data.Tiploc);
        currentLocation.locationDescription(data.Description);
        currentLocation.locationCRS(data.CRS);
        currentLocation.stationName(data.StationName);
        currentLocation.stationLocation(data.Lat + ", " + data.Lon);
        if (data.Lat && data.Lon) {
            $("#station-loc").attr("src",
                "http://maps.googleapis.com/maps/api/staticmap?center=" + data.Lat + "," + data.Lon +
                "5&zoom=14&size=310x310&sensor=false&style=feature:transit.station.rail" +
                "&key=" + apiKey);
            $("#station-loc").show();
        } else {
            $("#station-loc").hide();
        }
    });
}

function filter(location) {
    if (!location || location.length == 0) {
        currentFilter = '';
        $("#table-trains tbody tr").show();
        return;
    }

    $.getJSON("http://" + server + ":82/Station/", { id: location }, function (data) {
        currentFilter = data.Name;
        $("#table-trains tbody tr").hide();
        $("#table-trains tbody tr." + data.Name).show();
    });
}       
function preFilterLocation(location) {
    if (!location || location.length == 0) {
        sendPreFilter('');
    } else {
        $.getJSON("http://" + server + ":82/Station/", { id: location }, function (data) {
            sendPreFilter(data.Name);
        });
    }
}

function preLoadStations() {
    $.getJSON("http://" + server + ":82/Station/", null, function (results) {
        if (!results || results.length == 0)
            return;

        _locations = Array();

        for (var i = 0; i < results.length; i++) {
            if (results[i] && results[i].length > 0)
                _locations.push(results[i]);
        }

        $("#filter-location").typeahead({
            source: _locations,
            updater: function (item) {
                filter(item.substring(0, (item.indexOf('(') - 1)));
                return item;
            }
        });

        $("#filter-pre-location").typeahead({
            source: _locations,
            update: function (item) {
                sendPreFilter(item.substring(0, (item.indexOf('(') - 1)));
                return item;
            }
        });
    });
}