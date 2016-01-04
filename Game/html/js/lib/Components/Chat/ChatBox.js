'use strict';

define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat/ChatLine'], function (_, React, ReactBootstrap, interop, events, ui, ChatLine) {
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
        mixins: [RenderAtMixin, events.OnChatMixin],

        onChatMessage: function onChatMessage(message) {
            var self = this;

            message.index = this.state.counter++;

            self.state.messages.push(message);

            if (this.isMounted()) {
                self.setState({
                    messages: self.state.messages
                });
            }
        },

        remove: function remove(message) {
            _.remove(this.state.messages, function (e) {
                return e.index === message.index;
            });

            if (this.isMounted()) {
                this.setState({ messages: this.state.messages });
            }
        },

        getInitialState: function getInitialState() {
            return {
                //messages: [{index:1, message:'hello there'}, {index:2, message:'hello there again'}, {index:3, message:'hello there etc'}],
                messages: [],
                counter: 0
            };
        },

        componentDidMount: function componentDidMount() {
            var self = this;
            return;

            setInterval(function () {
                self.onChatMessage({ name: 'player 1', message: 'hello there again ' + self.state.counter });
            }, 1000);
        },

        componentDidUpdate: function componentDidUpdate() {
            if (this.props.full) {
                var container = React.findDOMNode(this.refs.chatDiv);
                container.scrollTop = container.scrollHeight;
            }
        },

        renderAt: function renderAt() {
            var self = this;

            var hasBackground = !self.props.full;
            var willFadeOut = !self.props.full;

            var messages = _.map(this.state.messages, function (message) {
                return React.createElement(ChatLine, { hasBackground: hasBackground, willFadeOut: willFadeOut, key: message.index, message: message, remove: self.remove });
            });

            if (this.props.full) {
                return React.createElement(
                    'div',
                    { ref: 'chatDiv', className: 'chat-background', style: { 'width': '100%', 'overflow-y': 'scroll', 'height': '100%', 'pointer-events': 'auto' } },
                    messages
                );
            } else {
                return React.createElement(
                    'div',
                    { style: { 'position': 'relative', 'width': '100%' } },
                    React.createElement(
                        'div',
                        { style: { 'position': 'absolute', 'bottom': '0', 'width': '100%' } },
                        messages
                    )
                );
            }
        }
    });
});