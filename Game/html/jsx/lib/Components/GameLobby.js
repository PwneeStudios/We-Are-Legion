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
                <div>
                    <Div pos={pos(15,0)}>
                        <Chat.ChatInput show pos={pos(.35,80)} size={width(49)} />
                        <Chat.ChatBox show pos={pos(.38, 78)} size={width(38)}/>
                    </Div>
                </div>
            );
        }
    });
});