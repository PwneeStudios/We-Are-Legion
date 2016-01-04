'use strict';

var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

define(['lodash', 'sound', 'react', 'react-bootstrap', 'ui/Div', 'ui/Gap', 'ui/RenderAtMixin', 'ui/UiImage', 'ui/UiButton', 'ui/Dropdown', 'ui/Item', 'ui/MenuItem', 'ui/Menu', 'ui/OptionList', 'ui/MenuTitle', 'ui/Input', 'ui/util'], function (_, sound, React, ReactBootstrap, Div, Gap, RenderAtMixin, UiImage, UiButton, Dropdown, Item, MenuItem, Menu, OptionList, MenuTitle, Input, util) {
    var Button = ReactBootstrap.Button;

    var ui = {
        Div: Div,
        Gap: Gap,

        UiImage: UiImage,
        UiButton: UiButton,

        Dropdown: Dropdown,
        Item: Item,

        Menu: Menu,
        MenuItem: MenuItem,
        MenuTitle: MenuTitle,

        Input: Input,

        OptionList: OptionList,

        back: function back() {
            sound.play.back();
            window.back();
        },

        BackButton: React.createClass({
            displayName: 'BackButton',

            render: function render() {
                return React.createElement(
                    Button,
                    { onMouseEnter: sound.play.hover, onClick: ui.back },
                    'Back'
                );
            }
        }),

        Button: React.createClass({
            displayName: 'Button',

            onClick: function onClick() {
                sound.play.click();

                if (this.props.onClick !== null) {
                    this.props.onClick();
                }
            },

            render: function render() {
                return React.createElement(
                    Button,
                    _extends({}, this.props, {
                        disabled: this.props.disabled,
                        onClick: this.onClick,
                        onMouseEnter: sound.play.hover }),
                    this.props.children
                );
            }
        }),

        RenderAtMixin: RenderAtMixin
    };

    return _.assign({}, ui, util);
});