define(['lodash'], function(_) {
    var invoke = (invocation, ...args) => {
        if (!window.updateKey) window.updateKey = 1;
        else window.updateKey++;
        
        invocation += '(';
        
        if (args && args.length > 0) {
            for (let i = 0; i < args.length; i++) {
                invocation += args[i];
                
                if (i < args.length - 1) {
                    invocation += '``'
                }
            }
        }
        
        invocation += ')';

        window.location.hash='#invoke!guid' + window.updateKey + '!' + invocation;
    };
    
    var log = (msg) => {
        console.log(msg);
        invoke('JsLog', msg);
    };
    
    var error = (message, url, lineNumber, colNumber, obj) => {
        var msg = `error encounted at ${lineNumber} : ${colNumber}: ${message}`;
        console.log(msg, obj);
        invoke('JsError', msg);
    };
    
    window.invoke = invoke;
    window.log = log;
    window.onerror = error;

    var interop = {
        InXna: function() {
            //return typeof xna !== 'undefined';
            return true;
        },

        leaveGame: function() {
            if (interop.InXna()) {
                invoke("LeaveGame");
            }
        },
        
        quitApp: function() {
            if (interop.InXna()) {
                invoke("QuitApp");
            }
        },
        
        dumpState: function(state) {
            if (interop.InXna()) {
                invoke("DumpState", state);
            }
        },
        
        onOver: function() {
            if (interop.InXna()) {
                invoke("OnMouseOver");
            }
        },

        onLeave: function() {
            if (interop.InXna()) {
                invoke("OnMouseLeave");
            }
        },

        disableGameInput: function() {
            if (interop.InXna()) {
                invoke("DisableGameInput");
            }
        },

        enableGameInput: function() {
            if (interop.InXna()) {
                invoke("EnableGameInput");
            }
        },

        lobbyUiCreated: function() {
            if (interop.InXna()) {
                invoke("LobbyUiCreated");
            }
        },

        drawMapPreviewAt: function(x, y, width, height) {
            if (interop.InXna()) {
                invoke("DrawMapPreviewAt",x, y, width, height);
            }
        },

        hideMapPreview: function() {
            if (interop.InXna()) {
                invoke("HideMapPreview");
            }
        },

        setMap: function(map) {
            if (interop.InXna()) {
                invoke("SetMap", map);
            }
        },

        get: function(funcName) {
            return 0;
            //fixme
            //var stringResult = xna[funcName]();
            //return JSON.parse(stringResult);
        },

        findLobbies: function(friends) {
            if (interop.InXna()) {
                if (friends) {
                    invoke("FindFriendLobbies");
                } else {
                    invoke("FindLobbies");
                }
            }
        },

        createLobby: function(type, training) {
            if (interop.InXna()) {
                invoke("CreateLobby", type, training);
            }
        },

        joinLobby: function(index) {
            if (interop.InXna()) {
                invoke("JoinLobby", index);
            }
        },

        leaveLobby: function() {
            if (interop.InXna()) {
                invoke("LeaveLobby");
            }
        },

        returnToLobby: function() {
            if (interop.InXna()) {
                invoke("ReturnToLobby");
            }
        },

        setLobbyType: function(type) {
            if (interop.InXna()) {
                invoke("SetLobbyType", type);
            }
        },

        requestPause: function(index) {
            if (interop.InXna()) {
                invoke("RequestPause");
            }
        },

        requestUnpause: function(index) {
            if (interop.InXna()) {
                invoke("RequestUnpause");
            }
        },

        toggleEditor: function(index) {
            if (interop.InXna()) {
                invoke("PlayButtonPressed");
            }
        },

        setUnitPaint: function(type) {
            if (interop.InXna()) {
                invoke("SetUnitPaint", type);
            }
        },

        setTilePaint: function(type) {
            if (interop.InXna()) {
                invoke("SetTilePaint", type);
            }
        },

        setPlayer: function(player) {
            if (interop.InXna()) {
                invoke("SetPlayer", player);
            }
        },

        setPaintChoice: function(choice) {
            if (interop.InXna()) {
                invoke("SetPaintChoice", choice);
            }
        },

        getMaps: function(directory) {
            if (interop.InXna()) {
                return JSON.parse(invoke("GetMaps", directory));
            } else {
                return maps = [{name:'CUSTOM',list:['__map1','__map2','__map3']},'Beset','Clash of Madness','Nice',{name:'DOWNLOADS',list:['map1','map2',{name:'CUSTOM',list:['__map1','__map2','__map3']},'map3',]}];
            }
        },

        loadMap: function(path) {
            if (interop.InXna()) {
                invoke("LoadMap", path);
            }
        },

        saveMap: function(path) {
            if (interop.InXna()) {
                invoke("SaveMap", path);
            }
        },

        createNewMap: function(path) {
            if (interop.InXna()) {
                invoke("CreateNewMap", path);
            }
        },

        editorUiClicked: function() {
            if (interop.InXna()) {
                invoke("EditorUiClicked");
            }
        },

        toggleChat: function(state) {
            if (interop.InXna()) {
                invoke("ToggleChat", state);
            }
        },

        startEditor: function() {
            if (interop.InXna()) {
                invoke("StartEditor");
            }
        },

        watchGame: function(lobby) {
            if (interop.InXna()) {
                invoke("WatchGame", lobby);
            }
        },

        join: function() {
            if (interop.InXna()) {
                invoke("Join");
            }
        },

        spectate: function() {
            if (interop.InXna()) {
                invoke("Spectate");
            }
        },

        playSound: function(sound, vol) {
            if (interop.InXna()) {
                if (typeof vol === 'undefined') {
                    vol = 1;
                }

                invoke("PlaySound", sound, vol);
            }
        },
        
        onLobbyChatEnter: function(message) {
            if (interop.InXna()) {
                invoke("OnLobbyChatEnter", message);
            }
        },
        
        onChatEnter: function(message) {
            if (interop.InXna()) {
                invoke("OnChatEnter", message);
            }
        },

        actionButtonPressed: function(name) {
            if (interop.InXna()) {
                invoke("ActionButtonPressed", name);
            }
        },
        
        selectTeam: function(team) {
            if (interop.InXna()) {
                invoke("SelectTeam", team);
            }
        },

        selectKingdom: function(kingdom) {
            if (interop.InXna()) {
                invoke("SelectKingdom", kingdom);
            }
        },

        startGame: function() {
            if (interop.InXna()) {
                invoke("StartGame");
            }
        },
        
        startGameCountdown: function() {
            if (interop.InXna()) {
                invoke("StartGameCountdown");
            }
        },
        
        getFullscreen: function() {
            if (interop.InXna()) {
                invoke("GetFullscreen");
            }
        },
    };
    
    return interop;
});