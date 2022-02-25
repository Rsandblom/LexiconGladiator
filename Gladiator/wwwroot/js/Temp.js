"use strict";

var _inFight = false;
var _fightId, _fighterId;
document.getElementById("attackButton").disabled = true;
document.getElementById("attackButton").value = "Not started!";
var challengerBaseHp;
var adversaryBaseHp;

// Setup connection
const connection = new signalR.HubConnectionBuilder()
	.withUrl("https://HOST:PORT/fightHub".replace("HOST", location.hostname).replace("PORT", location.port))
	.withAutomaticReconnect([0, 1000, 1000, 2000, 3000, 5000, 8000, 13000, 21000, 34000]) // <3 Fibonacci
	.configureLogging(signalR.LogLevel.Information)
	.build();

// Connect
async function start() {
	try {
		await connection.start();
		console.assert(connection.state === signalR.HubConnectionState.Connected);
		console.log("SignalR Connected.");
	}
	catch (err) {
		console.assert(connection.state === signalR.HubConnectionState.Disconnected);
		console.log(err);
		setTimeout(() => start(), 5000);
	}

};

// Connection lost, reconnecting.
connection.onreconnecting(error => {
	console.assert(connection.state === signalR.HubConnectionState.Reconnecting);

	// Do something?

});


// Connection is back up
connection.onreconnected(connectionId => {
	console.assert(connection.state === signalR.HubConnectionState.Connected);

	// Do something?

	if (_fightId !== null && _fighterId !== null) {
		connection.invoke("JoinFight", _fightId, _fighterId).catch(function (err) {
			return console.error(err.toString());
		});
	};
});

// Connection closed
connection.onclose(error => {
	console.assert(connection.state === signalR.HubConnectionState.Disconnected);

	// Do something?

});


connection.on("JoinedFight", (fightId, fighterId) => {
	_fightId = fightId;
	_fighterId = fighterId;
	document.getElementById("message").textContent = "Joined fight " + fightId;
});



connection.on("FightData", (figthId, data) => {
	challengerBaseHp = data.you.currentHpPercent;
	adversaryBaseHp = data.opponent.currentHpPercent;
	document.getElementById("Name1").textContent = data.you.name;
	document.getElementById("Str1").textContent = data.you.strength;
	document.getElementById("Def1").textContent = data.you.defence;
	document.getElementById("XP1").textContent = data.you.xp;
	document.getElementById("HP1").style.width = data.you.currentHpPercent + "%";
	document.getElementById("HP1").textContent = `HP: ${data.you.currentHp} (${data.you.startHp})`
	document.getElementById("Name2").textContent = data.opponent.name;
	document.getElementById("Str2").textContent = data.opponent.strength;
	document.getElementById("Def2").textContent = data.opponent.defence;
	document.getElementById("XP2").textContent = data.opponent.xp;
	document.getElementById("HP2").style.width = data.opponent.currentHpPercent + "%";
	document.getElementById("HP2").textContent = `HP: ${data.opponent.currentHp} (${data.opponent.startHp})`
	document.getElementById("message").textContent = `FightId: ${data.fightId}, ArenaName: ${data.arena.name}`;
});



connection.on("RoundData", (fightId, data) => {
	document.getElementById("HP1").style.width = data.you.currentHpPercent + "%";
	document.getElementById("HP1").textContent = `HP: ${data.you.currentHp} (${data.you.startHp})`
	document.getElementById("HP2").style.width = data.opponent.currentHpPercent + "%";
	document.getElementById("HP2").textContent = `HP: ${data.opponent.currentHp} (${data.opponent.startHp})`
	document.getElementById("round").textContent = `Round ${data.roundNo + 1}`;
	if (data.you.isAttacker) {
		document.getElementById("attackButton").value = "Waiting ...";
	}
});

connection.on("Attacked", (fightId) => {
	document.getElementById("attackButton").value = "Defending!";
});


connection.on("FightResults", (fightId, fightResultData) => {
	console.log(`FightResults ${fightId}`);
	document.getElementById("round").textContent = `Rounds: ${fightResultData.noRounds}`;
});

connection.on("AttackAllowed", (fightId, isAllowed) => {
	var attackButton = document.getElementById("attackButton");
	attackButton.disabled = !isAllowed;
	if (isAllowed) {
		attackButton.value = "Attack!";
	}
});

connection.on("FightStarted", (fightId) => {
	console.log(`FightStarted ${fightId}`);
	document.getElementById("attackButton").value = "Fight started!"
	document.getElementById("round").textContent = `Round 1`;
});


connection.on("FightEnded", (fightId) => {
	_inFight = false;
	console.log(`FightEnded ${fightId}`);
	document.getElementById("attackButton").disabled = true;
	document.getElementById("attackButton").value = "Fight Over!";
});


connection.on("Error", (errorMessage) => {
	console.error(errorMessage);
});

connection.on("FighterHighscores", (list) => {
	console.log(list);
});

connection.on("UserHighscores", (list) => {
	console.log(list);
});


connection.on("FightChallenge", (fightId, fightChallengeData) => {
	window.alert(`You got challanged to a fight by ${fightChallengeData.challenger.fighterUserName}`);
	connection.invoke("JoinFight", fightChallengeData.fightId, fightChallengeData.adversary.fighterId).catch(function (err) {
		return console.error(err.toString());
	})
});


document.getElementById("attackButton").addEventListener("click", function (event) {
	event.preventDefault();
	var attackButton = document.getElementById("attackButton");
	attackButton.disabled = true;
	attackButton.value = "Attacking!";
	connection.invoke("Attack", _fightId, _fighterId).catch(function (err) {
		return console.error(err.toString());
	});
});

//document.getElementById("resetButton").addEventListener("click", function (event) {
//	event.preventDefault();
//	document.getElementById("attackButton").disabled = false;
//	document.getElementById("message").innerText = "";

//});

document.getElementById("updateButton").addEventListener("click", function (event) {
	event.preventDefault();
	connection.invoke("GetPresentFightData", _fightId, _fighterId).catch(function (err) {
		return console.error(err.toString());
	});
	connection.invoke("GetFighterHighscores").catch(function (err) {
		return console.error(err.toString());
	});
	connection.invoke("GetUserHighscores").catch(function (err) {
		return console.error(err.toString());
	});


});


document.getElementById("joinButton").addEventListener("click", function (event) {
	event.preventDefault();
	connection.invoke("startFight", 4, 2, "341b68d0-6f19-4260-b056-2433f1ab49ab").catch(function (err) {
		return console.error(err.toString());
	});
	document.getElementById("round").textContent = ``;

});

start();
