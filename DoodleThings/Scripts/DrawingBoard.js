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
            setPoint(e.offsetX, e.offsetY,'black');
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
        $("#outputArea").text("We are finding another player for you. Should be anytime now.");
    }

    hub.client.startGame = function (question) {
        $("#timer").countdown({ until: '+50s', format: 'MS', onExpiry: gameTimeout });
        if (hub.state.Drawer == true) {
            $("#outputArea").text("You need to draw the following: " + question);
            $('#send-answer').hide();
        }
        else {
            $("#outputArea").text("You need to guess what the other player is drawing and enter your answer in the box below.");
            $("#color").hide();
            $("#colorLabel").hide();
            $("#clear").hide();
        }
        alert('Game has started');
    }

    $('#send-answer').submit(function () {
        var userAnswer = $('#user-answer').val();

        hub.server.submitAnswer(userAnswer);

        $('#new-message').val('');
        $('#new-message').focus();
    });

    function endGame(status) {
        if (status == "Correct") {
            alert("Successfully Guessed! Game Over!");
        }
        else if (status = "Incorrect") {
            alert("Incorrect Answer! Game Over!");
        }
    }

    function gameTimeout() {
        alert("Time Over!!");
        hub.server.gameTimeout();
    }

    hub.client.setDrawerState = function (value) {
        hub.state.Drawer = value;
    }

    hub.client.setGameId = function (gameId) {
        hub.state.GameId = gameId;
    }

    hub.client.write = function (msg) {
        $("#outputArea").text(msg);
    }
});
