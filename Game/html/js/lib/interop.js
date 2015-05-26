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

        findLobbies: function() {
            if (interop.InXna()) {
                xna.FindLobbies();
            }
        },

        createLobby: function() {
            if (interop.InXna()) {
                xna.CreateLobby();
            }
        },

        joinLobby: function(index) {
            if (interop.InXna()) {
                xna.JoinLobby(index);
            }
        },
    };
    
    return interop;
});