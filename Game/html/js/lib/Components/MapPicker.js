'use strict';

var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function (_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Glyphicon = ReactBootstrap.Glyphicon;
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    var ListGroup = ReactBootstrap.ListGroup;
    var ListGroupItem = ReactBootstrap.ListGroupItem;
    var Modal = ReactBootstrap.Modal;

    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var RenderAtMixin = ui.RenderAtMixin;
    var Input = ui.Input;

    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var MapEntry = React.createClass({
        displayName: 'MapEntry',

        render: function render() {
            return React.createElement(
                ListGroupItem,
                { href: '#', onClick: this.props.onPick, onDoubleClick: this.props.onConfirm },
                this.props.directory ? React.createElement(
                    'span',
                    null,
                    React.createElement(Glyphicon, { glyph: 'folder-open' }),
                    '  '
                ) : null,
                this.props.name
            );
        }
    });

    return React.createClass({
        getInitialState: function getInitialState() {
            if (this.props.getMaps) {
                this.props.maps = this.props.getMaps(this.props.directory);
            }

            return {
                up: [],
                maps: this.props.maps,
                path: [],
                file: ''
            };
        },

        componentDidMount: function componentDidMount() {
            this.enabled = false;
            //interop.disableGameInput();
            interop.toggleChat(true);
        },

        componentWillUpdate: function componentWillUpdate() {
            if (this.enabled) {
                this.enabled = false;
                //interop.disableGameInput();
                interop.toggleChat(true);
            }
        },

        componentWillUnmount: function componentWillUnmount() {
            this.enabled = true;
            //interop.enableGameInput();
            interop.toggleChat(false);
        },

        onPick: function onPick(map) {
            this.setState({
                file: map
            });

            if (this.props.onPick) {
                this.props.onPick(map);
            }
        },

        back: function back() {
            var prev = this.state.up.pop();
            this.state.path.pop();

            this.setState({
                up: this.state.up,
                maps: prev,
                path: this.state.path,
                file: ''
            });
        },

        path: function path() {
            var title = '';

            if (this.props.directory) {
                title = this.props.directory + '/';
            }

            for (var i = 0; i < this.state.path.length; i++) {
                title += this.state.path[i] + '/';
            }

            return title;
        },

        onConfirm: function onConfirm() {
            var file = this.state.file;
            if (this.refs.input) {
                file = this.refs.input.state.value;
            }

            if (file === null || file.length === 0) {
                return;
            }

            var map = this.path() + file;

            this.props.onRequestHide();
            this.props.onConfirm(map);
        },

        render: function render() {
            var _this = this;

            var mapEntrees = _.map(this.state.maps, function (map) {
                if (_.isObject(map)) {
                    var directory = map;

                    var onPick = function onPick() {
                        _this.state.up.push(_this.state.maps);
                        _this.state.path.push(directory.name);

                        _this.setState({
                            up: _this.state.up,
                            maps: directory.list,
                            path: _this.state.path
                        });
                    };

                    return React.createElement(MapEntry, { directory: true, name: directory.name, onPick: onPick });
                } else {
                    var onPick = function onPick() {
                        _this.onPick(map);
                    };

                    return React.createElement(MapEntry, { name: map, onPick: onPick, onConfirm: _this.onConfirm });
                }
            });

            var title = ' ';
            if (this.props.showPath) {
                title += this.path();

                title = React.createElement(
                    'h4',
                    { style: { 'margin-top': '3px', 'margin-bottom': '0px', 'color': 'white' } },
                    title
                );
            }

            return React.createElement(
                Modal,
                _extends({}, this.props, { bsStyle: 'primary', title: title, animation: false }),
                React.createElement(
                    'div',
                    { className: 'modal-body', style: { 'height': 0.5 * window.h + 'px' } },
                    React.createElement(
                        'div',
                        { className: 'chat-background', style: { 'width': '100%', 'overflow-y': 'scroll', 'height': '100%', 'pointer-events': 'auto' } },
                        React.createElement(
                            ListGroup,
                            { style: { 'pointer-events': 'auto', 'font-size': '1.4%' } },
                            mapEntrees,
                            this.state.up && this.state.up.length > 0 ? React.createElement(
                                ListGroupItem,
                                { href: '#', onClick: this.back },
                                React.createElement(Glyphicon, { glyph: 'arrow-left' })
                            ) : null
                        )
                    )
                ),
                React.createElement(
                    'div',
                    { className: 'modal-footer', style: { 'font-size': '1.4%' } },
                    this.props.saveAs ? React.createElement(
                        'div',
                        { style: { 'width': '56%', 'display': 'inline-block', 'float': 'left' } },
                        React.createElement(Input, { value: this.state.file, ref: 'input', prefix: 'Name' })
                    ) : null,
                    this.props.confirm ? React.createElement(
                        Button,
                        { onClick: this.onConfirm },
                        this.props.confirm
                    ) : null,
                    React.createElement(
                        Button,
                        { onClick: this.props.onRequestHide },
                        'Close'
                    )
                )
            );
        }
    });
});