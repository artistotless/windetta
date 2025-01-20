'use strict';

var url = 'https://localhost:20001/api/matches/195da05a-d3ee-4d8b-917c-a77cf7afa906';
var hub = 'https://localhost:20001/matches/195da05a-d3ee-4d8b-917c-a77cf7afa906';
var boardSize = 8;

function getRequestBody(request) {
    return {
        method: 'POST',
        body: JSON.stringify(request),
        credentials: "include",
        headers: {
            "Ticket": ticket,
            "Content-Type": "application/json",
            "Accept": "application/json"
        }
    }
};

const shipColors = {
    '14': 'linear-gradient(90deg, #e3ffe7 0%, #d9e7ff 100%)',  // boat
    '23': 'linear-gradient(90deg, #efd5ff 0%, #515ada 100%)',  // destroyer
    '32': 'linear-gradient(90deg, #1CB5E0 0%, #000851 100%)',  // cruiser
    '41': 'linear-gradient(90deg, #FC466B 0%, #3F5EFB 100%)',  // battleship
    'none': '#f7f7f7'                                          // none
};

var connection = new signalR.HubConnectionBuilder()
    .withUrl(hub)
    .withAutomaticReconnect(new signalR.DefaultReconnectPolicy([0, 2000, 3000, 5000, 10000, 30000]))
    .build();

// GameEvents
connection.on('onOccuredError', function (message) {
    console.log(`Error: ${message}`);
});

connection.on('onGameUpdated', function (e) {
    console.log(`Game updated:`);
    console.log(e);
});

connection.on('onGameFinished', function (e) {
    console.log(`Game finished:`);
    console.log(e);
});

connection.on('onGameCanceled', function (e) {
    console.log(`Game canceled:`);
    console.log(e);
});

// GameMethods
function buildBoard(size, cellsContainer, isOpponentCells) {

    for (let i = 1; i <= size*size; i++)
    {
        cellsContainer.append(isOpponentCells ?
            `<li onclick="dropBomb(${i})" id="opponent-cell-${i}">${i}</li>` :
            `<li id="my-cell-${i}">${i}</li>`)
    }
}

function clearBoard() {
    for (let cellIndex = 1; cellIndex <= boardSize * boardSize; cellIndex++) {
        markCell(cellIndex, false, shipColors['none']);
    }
}

async function dropBomb(cell) {

    console.log('execution dropBomb()...');

    var request = {
        "action": 1,
        "serializedData": `${cell}`
    };

    await fetch(`${url}/actions`, getRequestBody(request))
        .then(async (response) => {
        let result = await response.text();
        console.log(`Drop bomb result: ${result}`);

        if (result === "true" || result === "false") {
            let color = result == "true" ? 'green' : 'red';
            markCell(cell,true,color);
        }
        });
}

function markCell(cellId, isOpponentCell, color) {
    let elementPattern = isOpponentCell ?
        `#opponent-cell-${cellId}` :
        `#my-cell-${cellId}`

    $(elementPattern).css('background', color);
}

async function placeShips(ships) {

    console.log('execution placeShips()...');
    console.log(JSON.stringify(ships));

    var request = {
        "action": 0,
        "serializedData": JSON.stringify(ships)
    };

    let response = await fetch(`${url}/actions`, getRequestBody(request));

    return response.ok;
}

async function getState() {

    console.log('execution getState()...');

    let request = {
        "action": 2,
        "serializedData": "1"
    }

    let result = await fetch(`${url}/actions`, getRequestBody(request));

    let json = await result.json();

    console.log(json)

    return json;
}

document.getElementById('place_ships').addEventListener('click',  async (event) => {

    let shipsRequest = ShipsPlacementRandomizer.Create(boardSize);
    let ships = shipsRequest.Ships;

    if ((await placeShips(shipsRequest))) {
        // if successfully placed
        clearBoard();
        for (let shipIndex = 0; shipIndex < ships.length; shipIndex++) {
            for (let cellIndex = 0; cellIndex < ships[shipIndex].cells.length; cellIndex++) {
                markCell(ships[shipIndex].cells[cellIndex], false, shipColors[`${ships[shipIndex].ship}`]);
            }
        }
    }

    event.preventDefault();
});

document.getElementById('get_state').addEventListener('click', function (event) {

    getState();

    event.preventDefault();
});

class ShipsPlacementRandomizer {
    static _ships = [41, 32, 23, 14];

    static Create(boardSize, startFrom = 1) {
        let shipPlacements = [];
        let freeCells = Array.from({ length: (boardSize * boardSize) - startFrom }, (_, index) => index + startFrom);
        for (let item of this._ships) {
            let cells = this.PlaceInRandomPosition(item, freeCells, boardSize);
            shipPlacements.push({ ship: item, cells });
        }
        return {
            Ships: shipPlacements
        };
    }

    static Size(ship) {
        return Math.floor(ship / 10);
    }

    static Count(ship) {
        return ship % 10;
    }

    static PlaceInRandomPosition(ship, freeCells, boardSize) {
        let result = [];
        for (let i = 0; i < this.Count(ship); i++) {
            let cells;
            do {
                let position = freeCells[Math.floor(Math.random() * freeCells.length)];
                let isVertical = Math.random() < 0.5;
                if (isVertical) {
                    cells = this.GetVerticalCells(position, this.Size(ship), boardSize);
                } else {
                    cells = this.GetHorizontalCells(position, this.Size(ship), boardSize);
                }
            } while (cells[0] == -1|| !cells.every(cell => freeCells.includes(cell)));
            result.push(...cells);
            cells.forEach(cell => {
                let index = freeCells.indexOf(cell);
                if (index > -1) {
                    freeCells.splice(index, 1);
                }
            });
        }
        return result;
    }


static GetVerticalCells(position, shipSize, boardSize) {
    var cells = new Array(shipSize);
    cells[0] = position;
    for (var i = 1; i < cells.length; i++) {
        var cellId = position + boardSize * i;
        if (cellId > boardSize * boardSize) {
            cells[0] = -1;
            return cells;
        }

        cells[i] = cellId;
    }
    return cells;
}



    static GetHorizontalCells(position, shipSize, boardSize) {
    var cells = new Array(shipSize);
    for (var i = 0; i < cells.length; i++) {
        var cellId = position + i;
        if (cellId % boardSize === 0 && i !== cells.length - 1)
        {
            cells[0] = -1;
            return cells;
        }

        cells[i] = cellId;
    }
    return cells;
}


}

async function start() {
    try {

        await connection.start();
        console.log("SignalR Connected.");

        boardSize = (await getState()).boardSize;

        let myCells = $('#cells-1');
        let opponentCells = $('#cells-2');

        buildBoard(boardSize, myCells,false);
        buildBoard(boardSize, opponentCells, true);

        setCellsGridColumns(myCells);
        setCellsGridColumns(opponentCells);

    } catch (err) {
        console.log(err);
    }
};

function setCellsGridColumns(cells) {
    cells.css('grid-template-columns', `repeat(${boardSize}, 50px)`);
}

// Start the connection.
start();