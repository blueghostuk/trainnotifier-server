

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

function loadLocation(stanox, callback) {
    if (!stanox || stanox.length == 0)
        return;

    $.getJSON("http://" + server + ":82/Stanox/", { id: stanox }, function (data) {
        if (!callback)
            callback = loadLocationCallback;
        callback(data);
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