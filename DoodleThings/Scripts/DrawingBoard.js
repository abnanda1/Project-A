$(function () {

    function getSecurityHeaders() {
        var accessToken = sessionStorage["accessToken"] || localStorage["accessToken"];

        if (accessToken) {
            return accessToken;
        }
    }

    var canvas = $("#canvas");
    var hub = $.connection.drawingBoard;

    hub.state.color = $("#color").val(); // Accessible from server 
    var connected = false;

    var buttonPressed = false;

    canvas
    .mousedown(function () {
        buttonPressed = true;
    })
    .mouseup(function () {
        buttonPressed = false;
    })
    .mousemove(function (e) {
        if (buttonPressed && hub.state.Drawer) {
            setPoint(e.offsetX, e.offsetY, $("#color").val());
        }
    });

    var ctx = canvas[0].getContext("2d");

    function setPoint(x, y, color) {
        ctx.fillStyle = color;
        ctx.beginPath();
        ctx.arc(x, y, 2, 0, Math.PI * 2);
        ctx.fill();
    }

    function clearPoints() {
        ctx.clearRect(0, 0, canvas.width(), canvas.height());
    }

    $("#clear").click(function () {
        clearPoints();
    });

    // UI events 
    $("#color").change(function () {
        hub.state.color = $(this).val();
    });

    canvas.mousemove(function (e) {
        if (buttonPressed && connected && hub.state.Drawer) {
            hub.server.broadcastPoint(e.offsetX, e.offsetY);
        }
    });

    $("#clear").click(function () {
        if (connected) {
            hub.server.broadcastClear();
        }
    });

    // Event handlers 
    hub.client.clear = function () {
        clearPoints();
    };
    hub.client.drawPoint = function (x, y, color) {
        setPoint(x, y, color);
    };

    // Voila! 
    // hub.state.Drawer = false;
    $.connection.hub.qs = 'token=' + getSecurityHeaders();
    $.connection.hub.start()
    .done(function () {
        connected = true;
    });

    hub.client.updateState = function () {
        hub.server.updateState();
    }

    hub.client.showAlert = function (msg) {
        alert(msg);
    }

    hub.client.waitForPlayer = function () {
        hub.state.Drawer = false;
    }

    hub.client.startGame = function () {
        $("#timer").countdown({ until: '+50s', format: 'MS', onExpiry: endGame });
        alert('Game has started');
    }

    function endGame() {
        hub.server.endGame();
    }

    hub.client.setDrawerState = function (value) {
        hub.client.Drawer = value;
    }

    hub.client.setGameId = function (gameId) {
        hub.client.GameId = gameId;
    }
});
