define(['lodash', 'interop'], function(_, interop) {
    var onEvent = function(name, vol) {
        return function(e) {
            sound.playSound(name, vol);
        };
    };

    var sound = {
        playSound: function(name, vol) {
            interop.playSound(name, vol);
        },

        play: {
            hover: onEvent('Menu_Back', 0.0),
            listHover: onEvent('Menu_Back', 0.0),
            click: onEvent('Menu_Back', 0.3),
            back: onEvent('Menu_Back', 0.3),
        },
    };
    
    return sound;
});