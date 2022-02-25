
"use strict";

const defaultMainLogText = "<h5>WELCOME TO ARENA!</h5><br>It's a glorious day to find out if current champion can defend his honor or we can coronate a new Winner! <br /> Are YOU our new Champion ? <br /><br />You can play against computer or against your friend.<br />Take a look at Wall of Glory to view your competition.<br>Use log in option to participate.";

const avatars = ["/img/gl/0.png", "/img/gl/1.png", "/img/gl/2.png", "/img/gl/3.png", "/img/gl/4.png", "/img/gl/5.png"];


document.getElementById("attackButton").value = "No fight";
//document.getElementById("attackButton").disabled = true;
var fightDescription ="";
var lyou = "";
var lopponent = "";

// Setup connection
const connection = new signalR.HubConnectionBuilder()
	.withUrl("https://HOST:PORT/fightHub".replace("HOST", location.hostname).replace("PORT", location.port))
	.withAutomaticReconnect([0, 1000, 1000, 2000, 3000, 5000, 8000, 13000, 21000, 34000]) // <3 Fibonacci
	.configureLogging(signalR.LogLevel.Information)
	.build();

async function startSignalR() {
	try {
		await connection.start();
		console.assert(connection.state === signalR.HubConnectionState.Connected);
		console.log("SignalR Connected.");
		// Check if there already is a game going
		if (!tryGetExistingGame()) {
			var arenaId = document.getElementById("fight-data").dataset.arenaid;
			var challengerId = parseInt(document.getElementById("fight-data").dataset.challengerid);
			var adversaryId = parseInt(document.getElementById("fight-data").dataset.adversaryid);

			connection.invoke("startFight", challengerId, adversaryId, arenaId).catch(function (err) {
				return console.error(err.toString());
			});
		}
		
	}
	catch (err) {
		console.assert(connection.state === signalR.HubConnectionState.Disconnected);
		console.log(err);
		setTimeout(() => startSignalR(), 5000);
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
	setTimeout(tryGetExistingGame(), 5000);
});

// Connection closed
connection.onclose(error => {

	console.assert(connection.state === signalR.HubConnectionState.Disconnected);

	console.log(`Connection cloesed!Error:\n ${error}`);
	setTimeout(() => startSignalR(), 5000);

});


// Set up eventhandlers for SignalR

//Task Error(string errorMessage);
connection.on("Error", (errorMessage) => {
	console.error(`Recieived error: ${errorMessage}`);
});

//Task UserFightIdList(IList<Guid> fightIds);
// Get all active fights the user is in
connection.on("UserFightIdList", (fightIds) => {
	
});

//Task FightChallenge(Guid fightId, FightChallangeDTO fightChallangeData);
// Got Challenged to a fight
connection.on("FightChallenge", (fightId, fightChallengeData) => {

	if (confirm(
			`You got challenged to a fight by ${fightChallengeData.challenger.fighterUserName}!\n` +
			`Do you want to accept the challenge?`
	) == true) {
		storeFightId(fightId);
		storeFigherId(fightChallengeData.adversary.fighterId);

		connection.invoke("joinFight", fightId, parseInt(fightChallengeData.adversary.fighterId)).catch(function (err) {
			return console.error(err.toString());

			window.location.href = '/Home/Arena1';
		});
	} else {
		connection.invoke("leaveFight", fightId, parseInt(fightChallengeData.adversary.fighterId)).catch(function (err) {
			return console.error(err.toString());
		});
	}
	console.log("Got challenged to a fight!");
});

//Task JoinedFight(Guid fightId, int fighterId);
connection.on("JoinedFight", (fightId, fighterId) => {
	console.log(`Joined fight ${fightId} as fighter ${fighterId}.`);
	//document.getElementById("attackButton").value = "Not started";
	//document.getElementById("attackButton").disabled = true;
	setCookie("fightId", fightId, 1);
	setCookie("fighterId", fighterId, 1);
});

//Task FightStarted(Guid fightId);
connection.on("FightStarted", (fightId) => {
	document.getElementById("MainLog").innerHTML = "Fight has started!";

	document.getElementById("attackButton").value = "Started";
	//document.getElementById("attackButton").disabled = true;
});

//Task FightEnded(Guid fightId);
connection.on("FightEnded", (fightId) => {
	document.getElementById("attackButton").value = "Fight ended";
	document.getElementById("attackButton").disabled = true;
	deleteFightCookies();

});

//Task FightCancelled(Guid fightId);
connection.on("FightCancelled", (fightId) => {
	document.getElementById("attackButton").value = "Fight Cancelled/Challenge not accepted";
	document.getElementById("attackButton").disabled = true;
	deleteFightCookies();
});

//Task OpponentLeftFight(Guid fightId);
connection.on("OpponentLeftFight", (fightId) => {
	document.getElementById("attackButton").value = "Opponent left";
	document.getElementById("attackButton").disabled = true;
	deleteFightCookies();
	updateLog({roundDescription:lopponent+' has fled the field! The coward!',roundNo:'-'})
});

//Task AttackAllowed(Guid fightId, bool isAttackAllowed);
connection.on("AttackAllowed", (fightId, isAttackAllowed) => {
	if (isAttackAllowed) {
		document.getElementById("attackButton").value = "Attack!";
		document.getElementById("attackButton").disabled = false;
	} else {
		document.getElementById("attackButton").disabled = true;
	}
});

//Task Attacked(Guid fightId);
connection.on("Attacked", (fightId) => {
	document.getElementById("attackButton").value = "Defending";
	document.getElementById("attackButton").disabled = true;

});

//Task RoundData(Guid fightId, FightRoundDTO fightRoundData);
connection.on("RoundData", (fightId, data) => {

	
	updateLog(data);
	document.getElementById("ProgressBar1").style.width = `${data.you.currentHpPercent}%`;
	document.getElementById("ProgressBar1").textContent = `${data.you.currentHpPercent}%`;

	document.getElementById("ProgressBar2").style.width = `${data.opponent.currentHpPercent}%`;
	document.getElementById("ProgressBar2").textContent = `${data.opponent.currentHpPercent}%`;
	if (data.you.isAttacker) {
		document.getElementById("attackButton").value = "Waiting";
		document.getElementById("attackButton").disabled = true;
	}
});


//Task FightData(Guid fightId, FightDTO fightData);
connection.on("FightData", (fightId, data) => {
	if (typeof (data) !== "undefined" && data !== null) {
		if (data.opponent.isComputer) {
			document.getElementById("FightType").innerHTML = "GLADIATOR PvE";
		}
		else {
			document.getElementById("FightType").innerHTML = "GLADIATOR PvP";
		}
		lyou = data.you.name;
		lopponent = data.opponent.name;
		fightDescription = "";
		document.getElementById("Avatar1").src = avatars[data.you.fighterId % avatars.length];
		document.getElementById("Name1").innerHTML = data.you.name;
		document.getElementById("ProgressBar1").style.width = `${data.you.currentHpPercent}%`;
		document.getElementById("ProgressBar1").textContent = `${data.you.currentHpPercent}%`;
		document.getElementById("Str1").textContent = data.you.strength;
		document.getElementById("Def1").textContent = data.you.defence;
		document.getElementById("Xp1").textContent = data.you.xp;

		document.getElementById("Avatar2").src = avatars[data.opponent.fighterId % avatars.length];
		document.getElementById("Name2").innerHTML = data.opponent.name;
		document.getElementById("ProgressBar2").style.width = `${data.opponent.currentHpPercent}%`;
		document.getElementById("ProgressBar2").textContent = `${data.opponent.currentHpPercent}%`;
		document.getElementById("Str2").textContent = data.opponent.strength;
		document.getElementById("Def2").textContent = data.opponent.defence;
		document.getElementById("Xp2").textContent = data.opponent.xp;
	}
	else {
		document.getElementById("MainLog").innerHTML = defaultMainLogText;
		document.getElementById("FightType").innerHTML = "GLADIATOR PvP";
		document.getElementById("Avatar1").src = avatars[4];
		document.getElementById("FightType").innerHTML = "GLADIATOR PvP";
		document.getElementById("Name1").innerHTML = "PLAYER 1";
		document.getElementById("ProgressBar1").style.width = "100%";
		document.getElementById("ProgressBar1").textContent = "100%";
		document.getElementById("Str1").textContent = 0;
		document.getElementById("Def1").textContent = 0;
		document.getElementById("Xp1").textContent = 0;

		document.getElementById("Avatar2").src = avatars[2];
		document.getElementById("Name2").innerHTML = "PLAYER 2";
		document.getElementById("ProgressBar2").style.width = "100%";
		document.getElementById("ProgressBar2").textContent = "100%";
		document.getElementById("Str2").textContent = 0;
		document.getElementById("Def2").textContent = 0;
		document.getElementById("Xp2").textContent = 0;
	}

});

//Task FightResults(Guid fightId, FightResultDTO fightResultData);
connection.on("FightResults", (fightId, fightResultData) => {
	document.getElementById("attackButton").value = "Fight over";
	document.getElementById("attackButton").disabled = true;
	deleteFightCookies();
});

//Task FighterHighscores(IList<FighterHighscoreDTO> highscores);
connection.on("FighterHighscores", (highscoreList) => {
	console.log(highscoreList);
});

//Task UserHighscores(IList<UserHighscoreDTO> highscores);
connection.on("UserHighscores", (highscoreList) => {
	console.log(highscoreList);
});


document.getElementById("attackButton").addEventListener("click", function (event) {
	event.preventDefault();

	var attackButton = document.getElementById("attackButton");

	if (retriveFightId() != "" && retriveFighterId() > 0) {

		connection.invoke("attack", retriveFightId(), retriveFighterId()).catch(function (err) {
			success = false;
			return console.error(err.toString());
		});

		attackButton.disabled = true;
		attackButton.value = "Attacking!";
	}

});


window.onload = () => {

	startSignalR();
	init();

}


function init() {
	document.getElementById("MainLog").innerHTML = defaultMainLogText;

	document.getElementById("FightType").innerHTML = "GLADIATOR PvP";

	document.getElementById("Avatar1").src = avatars[4];
	document.getElementById("Name1").innerHTML = "PLAYER 1";
	document.getElementById("ProgressBar1").style.width = 100 + "%";
	document.getElementById("ProgressBar1").textContent = ``;
	document.getElementById("Str1").textContent = 0;
	document.getElementById("Def1").textContent = 0;
	document.getElementById("Xp1").textContent = 0;

	document.getElementById("Avatar2").src = avatars[2];
	document.getElementById("Name2").innerHTML = "PLAYER 2";
	document.getElementById("ProgressBar2").style.width = 100 + "%";
	document.getElementById("ProgressBar2").textContent = ``;
	document.getElementById("Str2").textContent = 0;
	document.getElementById("Def2").textContent = 0;
	document.getElementById("Xp2").textContent = 0;
};




// Helper functions

function storeFigherId(fighterId) {
	setCookie("fighterId", fighterId, 1);
};

function storeFightId(fightId) {
	setCookie("fightId", fightId, 1);
};

function retriveFighterId() {
	let fighterId = getCookie("fighterId");
	if (fighterId == "") {
		return -1;
	}
	return parseInt(fighterId);
};

function retriveFightId(fightId) {
	return getCookie("fightId");
};

function deleteFightCookies() {
	setCookie("fightId", "", -1);
	setCookie("fighterId", "", -1);
}

function tryGetExistingGame() {
	let fightId = retriveFightId();
	let fighterId = retriveFighterId();
	if (fightId != "" && fighterId > 0) {
		connection.invoke("JoinFight", fightId, fighterId).catch(function (err) {
			return console.error(err.toString());
		});
		return true;
	}
	return false;
}

function setCookie(cname, cvalue, exdays) {
	// From https://www.w3schools.com/js/js_cookies.asp
	const d = new Date();
	d.setTime(d.getTime() + (exdays * 86400000)); // (exdays * 86400000) == (exdays * 24 * 60 * 60 * 1000))
	let expires = "expires=" + d.toUTCString();
	document.cookie = cname + "=" + cvalue + ";Path=/;SameSite=Strict;Domain=" + location.hostname + ";" + expires;
}

function getCookie(cname) {
	// From https://www.w3schools.com/js/js_cookies.asp
	let name = cname + "=";
	let decodedCookie = decodeURIComponent(document.cookie);
	let ca = decodedCookie.split(';');
	for (let i = 0; i < ca.length; i++) {

		let c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			return c.substring(name.length, c.length);
		}
	}
	return "";
}

function updateLog(data) {
	fightDescription += "Round " + data.roundNo + ": " + data.roundDescription + "<br><br>";
	if (data.you.currentHpPercent == 0) { fightDescription += 'You have lost the fight!' }
	if (data.opponent.currentHpPercent == 0) { fightDescription += 'You have won the fight!' }
	document.getElementById("MainLog").innerHTML = fightDescription;
	document.getElementById('MainLog').scrollTop = document.getElementById('MainLog').scrollHeight;
}