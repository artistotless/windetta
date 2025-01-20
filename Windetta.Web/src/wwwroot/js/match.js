
function getScriptsHeader(match) {
    return `<script>
    var ticket = '${match.ticket}';
    var matchId = '${match.id}';
    </script>`;
}

function getUrlParams() {
    return new URLSearchParams(window.location.search);
}

async function fetchGameUI(playerId) {

    fetch(`${mainUrl}/api/ongoingMatches`, {
        method: 'GET',
        headers: {
            "Accept": "text/html"
        }
    }).then(async (response) => {

        var match = await response.json()
        await setView(match);
    });
}

async function setView(currentMatch) {

   await fetch(`${currentMatch.endpoint}api/views`, {
        method: 'Get',
        headers: {
            "Ticket": currentMatch.ticket,
            "Content-Type": "application/json",
            "Accept": "application/json"
        }
    }).then(async (response) => {
        $('#gameContent').append(`${getScriptsHeader(currentMatch)}${await response.text() }`);
        });
}

fetchGameUI(playerId);