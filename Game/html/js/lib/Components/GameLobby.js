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
                React.createElement("span", null, 
                    english, 
                    React.createElement("span", {style: {'text-align':'right', 'float':'right'}}, chinese)
                ),

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
        make('Team 2', '二', 2),
        make('Team 3', '三', 3),
        make('Team 4', '四', 4),
    ];

    var Choose = React.createClass({displayName: "Choose",
        mixins: [],
                
        getInitialState: function() {
            var self = this;
            var selected = this.props.choices[0];

            _.forEach(this.props.choices, function(choice) {
                if (choice.value === self.props.value) {
                    selected = choice;
                }
            });

            return {
                selected: selected,
            };
        },
        
        onSelected: function(item) {
            console.log('something selected');
            if (interop.InXna()) {
                console.log('select team');
                xna.SelectTeam(this.props.player, item.value);
            }

            this.setState({
                selected: item,
            });
        },

        render: function() {
            if (this.props.activePlayer === this.props.info.SteamID) {
                return (
                    React.createElement(Dropdown, {disabled: this.props.disabled, 
                              selected: this.state.selected, 
                              choices: this.props.choices, 
                              onSelect: this.onSelected})
                );
            } else {
                return (
                    React.createElement("span", null, this.state.selected.selectedName)
                );
            }
        },
    });

    var PlayerEntry = React.createClass({displayName: "PlayerEntry",
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            if (this.props.info.Name) {
                return (
                    React.createElement("tr", null, 
                        React.createElement("td", null, this.props.info.Name), 
                        React.createElement("td", null, React.createElement(Choose, React.__spread({disabled: this.props.disabled, choices: teamChoices, value: this.props.info.GameTeam, default: "Choose team"},  this.props))), 
                        React.createElement("td", null, React.createElement(Choose, React.__spread({disabled: this.props.disabled, choices: kingdomChoices, value: this.props.info.GamePlayer, default: "Choose kingdom"},  this.props)))
                    )
                );
            } else {
                return (
                    React.createElement("tr", null, 
                        React.createElement("td", null, "Slot open"), 
                        React.createElement("td", null), 
                        React.createElement("td", null)
                    )
                );
            }
        },
    });

    return React.createClass({
        mixins: [events.LobbyMixin, events.LobbyMapMixin],

        onLobbyUpdate: function(values) {
            console.log(values);
            console.log(values.LobbyInfo);

            this.setState({
                loading: false,
                lobbyInfo: values.LobbyInfo,
                activePlayer: values.SteamID,
            });
        },

        onLobbyMapUpdate: function(values) {
            console.log('onLobbyMapUpdate');
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
                var values = {
                    SteamID: 5,
                    LobbyInfo:
                        {"Players":[{"LobbyIndex":0,"Name":'some person',"SteamID":0,"GamePlayer":2,"GameTeam":3},{"LobbyIndex":0,"Name":'Me!',"SteamID":5,"GamePlayer":3,"GameTeam":4},{"LobbyIndex":0,"Name":null,"SteamID":0,"GamePlayer":0,"GameTeam":0},{"LobbyIndex":0,"Name":null,"SteamID":0,"GamePlayer":0,"GameTeam":0}]}
                };

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

        componentDidMount: function() {
            interop.drawMapPreviewAt(2.66, 0.554, 0.22, 0.22);
        },
        
        componentWillUnmount: function() {
            interop.hideMapPreview();
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

        render: function() {
            var _this = this;

            console.log('render the lobby');

            if (this.state.loading) {
                return (
                    React.createElement("div", null
                    )
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
                React.createElement("div", null, 
                    React.createElement(Div, {nonBlocking: true, pos: pos(10,5), size: width(80)}, 
                        React.createElement(Panel, null, 
                            React.createElement("h2", null, 
                                "Game Lobby"
                            )
                        ), 

                        React.createElement(Well, {style: {'height':'75%'}}, 

                            /* Chat */
                            React.createElement(Chat.ChatBox, {ref: "chat", show: true, full: true, pos: pos(2, 17), size: size(43,61)}), 
                            React.createElement(Chat.ChatInput, {show: true, lobbyChat: true, pos: pos(2,80), size: width(43)}), 

                            /* Player Table */
                            React.createElement(Div, {nonBlocking: true, pos: pos(48,16.9), size: width(50), style: {'pointer-events':'auto', 'font-size': '1.4%;'}}, 
                                React.createElement(Table, {style: {width:'100%'}}, React.createElement("tbody", null, 
                                    _.map(_.range(1, 5), function(i) {
                                        return (
                                            React.createElement(PlayerEntry, {disabled: disabled, 
                                                         player: i, 
                                                         info: _this.state.lobbyInfo.Players[i-1], 
                                                         activePlayer: _this.state.activePlayer})
                                         );
                                    })
                                ))
                            ), 

                            /* Map */
                            React.createElement(Div, {nonBlocking: true, pos: pos(38,68), size: width(60)}, 
                                React.createElement("div", {style: {'float':'right', 'pointer-events':'auto'}}, 
                                    React.createElement("p", null, 
                                        this.props.params.host ? 
                                            React.createElement(ModalTrigger, {modal: React.createElement(MapPicker, {onPick: this.onMapPick})}, 
                                                React.createElement(Button, {disabled: disabled, bsStyle: "primary", bsSize: "large"}, 
                                                    "Choose map..."
                                                )
                                            )
                                            : null
                                    )
                                )
                            ), 

                            /* Game visibility type */
                            this.props.params.host ? 
                                React.createElement(Div, {pos: pos(48,43), size: size(24,66.2)}, 
                                    React.createElement(OptionList, {disabled: disabled, options: visibility})
                                )
                                : null, 

                            /* Buttons */
                            React.createElement(Div, {nonBlocking: true, pos: pos(38,80), size: width(60)}, 
                                React.createElement("div", {style: {'float':'right', 'pointer-events':'auto'}}, 
                                    React.createElement("p", null, 
                                        this.props.params.host ?
                                            React.createElement(Button, {disabled: preventStart, onClick: this.onClickStart}, "Start Game")
                                            : null, 
                                        " ", 
                                        React.createElement(Button, {disabled: disabled, onClick: back}, "Leave Lobby")
                                    )
                                )
                            )

                        )
                    )
                )
            );
        }
    });
}); 