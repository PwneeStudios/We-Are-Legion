'use strict';

var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

define(['lodash', 'sound', 'react', 'react-bootstrap'], function (_, sound, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        onClick: function onClick(e) {
            if (this.props.to) {
                if (this.props.to === 'back') {
                    sound.play.back();
                    window.back();
                } else {
                    sound.play.click();
                    window.setScreen(this.props.to, this.props.params);
                }
            } else if (this.props.toMode) {
                sound.play.click();
                window.setMode(this.props.toMode, this.props.params);
            } else if (this.props.onClick) {
                sound.play.click();
                this.props.onClick(e);
            }
        },

        render: function render() {
            return React.createElement(
                NavItem,
                _extends({}, this.props, { onClick: this.onClick, onMouseEnter: sound.play.hover }),
                React.createElement(
                    'h3',
                    null,
                    this.props.children
                )
            );
        }
    });
});