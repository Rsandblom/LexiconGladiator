
var _fightId, _fighterId;

function startSignalR() {
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

		if (_fightId !== null && typeof _fightId !== 'undefined' && _fighterId !== null && typeof _fighterId !== 'undefined') {
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

	});

	//Task JoinedFight(Guid fightId, int fighterId);
	connection.on("JoinedFight", (fightId, fighterId) => {
		console.log(`Joined fight ${fightId} as fighter ${fighterId}`);


	});

	//Task FightStarted(Guid fightId);
	connection.on("FightStarted", (fightId) => {

	});

	//Task FightEnded(Guid fightId);
	connection.on("FightEnded", (fightId) => {
	

	});

	//Task FightCancelled(Guid fightId);
	connection.on("FightCancelled", (fightId) => {

	});

	//Task OpponentLeftFight(Guid fightId);
	connection.on("OpponentLeftFight", (fightId) => {

	});

	//Task AttackAllowed(Guid fightId, bool isAttackAllowed);
	connection.on("AttackAllowed", (fightId, isAttackAllowed) => {

	});

	//Task Attacked(Guid fightId);
	connection.on("Attacked", (fightId) => {

	});

	//Task RoundData(Guid fightId, FightRoundDTO fightRoundData);
	connection.on("RoundData", (fightId, fightRoundData) => {

	});

	//Task FightData(Guid fightId, FightDTO fightData);
	connection.on("FightData", (fightId, fightData) => {

	});

	//Task FightResults(Guid fightId, FightResultDTO fightResultData);
	connection.on("FightResults", (fightId, fightResultData) => {

	});

	//Task FighterHighscores(IList<FighterHighscoreDTO> highscores);
	connection.on("FighterHighscores", (highscoreList) => {

	});

	//Task UserHighscores(IList<UserHighscoreDTO> highscores);
	connection.on("UserHighscores", (highscoreList) => {

	});

}

// All is setup, time to start SignalR
startSignalR();


// Available commands on serverside

//connection.invoke("attack", fightId, attackerId).catch(function (err) {
//	return console.error(err.toString());
//});

//connection.invoke("startFight", challengerFighterId, adversaryFighterId).catch(function (err) {
//	return console.error(err.toString());
//});

//connection.invoke("joinFight", fightId, fighterId).catch(function (err) {
//	return console.error(err.toString());
//});

//connection.invoke("leaveFight", fightId, fighterId).catch(function (err) {
//	return console.error(err.toString());
//});

//connection.invoke("getPresentFightData", fightId, fighterId).catch(function (err) {
//	return console.error(err.toString());
//});

//connection.invoke("getUsersFights").catch(function (err) {
//	return console.error(err.toString());
//});

//connection.invoke("getFighterHighscores").catch(function (err) {
//	return console.error(err.toString());
//});

//connection.invoke("getUserHighscores").catch(function (err) {
//	return console.error(err.toString());
//});
