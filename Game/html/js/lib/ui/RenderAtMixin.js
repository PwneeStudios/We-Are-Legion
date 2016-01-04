'use strict';

define(['react', 'ui/Div'], function (React, Div) {
    return {
        render: function render() {
            if (this.props.pos) {
                return React.createElement(
                    Div,
                    this.props,
                    this.renderAt()
                );
            } else {
                return this.renderAt();
            }
        }
    };
});