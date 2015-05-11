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
                lobbyPlayerNum: 3,
            };
        },

        startGame: function() {
            if (interop.InXna()) {
                xna.StartGame();
            }
        },

        onClickStart: function() {
            this.setState({
                starting:true,
            });

            this.countDown();
        },

        countDown: function() {
            this.startGame();return;
            
            var _this = this;

            this.addMessage('Game starting in...');
            setTimeout(function() { _this.addMessage('3...'); }, 1000);
            setTimeout(function() { _this.addMessage('2...'); }, 2000);
            setTimeout(function() { _this.addMessage('1...'); }, 3000);
            setTimeout(_this.startGame, 4000);
        },

        addMessage: function(msg) {
            this.refs.chat.onChatMessage({message:msg,name:''});
        },

        render: function() {
            var _this = this;

            var visibility = [
                {name:'Public game', value:'public'},
                {name:'Friends only', value:'friend'},
                {name:'Private', value:'private'},
            ];

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
                            <Chat.ChatBox ref='chat' show full pos={pos(2, 17)} size={size(43,61)}/>
                            <Chat.ChatInput show pos={pos(2,80)} size={width(43)} />

                            {/* Player Table */}
                            <Div nonBlocking pos={pos(48,16.9)} size={width(50)} style={{'pointer-events':'auto', 'font-size': '1.4%;'}}>
                                <Table style={{width:'100%'}}><tbody>
                                    {_.map(_.range(1, 5), function(i) {
                                        return <PlayerEntry player={i} activePlayer={_this.state.lobbyPlayerNum} />;
                                    })}
                                </tbody></Table>
                            </Div>

                            {/* Map */}
                            <Div nonBlocking pos={pos(38,68)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        {this.props.params.host ? 
                                            <ModalTrigger modal={<MapPicker />}>
                                                <Button bsStyle='primary' bsSize='large'>Choose map...</Button>
                                            </ModalTrigger>
                                            : null}
                                    </p>
                                </div>
                            </Div>

                            {/* Game visibility type */}
                            {this.props.params.host ? 
                                <Div pos={pos(48,43)} size={size(24,66.2)}>
                                    <OptionList options={visibility} />
                                </Div>
                                : null}

                            {/* Buttons */}
                            <Div nonBlocking pos={pos(38,80)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        {this.props.params.host ?
                                            <Button disabled={this.state.starting} onClick={this.onClickStart}>Start Game</Button>
                                            : null}
                                        &nbsp;
                                        <Button disabled={this.state.starting} onClick={back}>Leave Lobby</Button>
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