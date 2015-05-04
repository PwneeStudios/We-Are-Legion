define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var RenderAtMixin = ui.RenderAtMixin;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var make = function(english, chinese, value) {
        return ({
            name:
                <span>
                    {english}
                    <span style={{'text-align':'right', 'float':'right'}}>{chinese}</span>
                </span>,

            selectedName: english,
            value: value,
        });
    };

    var kingdomChoices = [
        make('Kingdom of Wei',   '魏', 1),
        make('Kingdom of Shu',   '蜀', 3),
        make('Kingdom of Wu',    '吳', 4),
        make('Kingdom of Beast', '獸', 2),
    ];

    var teamChoices = [
        make('Team 1', '一', 1),
        make('Team 2', '二', 3),
        make('Team 3', '三', 4),
        make('Team 4', '四', 2),
    ];

    var Choose = React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
                selected: this.props.choices[0],
            };
        },
        
        onSelected: function(item) {
            this.setState({
                selected: item,
            });
        },

        render: function() {
            if (this.props.activePlayer == this.props.player) {
                return (
                    <Dropdown selected={this.state.selected} choices={this.props.choices} onSelected={this.onSelected} />
                );
            } else {
                return (
                    <span>{this.props.choices[0].selectedName}</span>
                );
            }
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
                    <td><Choose choices={teamChoices} default='Choose team' {...this.props} /></td>
                    <td><Choose choices={kingdomChoices} default='Choose kingdom' {...this.props} /></td>
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
            var _this = this;

            return (
                <div>
                    <Div nonBlocking pos={pos(10,5)} size={width(80)}>
                        <Panel>
                            <h2>
                                Game Lobby
                            </h2>
                        </Panel>

                        <Well style={{'height':'75%'}}>

                            {/* Chat */}
                            <Chat.ChatBox show full pos={pos(2, 17)} size={size(43,61)}/>
                            <Chat.ChatInput show pos={pos(2,80)} size={width(43)} />

                            {/* Player Table */}
                            <Div nonBlocking pos={pos(48,16.9)} size={width(50)} style={{'pointer-events':'auto', 'font-size': '1.4%;'}}>
                                <Table style={{width:'100%'}}><tbody>
                                    {_.map(_.range(1, 5), function(i) {
                                        return <PlayerEntry player={i} activePlayer={_this.props.lobbyPlayerNum} />;
                                    })}
                                </tbody></Table>
                            </Div>

                            {/* Map */}
                            <Div nonBlocking pos={pos(38,68)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        {this.props.host ? <Button>Choose map...</Button> : null}
                                    </p>
                                </div>
                            </Div>

                            {/* Buttons */}
                            <Div nonBlocking pos={pos(38,80)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        {this.props.host ? <Button>Start Game</Button> : null}
                                        &nbsp;
                                        <Button onClick={back}>Leave Lobby</Button>
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