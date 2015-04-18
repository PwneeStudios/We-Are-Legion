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

    var ChatInput = React.createClass({displayName: "ChatInput",
        mixins: [RenderAtMixin, events.ShowUpdateMixin],
                
        onShowUpdate: function(values) {
            this.setState({
                ShowChat: values.ShowChat,
            });
        },

        getInitialState: function() {
            return {
                value: '',
            };
        },
    
        onTextChange: function() {
            this.setState({
                value:this.refs.input.getValue()
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
            if (keyCode == '13') {
                if (interop.InXna()) {
                    var message = this.refs.input.getInputDOMNode().value;
                    xna.OnChatEnter(message);
                    
                    this.setState({
                        value:'',
                    });
                }
            }
        },
        
        renderAt: function() {
            if (this.state.ShowChat) {
                return (
                    React.createElement("div", null, 
                        React.createElement(Input, {value: this.state.value, ref: "input", type: "text", addonBefore: "All", 
                         style: {'pointer-events':'auto'}, 
                         onChange: this.onTextChange, onKeyDown: this.onKeyDown, 
                         onMouseOver: interop.onOver, onMouseLeave: interop.onLeave, 
                         onBlur: this.focus})
                     )
                );
            } else {
                return (
                    React.createElement("div", null
                    )
                )
            }
        },
    });

    var ChatLine = React.createClass({displayName: "ChatLine",
        getDefaultProps: function() {
            return {
                willFadeOut:true,
            }
        },

        componentDidMount: function() {
            var self = this;
            
            //this.alpha = 0;
            //this.fadeIn();
            
            this.alpha = 1;

            if (this.props.willFadeOut) {
                setTimeout(function() {
                    self.fadeOut();
                }, 4500);
            }
        },
        
        fadeIn: function() {
            this.alpha += .05;
            if (this.alpha > 1) {
                this.alpha = 1;
            } else {
                setTimeout(this.fadeIn, 16);
            }
            
            this.getDOMNode().style.opacity = this.alpha;
        },

        fadeOut: function() {
            this.alpha -= .05;
            if (this.alpha < 0) {
                this.alpha = 0;
                this.props.remove(this.props.message);
            } else {
                this.getDOMNode().style.opacity = this.alpha;
                setTimeout(this.fadeOut, 16);
            }
        },

        render: function() {
            var message = this.props.message;
            
            return (
                React.createElement("p", {className: "chat", style: {opacity:1}}, 
                    React.createElement("span", {style: {color:'rgba(180,180,255,255)'}}, message.name, ": "), 
                    React.createElement("span", null, message.message), 
                    React.createElement("br", null)
                )
            );
        },
    });
    
    var ChatBox = React.createClass({displayName: "ChatBox",
        mixins: [RenderAtMixin, events.OnChatMixin],

        onChatMessage: function(message) {
            var self = this;
            
            message.index = this.state.counter++;
            
            self.state.messages.push(message);
            self.setState({messages: self.state.messages});
        },
        
        remove: function(message) {
            _.remove(this.state.messages, function(e) {
                return e.index === message.index;
            });
            this.setState({messages: this.state.messages});
        },
        
        getInitialState: function() {
            return {
                //messages: [{index:1, message:'hello there'}, {index:2, message:'hello there again'}, {index:3, message:'hello there etc'}],
                messages: [],
                counter: 0,
            };
        },
    
        componentDidMount: function() {
            var self = this;
            return;
            setInterval(function() {
                self.onChatMessage({name:'player 1', message:'hello there again ' + self.state.counter});
            }, 1000);
        },
    
        renderAt: function() {
            var self = this;

            var messages = _.map(this.state.messages, function(message) {
                return (
                    React.createElement(ChatLine, {key: message.index, message: message, remove: self.remove})
                );
            });

            return (
                React.createElement("div", {style: {'position':'relative','width':'100%'}}, 
                    React.createElement("div", {style: {'position':'absolute','bottom':'0','width':'100%'}}, 
                        messages
                    )
                )
            );
        },
    });
});