var ws;

function connect() {
    $("#btn_Connect").attr("disabled", true);
    $("#btn_Disconnect").attr("disabled", false);

    ws = new WebSocket("ws://" + server + ":81");
    ws.onopen = function () {
        setStatus("Connected");

        $("#status").removeClass("btn-warning");
        $("#status").removeClass("btn-info");
        $("#status").addClass("btn-success");

        var preFilter = $("#filter-pre-location").val();
        if (preFilter && preFilter.length > 0) {
            preFilterLocation(preFilter.substring(0, (preFilter.indexOf('(') - 1)));
        }
    };
    ws.onmessage = function (msg) {
        var data = jQuery.parseJSON(msg.data);
        console.debug(data);
        setStatus("Received " + data.length + " messages at " + new Date(Date.now()).toLocaleString());

        $("#status").removeClass("btn-warning");
        $("#status").removeClass("btn-success");
        $("#status").addClass("btn-info");

        for (var i = 0; i < data.length; i++) {
            var message = data[i]; //.body;

            var existing = $("#" + message.train_id).length == 1;
            var cls = "";
            if (existing) {
                $("." + message.train_id).addClass(message.loc_stanox);
                cls = $("#" + message.train_id).attr('class');
                cls = cls.replace(" info", "");
                parent = message.train_id;
            } else {
                parent = null;
                cls = message.loc_stanox + " " + message.train_id;
            }

            var style = "";
            if (currentFilter &&
                currentFilter.length > 0 &&
                cls.indexOf(currentFilter) == -1) {
                style = "display:none;";
            }

            var date = new Date(new Number(message.actual_timestamp));
            var html = "";

            if (!existing) {
                html += "<tr class=\"" + cls + " info\" style=\"" + style + "\" id=\"" + message.train_id + "\">";
                html += "<td><a href=\"#\" onclick=\"filterTrain('" + message.train_id + "');\">" + message.train_id + "</a></td>";
                html += "<td>" + message.train_service_code + "</td>";
                html += "<td colspan=\"5\"><a href=\"#\" data-toggle=\"modal\" data-target=\"#mapModal\" onclick=\"showMap('" + message.train_id + "');\">View Map</button></td>";
                html += "</tr>";
            }

            html += "<tr class=\"" + cls + "\" style=\"" + style + "\" data-timestamp=\"" + message.actual_timestamp + "\">";
            html += "<td colspan=\"2\">&nbsp;&nbsp;</td>";
            html += "<td>" + message.event_type + "</td>";
            html += "<td>" + padTime(date.getUTCHours()) + ":" + padTime(date.getUTCMinutes()) + ":" + padTime(date.getUTCSeconds()) + "</td>";
            html += "<td>" + message.direction_ind + "</td>";
            html += "<td>" + message.platform + "</td>";
            html += "<td><a href=\"#\" class=\"stanox-" + message.loc_stanox + "\" onclick=\"loadLocation('" + message.loc_stanox + "')\">" + message.loc_stanox + "</a></td>";

            html += "</tr>";

            if (message.train_terminated && message.train_terminated == "true") {
                html += "<tr class=\"" + cls + " " + message.train_id + " warning\"  style=\"" + style + "\"><td colspan=\"7\">Terminated</td></tr>";
            }

            addMessage(html, parent);

            sortTrainId(message.train_id);

            fetchStanox(message.loc_stanox);
        }
    };
    ws.onclose = function () {
        setStatus("Disconnected");
        $("#status").removeClass("btn-success");
        $("#status").removeClass("btn-info");
        $("#status").addClass("btn-warning");
        $("#btn_Connect").attr("disabled", false);
        $("#btn_Disconnect").attr("disabled", true);
    };
}

function disconnect() {
    $("#btn_Connect").attr("disabled", false);
    $("#btn_Disconnect").attr("disabled", true);

    ws.close();
    setStatus("Closed");
    $("#status").removeClass("btn-success");
    $("#status").removeClass("btn-info");
    $("#status").addClass("btn-warning");
}