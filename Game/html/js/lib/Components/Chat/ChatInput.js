'use strict';

define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui'], function (_, React, ReactBootstrap, interop, events, ui) {
    var Input = ReactBootstrap.Input;

    var Div = ui.Div;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var RenderAtMixin = ui.RenderAtMixin;

    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    return React.createClass({
        mixins: [RenderAtMixin, events.ShowUpdateMixin],

        onShowUpdate: function onShowUpdate(values) {
            this.setState({
                ShowChat: values.ShowChat,
                ChatGlobal: values.ChatGlobal
            });
        },

        getInitialState: function getInitialState() {
            return {
                value: ''
            };
        },

        onTextChange: function onTextChange() {
            this.setState({
                value: this.refs.input.getValue()
            });
        },

        focus: function focus() {
            var input = this.refs.input;
            if (input) {
                input.getInputDOMNode().focus();
            }
        },

        componentDidMount: function componentDidMount() {
            this.focus();
        },

        componentDidUpdate: function componentDidUpdate() {
            this.focus();
        },

        onKeyDown: function onKeyDown(e) {
            var keyCode = e.which || e.keyCode;
            if (keyCode == '13') {
                if (interop.InXna()) {
                    var message = this.refs.input.getInputDOMNode().value;

                    if (this.props.lobbyChat) {
                        interop.onLobbyChatEnter(message);
                    } else {
                        interop.onChatEnter(message);
                    }

                    this.setState({
                        value: ''
                    });
                }
            }
        },

        renderAt: function renderAt() {
            if (this.state.ShowChat || this.props.show) {
                var style = {
                    'pointer-events': 'auto',
                    'background-color': 'lightgray'
                };

                return React.createElement(
                    'div',
                    null,
                    React.createElement(Input, { value: this.state.value, ref: 'input', type: 'text',
                        addonBefore: this.state.ChatGlobal ? "All" : "Team",
                        style: style,
                        onChange: this.onTextChange, onKeyDown: this.onKeyDown,
                        onMouseOver: interop.onOver, onMouseLeave: interop.onLeave,
                        onBlur: this.focus })
                );
            } else {
                return React.createElement('div', null);
            }
        }
    });
});