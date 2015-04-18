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
                <div>
                    <Div nonBlocking pos={pos(10,10)} size={width(60)}>
                        <Panel>
                            <p>
                                Game Lobby
                            </p>
                        </Panel>

                        <Chat.ChatBox show pos={pos(.38, 78)} size={width(38)}/>
                        <Chat.ChatInput show pos={pos(.35,80)} size={width(49)} />

                        <Div nonBlocking pos={pos(40,80)} size={width(60)}>
                            <div style={{'float':'right', 'pointer-events':'auto'}}>
                                <p>
                                    <Button>Start Game</Button>
                                    &nbsp;
                                    <Button>Leave Lobby</Button>
                                </p>
                            </div>
                        </Div>
                    </Div>
                </div>
            );
        }
    });
});