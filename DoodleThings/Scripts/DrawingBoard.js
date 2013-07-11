﻿$(function () {

    var canvas = $("#canvas");
    var hub = $.connection.drawingBoard;

    $("#timer").countdown({ until: '+50s', format: 'MS', onExpiry: endGame });

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
        if (buttonPressed) {
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
        if (buttonPressed && connected) {
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
    $.connection.hub.start()
    .done(function () {
        connected = true;
      
    });

    hub.client.showAlert = function (msg) {
        alert(msg);
    }

    function endGame() {
        hub.server.endGame();
    }
});
