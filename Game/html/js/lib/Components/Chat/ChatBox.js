define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat/ChatLine'], function(_, React, ReactBootstrap, interop, events, ui, ChatLine) {
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