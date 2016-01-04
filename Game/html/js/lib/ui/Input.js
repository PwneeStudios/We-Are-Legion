'use strict';

define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui/RenderAtMixin'], function (_, React, ReactBootstrap, interop, events, RenderAtMixin) {
    var Input = ReactBootstrap.Input;

    return React.createClass({
        mixins: [RenderAtMixin],

        componentWillReceiveProps: function componentWillReceiveProps(nextProps) {
            this.setState(this.getInitialState(nextProps));
        },

        getInitialState: function getInitialState(props) {
            if (typeof props === 'undefined') {
                props = this.props;
            }

            return {
                value: props.value
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
            if (keyCode == '13' && this.props.onConfirm) {
                this.props.onConfirm();
            }
        },

        renderAt: function renderAt() {
            var style = {
                'pointer-events': 'auto',
                'background-color': 'lightgray'
            };

            return React.createElement(
                'div',
                null,
                React.createElement(Input, { value: this.state.value, ref: 'input', type: 'text',
                    addonBefore: this.props.prefix,
                    style: style,
                    onChange: this.onTextChange, onKeyDown: this.onKeyDown,
                    onMouseOver: interop.onOver, onMouseLeave: interop.onLeave,
                    onBlur: this.focus })
            );
        }
    });
});