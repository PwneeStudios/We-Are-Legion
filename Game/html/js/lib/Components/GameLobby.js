define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Popover = ReactBootstrap.Popover;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
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
                    React.createElement(Div, {nonBlocking: true, pos: pos(10,10), size: width(60)}, 
                        React.createElement(Panel, null, 
                            React.createElement("p", null, 
                                "Game Lobby"
                            )
                        ), 

                        React.createElement(Chat.ChatBox, {show: true, pos: pos(.38, 78), size: width(38)}), 
                        React.createElement(Chat.ChatInput, {show: true, pos: pos(.35,80), size: width(49)}), 

                        React.createElement(Div, {nonBlocking: true, pos: pos(40,80), size: width(60)}, 
                            React.createElement("div", {style: {'float':'right', 'pointer-events':'auto'}}, 
                                React.createElement("p", null, 
                                    React.createElement(Button, null, "Start Game"), 
                                    " ", 
                                    React.createElement(Button, null, "Leave Lobby")
                                )
                            )
                        )
                    )
                )
            );
        }
    });
});