'use strict';

define(['lodash'], function (_) {
    var interop = {
        InXna: function InXna() {
            return typeof xna !== 'undefined';
        },

        xna: (function (_xna) {
            function xna() {
                return _xna.apply(this, arguments);
            }

            xna.toString = function () {
                return _xna.toString();
            };

            return xna;
        })(function () {
            return xna;
        }),

        onOver: function onOver() {
            //console.log('over');
            if (interop.InXna()) {
                xna.OnMouseOver();
            }
        },

        onLeave: function onLeave() {
            //console.log('leave');
            if (interop.InXna()) {
                xna.OnMouseLeave();
            }
        },

        disableGameInput: function disableGameInput() {
            if (interop.InXna()) {
                xna.DisableGameInput();
            }
        },

        enableGameInput: function enableGameInput() {
            if (interop.InXna()) {
                xna.EnableGameInput();
            }
        },

        lobbyUiCreated: function lobbyUiCreated() {
            if (interop.InXna()) {
                xna.LobbyUiCreated();
            }
        },

        drawMapPreviewAt: function drawMapPreviewAt(x, y, width, height) {
            if (interop.InXna()) {
                xna.DrawMapPreviewAt(x, y, width, height);
            }
        },

        hideMapPreview: function hideMapPreview() {
            if (interop.InXna()) {
                xna.HideMapPreview();
            }
        },

        setMap: function setMap(map) {
            if (interop.InXna()) {
                xna.SetMap(map);
            }
        },

        get: function get(funcName) {
            var stringResult = xna[funcName]();
            return JSON.parse(stringResult);
        },

        findLobbies: function findLobbies(friends) {
            if (interop.InXna()) {
                if (friends) {
                    xna.FindFriendLobbies();
                } else {
                    xna.FindLobbies();
                }
            }
        },

        createLobby: function createLobby(type, training) {
            if (interop.InXna()) {
                xna.CreateLobby(type, training);
            }
        },

        joinLobby: function joinLobby(index) {
            if (interop.InXna()) {
                xna.JoinLobby(index);
            }
        },

        leaveLobby: function leaveLobby() {
            if (interop.InXna()) {
                xna.LeaveLobby();
            }
        },

        returnToLobby: function returnToLobby() {
            if (interop.InXna()) {
                xna.ReturnToLobby();
            }
        },

        setLobbyType: function setLobbyType(type) {
            if (interop.InXna()) {
                xna.SetLobbyType(type);
            }
        },

        requestPause: function requestPause(index) {
            if (interop.InXna()) {
                xna.RequestPause();
            }
        },

        requestUnpause: function requestUnpause(index) {
            if (interop.InXna()) {
                xna.RequestUnpause();
            }
        },

        toggleEditor: function toggleEditor(index) {
            if (interop.InXna()) {
                xna.PlayButtonPressed();
            }
        },

        setUnitPaint: function setUnitPaint(type) {
            if (interop.InXna()) {
                xna.SetUnitPaint(type);
            }
        },

        setTilePaint: function setTilePaint(type) {
            if (interop.InXna()) {
                xna.SetTilePaint(type);
            }
        },

        setPlayer: function setPlayer(player) {
            if (interop.InXna()) {
                xna.SetPlayer(player);
            }
        },

        setPaintChoice: function setPaintChoice(choice) {
            if (interop.InXna()) {
                xna.SetPaintChoice(choice);
            }
        },

        getMaps: function getMaps(directory) {
            if (interop.InXna()) {
                return JSON.parse(xna.GetMaps(directory));
            } else {
                return maps = [{ name: 'CUSTOM', list: ['__map1', '__map2', '__map3'] }, 'Beset', 'Clash of Madness', 'Nice', { name: 'DOWNLOADS', list: ['map1', 'map2', { name: 'CUSTOM', list: ['__map1', '__map2', '__map3'] }, 'map3'] }];
            }
        },

        loadMap: function loadMap(path) {
            if (interop.InXna()) {
                xna.LoadMap(path);
            }
        },

        saveMap: function saveMap(path) {
            if (interop.InXna()) {
                xna.SaveMap(path);
            }
        },

        createNewMap: function createNewMap(path) {
            if (interop.InXna()) {
                xna.CreateNewMap(path);
            }
        },

        editorUiClicked: function editorUiClicked() {
            if (interop.InXna()) {
                xna.EditorUiClicked();
            }
        },

        toggleChat: function toggleChat(state) {
            if (interop.InXna()) {
                xna.ToggleChat(state);
            }
        },

        startEditor: function startEditor() {
            if (interop.InXna()) {
                xna.StartEditor();
            }
        },

        watchGame: function watchGame(lobby) {
            if (interop.InXna()) {
                xna.WatchGame(lobby);
            }
        },

        join: function join() {
            if (interop.InXna()) {
                xna.Join();
            }
        },

        spectate: function spectate() {
            if (interop.InXna()) {
                xna.Spectate();
            }
        },

        playSound: function playSound(sound, vol) {
            if (!window.updateKey) window.updateKey = 1;else window.updateKey++;

            //window.location.hash='#hello' + window.updateKey;
            window.location.hash = '#playSound,sound,vol,' + window.updateKey;

            if (interop.InXna()) {
                if (typeof vol === 'undefined') {
                    vol = 1;
                }

                xna.PlaySound(sound, vol);
            }
        }
    };

    return interop;
});