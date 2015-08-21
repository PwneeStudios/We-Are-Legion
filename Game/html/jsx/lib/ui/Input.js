define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui/RenderAtMixin'], function(_, React, ReactBootstrap, interop, events, RenderAtMixin) {
    var Input = ReactBootstrap.Input;
    
    return React.createClass({
        mixins: [RenderAtMixin],
                
        componentWillReceiveProps: function(nextProps) {
            this.setState(this.getInitialState(nextProps));
        },
                
        getInitialState: function(props) {
            if (typeof props === 'undefined') {
                props = this.props;
            }

            return {
                value: props.value,
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
                <div>
                    <Input value={this.state.value} ref="input" type="text"
                     addonBefore={this.props.prefix}
                     style={style}
                     onChange={this.onTextChange} onKeyDown={this.onKeyDown}
                     onMouseOver={interop.onOver} onMouseLeave={interop.onLeave}
                     onBlur={this.focus}/>
                </div>
            );
        },
    });
});