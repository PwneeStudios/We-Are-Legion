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

        get: function(funcName) {
            var stringResult = xna[funcName]();
            return JSON.parse(stringResult);
        },
    };
    
    return interop;
});