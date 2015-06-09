define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui',
        'Components/Chat', 'Components/MapPicker'],
function(_, React, ReactBootstrap, interop, events, ui,
         Chat, MapPicker) {

    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    var ListGroup = ReactBootstrap.ListGroup;
    var ListGroupItem = ReactBootstrap.ListGroupItem;
    var ModalTrigger = ReactBootstrap.ModalTrigger;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var OptionList = ui.OptionList;
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
            var _this = this;

            return (
                <div>
                    <Div nonBlocking pos={pos(10,10)} size={width(80)}>
                        <Well style={{'height':'80%'}}>
                            <h1>
                                {this.props.params.victory ? 'Victory!' : 'Defeat!'}
                            </h1>

                            {this.props.params.winningTeam ?
                            <h2>
                                 Team {this.props.params.winningTeam} wins!
                            </h2>
                            : null}

                            {/* Buttons */}
                            <Div nonBlocking pos={pos(36,72)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        <Button onClick={leaveGame}>Leave Game</Button>
                                    </p>
                                </div>
                            </Div>
                        </Well>
                    </Div>
                </div>
            );
        }
    });
}); 