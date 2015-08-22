define(['lodash'], function(_) {
    var interop = {
        InXna: function() {
            return typeof xna !== 'undefined';
        },
        
        xna: function() {
            return xna;
        },
        
        onOver: function() {
            //console.log('over');
            if (interop.InXna()) {
                xna.OnMouseOver();
            }
        },

        onLeave: function() {
            //console.log('leave');
            if (interop.InXna()) {
                xna.OnMouseLeave();
            }
        },

        disableGameInput: function() {
            if (interop.InXna()) {
                xna.DisableGameInput();
            }
        },

        enableGameInput: function() {
            if (interop.InXna()) {
                xna.EnableGameInput();
            }
        },

        lobbyUiCreated: function() {
            if (interop.InXna()) {
                xna.LobbyUiCreated();
            }
        },

        drawMapPreviewAt: function(x, y, width, height) {
            if (interop.InXna()) {
                xna.DrawMapPreviewAt(x, y, width, height);
            }
        },

        hideMapPreview: function() {
            if (interop.InXna()) {
                xna.HideMapPreview();
            }
        },

        setMap: function(map) {
            if (interop.InXna()) {
                xna.SetMap(map);
            }
        },

        get: function(funcName) {
            var stringResult = xna[funcName]();
            return JSON.parse(stringResult);
        },

        findLobbies: function(friends) {
            if (interop.InXna()) {
                if (friends) {
                    xna.FindFriendLobbies();
                } else {
                    xna.FindLobbies();
                }
            }
        },

        createLobby: function(type, training) {
            if (interop.InXna()) {
                xna.CreateLobby(type, training);
            }
        },

        joinLobby: function(index) {
            if (interop.InXna()) {
                xna.JoinLobby(index);
            }
        },

        leaveLobby: function() {
            if (interop.InXna()) {
                xna.LeaveLobby();
            }
        },

        setLobbyType: function(type) {
            if (interop.InXna()) {
                xna.SetLobbyType(type);
            }
        },

        requestPause: function(index) {
            if (interop.InXna()) {
                xna.RequestPause();
            }
        },

        requestUnpause: function(index) {
            if (interop.InXna()) {
                xna.RequestUnpause();
            }
        },

        toggleEditor: function(index) {
            if (interop.InXna()) {
                xna.PlayButtonPressed();
            }
        },

        setUnitPaint: function(type) {
            if (interop.InXna()) {
                xna.SetUnitPaint(type);
            }
        },

        setTilePaint: function(type) {
            if (interop.InXna()) {
                xna.SetTilePaint(type);
            }
        },

        setPlayer: function(player) {
            if (interop.InXna()) {
                xna.SetPlayer(player);
            }
        },

        setPaintChoice: function(choice) {
            if (interop.InXna()) {
                xna.SetPaintChoice(choice);
            }
        },

        getMaps: function(directory) {
            if (interop.InXna()) {
                return JSON.parse(xna.GetMaps(directory));
            } else {
                return maps = [{name:'CUSTOM',list:['__map1','__map2','__map3']},'Beset','Clash of Madness','Nice',{name:'DOWNLOADS',list:['map1','map2',{name:'CUSTOM',list:['__map1','__map2','__map3']},'map3',]}];
            }
        },

        loadMap: function(path) {
            if (interop.InXna()) {
                xna.LoadMap(path);
            }
        },

        saveMap: function(path) {
            if (interop.InXna()) {
                xna.SaveMap(path);
            }
        },

        createNewMap: function(path) {
            if (interop.InXna()) {
                xna.CreateNewMap(path);
            }
        },

        editorUiClicked: function() {
            if (interop.InXna()) {
                xna.EditorUiClicked();
            }
        },

        toggleChat: function(state) {
            if (interop.InXna()) {
                xna.ToggleChat(state);
            }
        },

        playSound: function(sound, vol) {
            if (interop.InXna()) {
                if (typeof vol === 'undefined') {
                    vol = 1;
                }

                xna.PlaySound(sound, vol);
            }
        },
    };
    
    return interop;
});