'use strict';

define(['lodash', 'react'], function (_, React) {
    var util = {
        pos: function pos(x, y, type) {
            if (typeof type === 'undefined') {
                type = 'absolute';
            }

            return {
                position: type,
                left: x + '%',
                top: y + '%'
            };
        },

        size: function size(x, y) {
            return {
                width: x + '%',
                height: y + '%'
            };
        },

        width: function width(x) {
            return util.size(x, 100);
        },

        subImage: function subImage(image, offset) {
            var sub = _.assign({}, image);
            sub.offset = offset;
            return sub;
        }
    };

    return util;
});