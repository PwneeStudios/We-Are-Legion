define(['lodash', 'react'], function(_, React) {
    util = {
        pos: function(x, y, type) {
            if (typeof type === 'undefined') {
                type = 'absolute';
            }
        
            return {
                position:type,
                left: x + '%',
                top: y + '%',
            };
        },

        size: function(x, y) {
            return {
                width: x + '%',
                height: y + '%',
            };
        },

        width: function(x) {
            return util.size(x, 100);
        },
    };
    
    return util;
});