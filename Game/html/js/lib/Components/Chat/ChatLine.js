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
});