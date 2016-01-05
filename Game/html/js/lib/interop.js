'use strict';

define(['lodash'], function (_) {
    var invoke = function invoke(invocation) {
        for (var _len = arguments.length, args = Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
            args[_key - 1] = arguments[_key];
        }

        if (!window.updateKey) window.updateKey = 1;else window.updateKey++;

        invocation += '(';

        if (args && args.length > 0) {
            for (var i = 0; i < args.length; i++) {
                invocation += args[i];

                if (i < args.length - 1) {
                    invocation += '``';
                }
            }
        }

        invocation += ')';

        window.location.hash = '#invoke!guid' + window.updateKey + '!' + invocation;
    };

    var log = function log(msg) {
        console.log(msg);
        invoke('JsLog', msg);
    };

    var error = function error(message, url, lineNumber, colNumber, obj) {
        var msg = 'error encounted at ' + lineNumber + ' : ' + colNumber + ': ' + message;
        console.log(msg, obj);
        invoke('JsError', msg);
    };

    window.invoke = invoke;
    window.log = log;
    window.onerror = error;

    var interop = {
        InXna: function InXna() {
            //return typeof xna !== 'undefined';
            return true;
        },

        leaveGame: function leaveGame() {
            if (interop.InXna()) {
                invoke("LeaveGame");
            }
        },

        quitApp: function quitApp() {
            if (interop.InXna()) {
                invoke("QuitApp");
            }
        },

        dumpState: function dumpState(state) {
            if (interop.InXna()) {
                invoke("DumpState", state);
            }
        },

        onOver: function onOver() {
            if (interop.InXna()) {
                invoke("OnMouseOver");
            }
        },

        onLeave: function onLeave() {
            if (interop.InXna()) {
                invoke("OnMouseLeave");
            }
        },

        disableGameInput: function disableGameInput() {
            if (interop.InXna()) {
                invoke("DisableGameInput");
            }
        },

        enableGameInput: function enableGameInput() {
            if (interop.InXna()) {
                invoke("EnableGameInput");
            }
        },

        lobbyUiCreated: function lobbyUiCreated() {
            if (interop.InXna()) {
                invoke("LobbyUiCreated");
            }
        },

        drawMapPreviewAt: function drawMapPreviewAt(x, y, width, height) {
            if (interop.InXna()) {
                invoke("DrawMapPreviewAt", x, y, width, height);
            }
        },

        hideMapPreview: function hideMapPreview() {
            if (interop.InXna()) {
                invoke("HideMapPreview");
            }
        },

        setMap: function setMap(map) {
            if (interop.InXna()) {
                invoke("SetMap", map);
            }
        },

        get: function get(funcName) {
            return 0;
            //fixme
            //var stringResult = xna[funcName]();
            //return JSON.parse(stringResult);
        },

        findLobbies: function findLobbies(friends) {
            if (interop.InXna()) {
                if (friends) {
                    invoke("FindFriendLobbies");
                } else {
                    invoke("FindLobbies");
                }
            }
        },

        createLobby: function createLobby(type, training) {
            if (interop.InXna()) {
                invoke("CreateLobby", type, training);
            }
        },

        joinLobby: function joinLobby(index) {
            if (interop.InXna()) {
                invoke("JoinLobby", index);
            }
        },

        leaveLobby: function leaveLobby() {
            if (interop.InXna()) {
                invoke("LeaveLobby");
            }
        },

        returnToLobby: function returnToLobby() {
            if (interop.InXna()) {
                invoke("ReturnToLobby");
            }
        },

        setLobbyType: function setLobbyType(type) {
            if (interop.InXna()) {
                invoke("SetLobbyType", type);
            }
        },

        requestPause: function requestPause(index) {
            if (interop.InXna()) {
                invoke("RequestPause");
            }
        },

        requestUnpause: function requestUnpause(index) {
            if (interop.InXna()) {
                invoke("RequestUnpause");
            }
        },

        toggleEditor: function toggleEditor(index) {
            if (interop.InXna()) {
                invoke("PlayButtonPressed");
            }
        },

        setUnitPaint: function setUnitPaint(type) {
            if (interop.InXna()) {
                invoke("SetUnitPaint", type);
            }
        },

        setTilePaint: function setTilePaint(type) {
            if (interop.InXna()) {
                invoke("SetTilePaint", type);
            }
        },

        setPlayer: function setPlayer(player) {
            if (interop.InXna()) {
                invoke("SetPlayer", player);
            }
        },

        setPaintChoice: function setPaintChoice(choice) {
            if (interop.InXna()) {
                invoke("SetPaintChoice", choice);
            }
        },

        getMaps: function getMaps(directory) {
            if (interop.InXna()) {
                return JSON.parse(invoke("GetMaps", directory));
            } else {
                return maps = [{ name: 'CUSTOM', list: ['__map1', '__map2', '__map3'] }, 'Beset', 'Clash of Madness', 'Nice', { name: 'DOWNLOADS', list: ['map1', 'map2', { name: 'CUSTOM', list: ['__map1', '__map2', '__map3'] }, 'map3'] }];
            }
        },

        loadMap: function loadMap(path) {
            if (interop.InXna()) {
                invoke("LoadMap", path);
            }
        },

        saveMap: function saveMap(path) {
            if (interop.InXna()) {
                invoke("SaveMap", path);
            }
        },

        createNewMap: function createNewMap(path) {
            if (interop.InXna()) {
                invoke("CreateNewMap", path);
            }
        },

        editorUiClicked: function editorUiClicked() {
            if (interop.InXna()) {
                invoke("EditorUiClicked");
            }
        },

        toggleChat: function toggleChat(state) {
            if (interop.InXna()) {
                invoke("ToggleChat", state);
            }
        },

        startEditor: function startEditor() {
            if (interop.InXna()) {
                invoke("StartEditor");
            }
        },

        watchGame: function watchGame(lobby) {
            if (interop.InXna()) {
                invoke("WatchGame", lobby);
            }
        },

        join: function join() {
            if (interop.InXna()) {
                invoke("Join");
            }
        },

        spectate: function spectate() {
            if (interop.InXna()) {
                invoke("Spectate");
            }
        },

        playSound: function playSound(sound, vol) {
            if (interop.InXna()) {
                if (typeof vol === 'undefined') {
                    vol = 1;
                }

                invoke("PlaySound", sound, vol);
            }
        },

        onLobbyChatEnter: function onLobbyChatEnter(message) {
            if (interop.InXna()) {
                invoke("OnLobbyChatEnter", message);
            }
        },

        onChatEnter: function onChatEnter(message) {
            if (interop.InXna()) {
                invoke("OnChatEnter", message);
            }
        },

        actionButtonPressed: function actionButtonPressed(name) {
            if (interop.InXna()) {
                invoke("ActionButtonPressed", name);
            }
        },

        selectTeam: function selectTeam(team) {
            if (interop.InXna()) {
                invoke("SelectTeam", team);
            }
        },

        selectKingdom: function selectKingdom(kingdom) {
            if (interop.InXna()) {
                invoke("SelectKingdom", kingdom);
            }
        },

        startGame: function startGame() {
            if (interop.InXna()) {
                invoke("StartGame");
            }
        },

        startGameCountdown: function startGameCountdown() {
            if (interop.InXna()) {
                invoke("StartGameCountdown");
            }
        },

        getFullscreen: function getFullscreen() {
            if (interop.InXna()) {
                invoke("GetFullscreen");
            }
        }
    };

    return interop;
});