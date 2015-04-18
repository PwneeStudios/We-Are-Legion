define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Input = ReactBootstrap.Input;
    var Popover = ReactBootstrap.Popover;
    
    var Div = ui.Div;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var RenderAtMixin = ui.RenderAtMixin;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(Div, {pos: pos(15,0)}, 
                        React.createElement(Chat.ChatInput, {show: true, pos: pos(.35,80), size: width(49)}), 
                        React.createElement(Chat.ChatBox, {show: true, pos: pos(.38, 78), size: width(38)})
                    )
                )
            );
        }
    });
});