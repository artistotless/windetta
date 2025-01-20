'use strict';

document.getElementById('room_index').value = 0;
document.getElementById('lobby_id').value = "195da05a-d3ee-4d8b-917c-a77cf7afa906";

var connection = new signalR.HubConnectionBuilder()
    .withUrl(mainHubUrl, {
        headers: { "X-UserId": setUserId() },
        transport: signalR.HttpTransportType.LongPolling,
        accessTokenFactory: () => {
            return $.get('/tokens/accessToken')
                .done(function (token) { return token; })
        }
    })
    .withAutomaticReconnect(new signalR.DefaultReconnectPolicy(
        [0, 2000, 3000, 5000, 10000, 30000])).build();

function setUserId() {

    let userId = generateUUID();
    document.getElementById('user_id').value = userId;

    return userId;
}

function getUserId() {

    return document.getElementById('user_id').value;
}

function disableButtons(disabled) {
    document.getElementById('join_lobby').disabled = disabled;
    document.getElementById('leave_lobby').disabled = disabled;
    document.getElementById('create_lobby').disabled = disabled;
    document.getElementById('get_lobbies').disabled = disabled;
}

//Disable the send button until connection is established.
disableButtons(true);

connection.on('onOccuredError', function (message) {
    console.log(`Error: ${message}`);
});

connection.on('onReceivedLobbies', function (lobbies) {
    console.log('Lobbies:');
    console.log(lobbies);
});

connection.on('onDeletedLobby', function (lobbyId) {
    console.log(`Lobby deleted: ${lobbyId}`);
});

connection.on('onReadyLobby', function (lobbyId) {
    console.log(`Lobby ready: ${lobbyId}`);
});

connection.on('onUpdateddLobby', function (loby) {
    console.log('Lobby updated:');
    console.log(loby);
});

connection.on('onAddedLobby', function (lobby) {
    console.log('Lobby added:');
    console.log(lobby);
});

connection.on('onServerFound', function () {
    console.log('ServerFound');
});

connection.on('onMatchCanceled', function () {
    console.log('MatchCanceled');
});

connection.on('onReadyToConnect', function (data) {
    console.log('ReadyToConnect!');
    console.log(`https://localhost:7084/matches/${data.matchId}?playerId=${getUserId()}`);
});

connection.start().then(function () {
    disableButtons(false);
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById('join_lobby').addEventListener('click', function (event) {

    var lobbyId = document.getElementById('lobby_id').value;
    var roomId = document.getElementById('room_index').value;

    $.ajax({
        url: `${lobbiesUrl}/${lobbyId}/rooms/${roomId}`,
        method: 'POST',
        headers: { "X-UserId": getUserId() },
    })
        .then(function (data) {
            console.log(data)
        });

    event.preventDefault();
});

document.getElementById('leave_lobby').addEventListener('click', function (event) {

    var lobbyId = document.getElementById('lobby_id').value;
    var roomId = document.getElementById('room_index').value;

    $.ajax({
        url: `${lobbiesUrl}/${lobbyId}/rooms/${roomId}`,
        method: 'DELETE',
        headers: { "X-UserId": getUserId() },
    })
        .then(function (data) {
            console.log(data)
        });

    event.preventDefault();
});

document.getElementById('get_lobbies').addEventListener('click', function (event) {

    $.ajax({
        url: lobbiesUrl, method: 'GET'
    })
        .then(function (data) {
            console.log(data)
        });

    event.preventDefault();
});

document.getElementById('create_lobby').addEventListener('click', function (event) {

    let request =
    {
        "gameId": "b15739de-8d5b-4759-a20c-75389b14ecb8",
        "bet": {
            "currencyId": 1,
            "amount": 1000
        },
        "private": false,
        "autoReadyStrategy": null,
        "properties": {"boardSize": '5'},
        "autoDisposeStrategy": null
    };

    $.ajax({
        url: lobbiesUrl,
        method: 'POST',
        headers: {
            "X-UserId": getUserId(),
            "Content-Type": "application/json; odata=verbose"
        },
        data: JSON.stringify(request),
        processData: false,
    })
        .then(function (data) {
            console.log(data)
        });

    event.preventDefault();
});

function generateUUID() { // Public Domain/MIT
    var d = new Date().getTime();//Timestamp
    var d2 = ((typeof performance !== 'undefined') && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16;//random number between 0 and 16
        if (d > 0) {//Use timestamp until depleted
            r = (d + r) % 16 | 0;
            d = Math.floor(d / 16);
        } else {//Use microseconds since page-load if supported
            r = (d2 + r) % 16 | 0;
            d2 = Math.floor(d2 / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}