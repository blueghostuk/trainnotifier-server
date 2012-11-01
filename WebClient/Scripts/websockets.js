var ws;

function connect() {
    $(".btn_connect").attr("disabled", true);
    $(".btn_disconnect").attr("disabled", false);

    ws = new WebSocket("ws://" + server + ":81");
    ws.onopen = function () {
        setStatus("Connected");

        $("#status").removeClass("btn-warning");
        $("#status").removeClass("btn-info");
        $("#status").addClass("btn-success");
        $(".btn_connect").attr("disabled", true);

        var preFilter = $("#filter-pre-location").val();
        if (preFilter && preFilter.length > 0) {
            preFilterLocation(preFilter.substring(0, (preFilter.indexOf('(') - 1)));
        }

        try{
            wsOpenCommand();
        } catch (err) { }
    };
    ws.onclose = function () {
        setStatus("Disconnected");
        $("#status").removeClass("btn-success");
        $("#status").removeClass("btn-info");
        $("#status").addClass("btn-warning");
        $(".btn_connect").attr("disabled", false);
        $(".btn_disconnect").attr("disabled", true);
    };
}

function disconnect() {
    $(".btn_connect").attr("disabled", false);
    $(".btn_disconnect").attr("disabled", true);

    ws.close();
    setStatus("Closed");
    $("#status").removeClass("btn-success");
    $("#status").removeClass("btn-info");
    $("#status").addClass("btn-warning");
}