"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/ConversationalHub").build();

document.getElementById("sendButton").disabled = true;
document.getElementById("ttsButton").disabled = true;


connection.on("Log", function (user, message, utcTime) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var hasSpeech = user.indexOf(": SpeechService") > 0;

    var tds = (hasSpeech ?
        `<tr><td>${user}</td><td>${utcTime}</td><td>${msg}<br/><Button onclick="PlayBase64Audio('${msg}')">Play Speech</Button></td></tr>`
        : `<tr><td>${user}</td><td>${utcTime}</td><td>${msg}</td></tr>`);
    $("#logDisplay").append($('<tbody>').append(tds));
});
connection.on("Text2SpeechResponse", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var tds = `<tr><td>${user}</td><td>&nbsp;</td><td>${msg}<br/><Button onclick="PlayBase64Audio('${msg}')">Play Speech</Button></td></tr>`;
    $("#logDisplay").append($('<tbody>').append(tds));
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    document.getElementById("ttsButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
document.getElementById("ttsButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
        connection.invoke("GetText2Speech", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});