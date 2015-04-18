define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var DropdownButton = ReactBootstrap.DropdownButton;
    var MenuItem = ReactBootstrap.MenuItem;
    var Table = ReactBootstrap.Table;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var RenderAtMixin = ui.RenderAtMixin;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var ChooseKingdom = React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            return (
                <div style={{'pointer-events':'auto'}}>
                    <DropdownButton title='Choose your Kingdom'>
                        <MenuItem>Kingdom of Wei</MenuItem>
                        <MenuItem>Kingdom of Shu</MenuItem>
                        <MenuItem>Kingdom of Shen</MenuItem>
                        <MenuItem>Kingdom of Beast</MenuItem>
                    </DropdownButton>
                </div>
            );
        },
    });

    var ChooseTeam = React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            return (
                <div style={{'pointer-events':'auto'}}>
                    <DropdownButton title='Team 1'>
                        {_.map(_.range(1, 5), function(i) { return (
                            <MenuItem>
                                Team {i}
                            </MenuItem>
                        );})}
                    </DropdownButton>
                </div>
            );
        },
    });

    var PlayerEntry = React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            return (
                <tr>
                    <td>Player {this.props.player}</td>
                    <td><ChooseTeam /></td>
                    <td><ChooseKingdom /></td>
                </tr>
            );
        },
    });

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            return (
                <div>
                    <Div nonBlocking pos={pos(10,5)} size={width(80)}>
                        <Panel>
                            <h2>
                                Game Lobby
                            </h2>
                        </Panel>

                        <Well style={{'height':'75%'}}>
                            <Chat.ChatBox show pos={pos(2, 78)} size={width(43)}/>
                            <Chat.ChatInput show pos={pos(2,80)} size={width(43)} />

                            <Div nonBlocking pos={pos(48,20)} size={width(50)} style={{'pointer-events':'auto', 'font-size': '1.4%;'}}>
                                <Table style={{width:'100%'}}><tbody>
                                    {_.map(_.range(1, 5), function(i) { return <PlayerEntry player={i} />; })}
                                </tbody></Table>
                            </Div>

                            <Div nonBlocking pos={pos(38,80)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        <Button>Start Game</Button>
                                        &nbsp;
                                        <Button>Leave Lobby</Button>
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