define(['lodash', 'interop'], function(_, interop) {
    var sound = {
        onEvent: function(name) {
            return function(e) {
                interop.playSound(name);
            };
        },
    };
    
    return sound;
});