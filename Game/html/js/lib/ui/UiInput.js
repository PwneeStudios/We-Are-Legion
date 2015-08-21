define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui'], function(_, React, ReactBootstrap, interop, events, ui) {
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
        mixins: [RenderAtMixin],
                
        getInitialState: function() {
            return {
                value: this.props.value,
            };
        },
    
        onTextChange: function() {
            this.setState({
                value: this.refs.input.getValue()
            });
        },
        
        focus: function() {
            var input = this.refs.input;
            if (input) {
                input.getInputDOMNode().focus();
            }
        },

        componentDidMount: function() {
            this.focus();
        },
        
        componentDidUpdate: function() {
            this.focus();
        },
        
        onKeyDown: function(e) {
            var keyCode = e.which || e.keyCode;
            if (keyCode == '13' && this.props.onConfirm) {
                this.props.onConfirm();
            }
        },
        
        renderAt: function() {
            var style = {
                'pointer-events':'auto',
                'background-color':'lightgray',
            };

            return (
                React.createElement("div", null, 
                    React.createElement(Input, {value: this.state.value, ref: "input", type: "text", 
                     addonBefore: this.props.prefix, 
                     style: style, 
                     onChange: this.onTextChange, onKeyDown: this.onKeyDown, 
                     onMouseOver: interop.onOver, onMouseLeave: interop.onLeave, 
                     onBlur: this.focus})
                )
            );
        },
    });
});