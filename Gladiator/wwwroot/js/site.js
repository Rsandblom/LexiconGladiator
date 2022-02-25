// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// Setup connection
const connectionSite = new signalR.HubConnectionBuilder()
	.withUrl("https://HOST:PORT/fightHub".replace("HOST", location.hostname).replace("PORT", location.port))
	.withAutomaticReconnect([0, 1000, 1000, 2000, 3000, 5000, 8000, 13000, 21000, 34000]) // <3 Fibonacci
	.configureLogging(signalR.LogLevel.Information)
	.build();

async function startSignalRSite() {
	try {
		await connectionSite.start();
		console.assert(connectionSite.state === signalR.HubConnectionState.Connected);
		console.log("SignalR Connected.");

		tryGetExistingGame();
	}
	catch (err) {
		console.assert(connectionSite.state === signalR.HubConnectionState.Disconnected);
		console.log(err);
		setTimeout(() => startSignalRSite(), 5000);
	}

};

// Connection lost, reconnecting.
connectionSite.onreconnecting(error => {

	console.assert(connectionSite.state === signalR.HubConnectionState.Reconnecting);


	// Do something?

});


// Connection is back up
connectionSite.onreconnected(connectionId => {

	console.assert(connectionSite.state === signalR.HubConnectionState.Connected);
	setTimeout(tryGetExistingGame(), 5000);
});

// Connection closed
connectionSite.onclose(error => {

	console.assert(connectionSite.state === signalR.HubConnectionState.Disconnected);

	console.log(`Connection cloesed!Error:\n ${error}`);
	setTimeout(() => startSignalR(), 5000);

});


//Task FightChallenge(Guid fightId, FightChallangeDTO fightChallangeData);
// Got Challenged to a fight
connectionSite.on("fightChallenge", (fightId, fightChallengeData) => {

	if (confirm(
		`You got challenged to a fight by ${fightChallengeData.challenger.fighterUserName}!\n` +
		`Do you want to accept the challenge?`
	) == true) {
		storeFightId(fightId);
		storeFigherId(fightChallengeData.adversary.fighterId);

		connectionSite.invoke("joinFight", fightId, parseInt(fightChallengeData.adversary.fighterId)).catch(function (err) {
			return console.error(err.toString());
		});

		window.location.href = '/Home/Arena1';

	} else {

		connectionSite.invoke("leaveFight", fightId, parseInt(fightChallengeData.adversary.fighterId)).catch(function (err) {
			return console.error(err.toString());
		});

	}
	console.log("Got challenged to a fight!");
});

startSignalRSite();


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