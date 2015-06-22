define(['lodash'], function(_) {
    var interop = {
        InXna: function() {
            return typeof xna !== 'undefined';
        },
        
        xna: function() {
            return xna;
        },
        
        onOver: function() {
            if (interop.InXna()) {
                xna.OnMouseOver();
            }
        },

        onLeave: function() {
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

        createLobby: function(type) {
            if (interop.InXna()) {
                xna.CreateLobby(type);
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