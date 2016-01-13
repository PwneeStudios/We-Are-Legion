'use strict';

define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function (_, sound, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Nav = ReactBootstrap.Nav;
    var NavItem = ReactBootstrap.NavItem;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;

    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var Menu = ui.Menu;
    var RenderAtMixin = ui.RenderAtMixin;

    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var MenuItem = React.createClass({
        displayName: 'MenuItem',

        render: function render() {
            return React.createElement(
                NavItem,
                this.props,
                React.createElement(
                    'h3',
                    null,
                    this.props.children
                )
            );
        }
    });

    var MenuSlider = React.createClass({
        displayName: 'MenuSlider',

        onChange: function onChange(e) {
            var value = this.refs.slider.getDOMNode().value;
            this.setState({ value: value });

            if (interop.InXna()) {
                window.invoke('Set' + this.props.variable, value);
            }
        },

        getInitialState: function getInitialState() {
            return { value: 0 };
        },

        componentDidMount: function componentDidMount() {
            var _this = this;
            window['get' + this.props.variable] = function (value) {
                return _this.setState({ value: value });
            };

            if (interop.InXna()) {
                window.invoke('Get' + this.props.variable);
            }
        },

        componentWillUnmount: function componentWillUnmount() {
            window['get' + this.props.variable] = function () {};
        },

        render: function render() {
            return React.createElement(
                'tr',
                { style: { 'background-color': '#1c1e22', 'pointer-events': 'auto' } },
                React.createElement(
                    'td',
                    { className: 'menu-description' },
                    this.props.children
                ),
                React.createElement(
                    'td',
                    null,
                    React.createElement('input', { style: { 'float': 'right', 'width': '100%' },
                        ref: 'slider',
                        type: 'range',
                        value: this.state.value,
                        min: 0,
                        max: 1,
                        onChange: this.onChange,
                        step: 0.05 })
                )
            );
        }
    });

    var MenuDropdown = React.createClass({
        displayName: 'MenuDropdown',

        onSelect: function onSelect(item) {
            if (interop.InXna()) {
                window.invoke('Set' + this.props.variable, item.value);
            }
        },

        getInitialState: function getInitialState() {
            return {};
        },

        componentDidMount: function componentDidMount() {
            var _this2 = this;

            var _this = this;
            window['get' + this.props.variable + 'Values'] = function (choices) {
                window['get' + _this2.props.variable] = function (value) {
                    _this.setState({
                        choices: choices,
                        value: value
                    });
                };
            };

            if (interop.InXna()) {
                window.invoke('Get' + this.props.variable + 'Values');
                window.invoke('Get' + this.props.variable);
            }
        },

        componentWillUnmount: function componentWillUnmount() {
            window['get' + this.props.variable] = function () {};
            window['get' + this.props.variable + 'Values'] = function () {};
        },

        render: function render() {
            var choices, value;

            choices = this.state.choices || this.props.choices;

            if (_.isUndefined(this.state.value)) {
                value = ' ';
            } else {
                value = this.state.choices[0];

                for (var i = 0; i < this.state.choices.length; i++) {
                    if (this.state.choices[i].value == this.state.value) {
                        value = this.state.choices[i];
                    }
                }
            }

            return React.createElement(
                'tr',
                { style: { 'background-color': '#1c1e22' } },
                React.createElement(
                    'td',
                    { className: 'menu-description' },
                    this.props.children
                ),
                React.createElement(
                    'td',
                    { className: 'menu-cell-dropdown' },
                    React.createElement(Dropdown, {
                        disabled: this.props.disabled,
                        scroll: this.props.scroll,
                        selected: value,
                        choices: choices,
                        onSelect: this.onSelect,
                        style: { 'float': 'right' } })
                )
            );
        }
    });

    var MenuButton = React.createClass({
        displayName: 'MenuButton',

        onClick: function onClick() {
            sound.play.click();

            if (this.props.onClick) {
                this.props.onClick();
            }
        },

        render: function render() {
            return React.createElement(
                'tr',
                { style: { 'background-color': '#1c1e22', 'pointer-events': 'auto' } },
                React.createElement('td', null),
                React.createElement(
                    'td',
                    null,
                    React.createElement(
                        Button,
                        { onClick: this.onClick, onMouseEnter: sound.play.hover, style: { 'float': 'right', 'width': '100%' } },
                        this.props.children
                    )
                )
            );
        }
    });

    return React.createClass({
        mixins: [events.AllowBackMixin],

        getInitialState: function getInitialState() {
            return {};
        },

        render: function render() {
            var resolutionChoices = [{ name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }, { name: '1920x1080', value: [1920, 1080] }];

            var fullscreenChoices = [{ name: 'Fullscreen', value: true }, { name: 'Windowed', value: false }];

            var disableResolutions = false;
            /*if (interop.InXna()) {
                var value = interop.getFullscreen();
                if (value) {
                    disableResolutions = true;
                }
            }fixme*/

            var screenOptions = [React.createElement(
                MenuDropdown,
                { disabled: disableResolutions, scroll: true, variable: 'Resolution', choices: resolutionChoices },
                'Resolution'
            ), React.createElement(
                MenuDropdown,
                { variable: 'Fullscreen', choices: fullscreenChoices },
                'Fullscreen setting'
            )];

            return React.createElement(
                Menu,
                { width: 30, type: 'table' },
                React.createElement(
                    MenuSlider,
                    { variable: 'SoundVolume' },
                    'Sound'
                ),
                React.createElement(
                    MenuSlider,
                    { variable: 'MusicVolume' },
                    'Music'
                ),
                this.props.params.inGame ? null : screenOptions,
                React.createElement(
                    MenuButton,
                    { onClick: back },
                    'Back'
                )
            );
        }
    });
});