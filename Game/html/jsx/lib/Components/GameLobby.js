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
        return {
            name: (
                <span>
                    {english}
                    <span style={{'text-align':'right', 'float':'right'}} >
                        {chinese}
                    </span>
                </span>
            ),

            selectedName: english,
            value: value,
            taken: false,
        };
    };

    var kingdomChoices = [
        make('Kingdom of Wei',   '魏', 1),
        make('Kingdom of Shu',   '蜀', 3),
        make('Kingdom of Wu',    '吳', 4),
        make('Kingdom of Beast', '獸', 2),
    ];

    var teamChoices = [
        make('Team 1', '一', 1),
        make('Team 2', '二', 2),
        make('Team 3', '三', 3),
        make('Team 4', '四', 4),
    ];

    var arrayClone = function(l) {
        var copy = [];
        for (var i = 0; i < l.length; i++) {
            copy.push(_.clone(l[i]));
        }

        return copy;
    };

    var Choose = React.createClass({
        mixins: [],

        componentWillReceiveProps: function(nextProps) {
            this.setState(this.getInitialState(nextProps));
        },
                
        getInitialState: function(props) {
            if (typeof props === 'undefined') {
                props = this.props;
            }

            var self = this;
            var selected = props.choices[0];

            _.forEach(props.choices, function(choice) {
                if (choice.value === props.value) {
                    selected = choice;
                }
            });

            return {
                selected: selected,
            };
        },
        
        onSelected: function(item) {
            if (this.props.onSelect) {
                this.props.onSelect(item);
            }

            this.setState({
                selected: item,
            });
        },

        render: function() {
            if (this.props.activePlayer === this.props.info.SteamID) {
                return (
                    <Dropdown disabled={this.props.disabled}
                              selected={this.state.selected}
                              choices={this.props.choices}
                              onSelect={this.onSelected} />
                );
            } else {
                return (
                    <span>{this.state.selected.selectedName}</span>
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

        selectTeam: function(item) {
            if (interop.InXna()) {
                xna.SelectTeam(item.value);
            }
        },

        selectKingdom: function(item) {
            if (interop.InXna()) {
                xna.SelectKingdom(item.value);
            }
        },
        
        render: function() {
            if (this.props.info.Name) {
                var myTeamChoices = arrayClone(teamChoices);
                var myKingdomChoices = arrayClone(kingdomChoices);
                for (i = 1; i < 5; i++) {
                    if (i === this.props.player) continue;

                    var player = this.props.players[i-1];
                    if (player.SteamID === 0) continue;

                    _.find(myTeamChoices, 'value', player.GameTeam).taken = true;
                    _.find(myKingdomChoices, 'value', player.GamePlayer).taken = true;
                }

                return (
                    <tr>
                        <td>{this.props.info.Name}</td>
                        <td>
                            <Choose onSelect={this.selectTeam} disabled={this.props.disabled} choices={myTeamChoices}
                                    value={this.props.info.GameTeam} default='Choose team' {...this.props} />
                        </td>
                        <td>
                            <Choose onSelect={this.selectKingdom} disabled={this.props.disabled} choices={myKingdomChoices}
                                    value={this.props.info.GamePlayer} default='Choose kingdom' {...this.props} />
                        </td>
                    </tr>
                );
            } else {
                return (
                    <tr>
                        <td>Slot open</td>
                        <td></td>
                        <td></td>
                    </tr>
                );
            }
        },
    });

    return React.createClass({
        mixins: [events.LobbyMixin, events.LobbyMapMixin],

        onLobbyUpdate: function(values) {
            if (!this.state.loading && values.LobbyLoading) {
                return;
            }

            //console.log('values');
            //console.log(JSON.stringify(values));
            //console.log('---');

            if (values.CountDownStarted && !this.state.starting) {
                this.startStartGameCountdown();
            }

            this.setState({
                loading: values.LobbyLoading || false,
                name: values.LobbyName || this.state.name || '',
                lobbyInfo: values.LobbyInfo ? JSON.parse(values.LobbyInfo) : this.state.lobbyInfo || null,
                activePlayer: values.SteamID || this.state.activePlayer,
                maps: values.Maps || this.state.maps,
                map: 'Beset',
            });
        },

        onLobbyMapUpdate: function(values) {
            var mapLoading = values.LobbyMapLoading;

            if (mapLoading === this.state.mapLoading) {
                return;
            }

            this.setState({
                mapLoading: mapLoading,
            });
        },

        joinLobby: function() {
            if (!interop.InXna()) {
                values =
                    {"SteamID":100410705,"LobbyName":"Cookin' Ash's lobby","Maps":['Beset', 'Clash of Madness', 'Nice'],"LobbyInfo":"{\"Players\":[{\"LobbyIndex\":0,\"Name\":\"Cookin' Ash\",\"SteamID\":100410705,\"GamePlayer\":1,\"GameTeam\":1},{\"LobbyIndex\":0,\"Name\":\"other player\",\"SteamID\":12,\"GamePlayer\":2,\"GameTeam\":3},{\"LobbyIndex\":0,\"Name\":null,\"SteamID\":0,\"GamePlayer\":0,\"GameTeam\":0},{\"LobbyIndex\":0,\"Name\":null,\"SteamID\":0,\"GamePlayer\":0,\"GameTeam\":0}]}","LobbyLoading":false};

                setTimeout(function() {
                    window.lobby(values);
                }, 100);

                return;
            }

            if (this.props.params.host) {
                interop.createLobby();
            } else {
                interop.joinLobby(this.props.params.lobbyIndex);
            }
        },

        getInitialState: function() {
            this.joinLobby();

            return {
                loading: true,
                lobbyPlayerNum: 3,
                mapLoading: false,
            };
        },

        componentWillUnmount: function() {
            interop.hideMapPreview();
        },

        componentDidUpdate: function() {
            this.drawMap();
        },

        componentDidMount: function() {
            this.drawMap();
        },

        drawMap: function() {
            if (!this.state.loading) {
                interop.drawMapPreviewAt(2.66, 0.554, 0.22, 0.22);
            }
        },

        startGame: function() {
            if (interop.InXna()) {
                xna.StartGame();
            }
        },

        onClickStart: function() {
            if (interop.InXna()) {
                xna.StartGameCountdown();
            } else {
                setTimeout(function() {
                    window.lobby({
                        CountDownStarted:true
                    });
                }, 100);
            }
        },

        startStartGameCountdown: function() {
            this.setState({
                starting:true,
            });

            this.countDown();
        },

        countDown: function() {
            //this.startGame(); return;

            var _this = this;

            this.addMessage('Game starting in...');
            setTimeout(function() { _this.addMessage('3...'); }, 1000);
            setTimeout(function() { _this.addMessage('2...'); }, 2000);
            setTimeout(function() { _this.addMessage('1...'); }, 3000);
            setTimeout(_this.startGame, 4000);
        },

        addMessage: function(msg) {
            if (this.refs.chat && this.refs.chat.onChatMessage) {
                this.refs.chat.onChatMessage({message:msg,name:''});
            }
        },

        onMapPick: function(map) {
            console.log(map);
            interop.setMap(map);
        },

        leaveLobby: function() {
            interop.leaveLobby();
            back();
        },

        render: function() {
            var _this = this;

            if (this.state.loading) {
                return (
                    <div>
                    </div>
                );
            }

            var visibility = [
                {name:'Public game', value:'public'},
                {name:'Friends only', value:'friend'},
                {name:'Private', value:'private'},
            ];

            var disabled = this.state.starting;
            var preventStart = this.state.starting || this.state.mapLoading;

            return (
                <div>
                    <Div nonBlocking pos={pos(10,5)} size={width(80)}>
                        <Panel>
                            <h2>
                                {this.state.name}
                            </h2>
                        </Panel>

                        <Well style={{'height':'75%'}}>

                            {/* Chat */}
                            <Chat.ChatBox ref='chat' show full pos={pos(2, 17)} size={size(43,61)}/>
                            <Chat.ChatInput show lobbyChat pos={pos(2,80)} size={width(43)} />

                            {/* Player Table */}
                            <Div nonBlocking pos={pos(48,16.9)} size={width(50)} style={{'pointer-events':'auto', 'font-size': '1.4%;'}}>
                                <Table style={{width:'100%'}}><tbody>
                                    {_.map(_.range(1, 5), function(i) {
                                        return (
                                            <PlayerEntry disabled={disabled}
                                                         player={i}
                                                         info={_this.state.lobbyInfo.Players[i-1]}
                                                         players={_this.state.lobbyInfo.Players}
                                                         activePlayer={_this.state.activePlayer} />
                                         );
                                    })}
                                </tbody></Table>
                            </Div>

                            {/* Map */}
                            <Div nonBlocking pos={pos(38,68)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        {this.props.params.host ? 
                                            <ModalTrigger modal={<MapPicker maps={this.state.maps} onPick={this.onMapPick} />}>
                                                <Button disabled={disabled} bsStyle='primary' bsSize='large'>
                                                    Choose map...
                                                </Button>
                                            </ModalTrigger>
                                            : null}
                                    </p>
                                </div>
                            </Div>

                            {/* Game visibility type */}
                            {this.props.params.host ? 
                                <Div pos={pos(48,43)} size={size(24,66.2)}>
                                    <OptionList disabled={disabled} options={visibility} />
                                </Div>
                                : null}

                            {/* Buttons */}
                            <Div nonBlocking pos={pos(38,80)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        {this.props.params.host ?
                                            <Button disabled={preventStart} onClick={this.onClickStart}>Start Game</Button>
                                            : null}
                                        &nbsp;
                                        <Button disabled={disabled} onClick={this.leaveLobby}>Leave Lobby</Button>
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