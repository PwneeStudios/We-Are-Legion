'use strict';

define(['lodash', 'react'], function (_, React) {
    return React.createClass({
        render: function render() {
            var style = {
                width: '100%',
                height: '100%',
                'pointer-events': 'none'
            };

            style = _.assign(style, this.props.pos, this.props.size, this.props.style);

            if (this.props.nonBlocking) {
                style['pointer-events'] = 'none';
            } else if (this.props.blocking) {
                style['pointer-events'] = 'auto';
            }

            return React.createElement(
                'div',
                { style: style, className: this.props.className },
                this.props.children
            );
        }
    });
});