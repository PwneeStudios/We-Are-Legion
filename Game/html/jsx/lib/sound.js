define(['lodash', 'interop'], function(_, interop) {
    var onEvent = function(name) {
        return function(e) {
            sound.playSound(name);
        };
    };

    var sound = {
        playSound: function(name, vol) {
            interop.playSound(name, vol);
        },

        play: {
            hover: onEvent('Menu_Back'),
            click: onEvent('Menu_Back'),
            back: onEvent('Menu_Back'),
        },
    };
    
    return sound;
});