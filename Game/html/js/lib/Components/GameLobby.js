'use strict';

var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat', 'Components/MapPicker'], function (_, React, ReactBootstrap, interop, events, ui, Chat, MapPicker) {

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

    var make = function make(english, chinese, value) {
        return {
            name: React.createElement(
                'span',
                null,
                english,
                React.createElement(
                    'span',
                    { style: { 'text-align': 'right', 'float': 'right' } },
                    chinese
                )
            ),

            selectedName: english,
            value: value,
            taken: false
        };
    };

    var kingdomChoices = [make('Kingdom of Wei', '魏', 1), make('Kingdom of Shu', '蜀', 3), make('Kingdom of Wu', '吳', 4), make('Kingdom of Beast', '獸', 2)];

    var teamChoices = [make('Team 1', '一', 1), make('Team 2', '二', 2), make('Team 3', '三', 3), make('Team 4', '四', 4)];

    var arrayClone = function arrayClone(l) {
        var copy = [];
        for (var i = 0; i < l.length; i++) {
            copy.push(_.clone(l[i]));
        }

        return copy;
    };

    var Choose = React.createClass({
        displayName: 'Choose',

        mixins: [],

        componentWillReceiveProps: function componentWillReceiveProps(nextProps) {
            this.setState(this.getInitialState(nextProps));
        },

        getInitialState: function getInitialState(props) {
            if (typeof props === 'undefined') {
                props = this.props;
            }

            var self = this;
            var selected = props.choices[0];

            _.forEach(props.choices, function (choice) {
                if (choice.value === props.value) {
                    selected = choice;
                }
            });

            return {
                selected: selected
            };
        },

        onSelected: function onSelected(item) {
            if (this.props.onSelect) {
                this.props.onSelect(item);
            }

            this.setState({
                selected: item
            });
        },

        render: function render() {
            if (this.props.activePlayer === this.props.info.SteamID) {
                return React.createElement(Dropdown, { disabled: this.props.disabled,
                    selected: this.state.selected,
                    choices: this.props.choices,
                    onSelect: this.onSelected });
            } else {
                return React.createElement(
                    'span',
                    null,
                    this.state.selected.selectedName
                );
            }
        }
    });

    var PlayerEntry = React.createClass({
        displayName: 'PlayerEntry',

        mixins: [],

        getInitialState: function getInitialState() {
            return {};
        },

        selectTeam: function selectTeam(item) {
            if (interop.InXna()) {
                interop.selectTeam(item.value);
            }
        },

        selectKingdom: function selectKingdom(item) {
            if (interop.InXna()) {
                interop.selectKingdom(item.value);
            }
        },

        render: function render() {
            if (this.props.info.Name) {
                var myTeamChoices = arrayClone(teamChoices);
                var myKingdomChoices = arrayClone(kingdomChoices);
                var numPlayers = this.props.player.length + 1;

                for (var i = 1; i < numPlayers; i++) {
                    if (i === this.props.player) continue;

                    var player = this.props.players[i - 1];
                    if (player.SteamID === 0) continue;

                    // Use this if you want to prevent taken teams from being double picked.
                    //_.find(myTeamChoices, 'value', player.GameTeam).taken = true;

                    // Use this if you want to prevent taken kingdoms from being double picked.
                    // NOTE: multiple players playing as the same kingdom is not supported by the engine.
                    _.find(myKingdomChoices, 'value', player.GamePlayer).taken = true;
                }

                return React.createElement(
                    'tr',
                    null,
                    React.createElement(
                        'td',
                        null,
                        this.props.info.Name
                    ),
                    React.createElement(
                        'td',
                        null,
                        React.createElement(Choose, _extends({ onSelect: this.selectTeam, disabled: this.props.disabled, choices: myTeamChoices,
                            value: this.props.info.GameTeam, 'default': 'Choose team' }, this.props))
                    ),
                    React.createElement(
                        'td',
                        null,
                        React.createElement(Choose, _extends({ onSelect: this.selectKingdom, disabled: this.props.disabled, choices: myKingdomChoices,
                            value: this.props.info.GamePlayer, 'default': 'Choose kingdom' }, this.props))
                    )
                );
            } else {
                return React.createElement(
                    'tr',
                    null,
                    React.createElement(
                        'td',
                        null,
                        'Slot open'
                    ),
                    React.createElement('td', null),
                    React.createElement('td', null)
                );
            }
        }
    });

    return React.createClass({
        mixins: [events.LobbyMixin, events.JoinFailedMixin, events.LobbyMapMixin],

        //events.AllowBackMixin,
        onLobbyUpdate: function onLobbyUpdate(values) {
            log('lobby update!');

            if (!this.state.loading && values.LobbyLoading) {
                return;
            }

            log('should we startStartGameCountdown?');
            log(JSON.stringify(values));
            log(values.CountDownStarted);
            log(values.GameStarted);
            log(this.state.starting);
            log(!this.state.starting);
            log(values.CountDownStarted && !this.state.starting);
            log(values.CountDownStarted && !this.state.starting && !values.GameStarted);
            log('----------');
            log(values.CountDownStarted);
            log(values.GameStarted);
            log(this.state.starting);
            log(!this.state.starting);
            log(values.CountDownStarted && !this.state.starting);
            log(values.CountDownStarted && !this.state.starting && !values.GameStarted);
            log('----------');

            if (values.CountDownStarted && !this.state.starting && !values.GameStarted) {
                log('startStartGameCountdown');
                log(JSON.stringify(values));
                log(values.CountDownStarted);
                log(values.GameStarted);
                log(this.state.starting);
                this.startStartGameCountdown();
                log(values.CountDownStarted && !this.state.starting && !values.GameStarted);
            }

            var lobbyInfo = values.LobbyInfo ? JSON.parse(values.LobbyInfo) : this.state.lobbyInfo || null;
            var player = null;
            if (lobbyInfo && lobbyInfo.Players) {
                for (var i = 0; i < lobbyInfo.Players.length; i++) {
                    if (lobbyInfo.Players[i].SteamID === values.SteamID) {
                        player = lobbyInfo.Players[i];
                        log('found the player in players');
                    }
                }

                if (lobbyInfo.Spectators) {
                    for (var i = 0; i < lobbyInfo.Spectators.length; i++) {
                        if (lobbyInfo.Spectators[i].SteamID === values.SteamID) {
                            player = lobbyInfo.Spectators[i];
                            log('found the player in spectators');
                        }
                    }
                } else {
                    log('no spectators found');
                }
            }

            this.setState({
                loading: values.LobbyLoading || false,
                name: values.LobbyName || this.state.name || '',
                lobbyInfo: lobbyInfo,
                activePlayer: values.SteamID || this.state.activePlayer,
                player: player,
                maps: values.Maps || this.state.maps,
                map: 'Beset'
            });
        },

        onJoinFailed: function onJoinFailed() {
            back();
        },

        onLobbyMapUpdate: function onLobbyMapUpdate(values) {
            var mapLoading = values.LobbyMapLoading;

            if (mapLoading === this.state.mapLoading) {
                return;
            }

            this.setState({
                mapLoading: mapLoading
            });
        },

        joinLobby: function joinLobby() {
            if (!interop.InXna()) {
                var values = { "SteamID": 100410705, "LobbyName": "Cookin' Ash's lobby", "Maps": ['Beset', 'Clash of Madness', 'Nice'],
                    "LobbyInfo": "{\"Players\":[{\"LobbyIndex\":0,\"Name\":\"Cookin' Ash\",\"SteamID\":9100410705,\"GamePlayer\":1,\"GameTeam\":1},{\"LobbyIndex\":0,\"Name\":\"other player\",\"SteamID\":12,\"GamePlayer\":2,\"GameTeam\":3},{\"LobbyIndex\":0,\"Name\":null,\"SteamID\":0,\"GamePlayer\":0,\"GameTeam\":0},{\"LobbyIndex\":0,\"Name\":null,\"SteamID\":0,\"GamePlayer\":0,\"GameTeam\":0}],\"Spectators\":[{\"Spectator\":true,\"LobbyIndex\":0,\"Name\":\"Cookin' Ash\",\"SteamID\":100410705,\"GamePlayer\":1,\"GameTeam\":1},{\"LobbyIndex\":0,\"Name\":\"other player\",\"SteamID\":12,\"GamePlayer\":2,\"GameTeam\":3}]}",
                    "LobbyLoading": false
                };

                setTimeout(function () {
                    window.lobby(values);
                }, 100);

                return;
            }

            if (this.props.params.host) {
                interop.createLobby(this.props.params.type, this.props.params.training);
            } else {
                interop.joinLobby(this.props.params.lobbyIndex);
            }

            log('made the lobby');
        },

        getInitialState: function getInitialState() {
            this.joinLobby();

            return {
                loading: true,
                lobbyPlayerNum: 3,
                mapLoading: false
            };
        },

        componentWillUnmount: function componentWillUnmount() {
            interop.hideMapPreview();
        },

        componentDidUpdate: function componentDidUpdate() {
            this.drawMap();
        },

        componentDidMount: function componentDidMount() {
            window.onEscape = this.leaveLobby;
            interop.lobbyUiCreated();
            this.drawMap();
        },

        drawMap: function drawMap() {
            if (!this.state.loading) {
                interop.drawMapPreviewAt(2.66, 0.554, 0.22, 0.22);
            }
        },

        startGame: function startGame() {
            if (interop.InXna()) {
                interop.startGame();
            }
        },

        onClickStart: function onClickStart() {
            log('click start');

            if (interop.InXna()) {
                interop.startGameCountdown();
            } else {
                setTimeout(function () {
                    log('do lobby update with');
                    window.lobby({
                        CountDownStarted: true,
                        GameStarted: false
                    });
                }, 100);
            }
        },

        startStartGameCountdown: function startStartGameCountdown() {
            this.setState({
                starting: true
            });

            this.countDown();
        },

        countDown: function countDown() {
            //this.startGame(); return;

            var _this = this;

            this.addMessage('Game starting in...');
            setTimeout(function () {
                _this.addMessage('3...');
            }, 1000);
            setTimeout(function () {
                _this.addMessage('2...');
            }, 2000);
            setTimeout(function () {
                _this.addMessage('1...');
            }, 3000);
            setTimeout(_this.startGame, 4000);
        },

        addMessage: function addMessage(msg) {
            if (this.refs.chat && this.refs.chat.onChatMessage) {
                this.refs.chat.onChatMessage({ message: msg, name: '' });
            }
        },

        onMapPick: function onMapPick(map) {
            log(map);
            interop.setMap(map);
        },

        leaveLobby: function leaveLobby() {
            interop.leaveLobby();
            back();
        },

        onLobbyTypeSelect: function onLobbyTypeSelect(item) {
            interop.setLobbyType(item.value);
        },

        join: function join() {
            interop.join();
        },

        spectate: function spectate() {
            interop.spectate();
        },

        render: function render() {
            var _this = this;

            if (this.state.loading || !this.state.lobbyInfo || !this.state.lobbyInfo.Players) {
                return React.createElement('div', null);
            }

            var visibility = [{ name: 'Public game', value: 'public' }, { name: 'Friends only', value: 'friends' }, { name: 'Private', value: 'private' }];

            var disabled = this.state.starting;
            var preventStart = this.state.starting || this.state.mapLoading;
            var spectate = this.state.player && this.state.player.Spectator;
            var numPlayers = this.state.lobbyInfo.Players.length;

            if (spectate) {
                log('we are spectating');
            } else {
                log('we are in the game');
            }

            if (this.state.lobbyInfo.Spectators) {
                if (this.state.lobbyInfo.Spectators.length === 1) {
                    var spectators = '1 watcher';
                } else if (this.state.lobbyInfo.Spectators.length > 1) {
                    var spectators = this.state.lobbyInfo.Spectators.length + ' watchers';
                }
            }

            return React.createElement(
                'div',
                null,
                React.createElement(
                    Div,
                    { nonBlocking: true, pos: pos(10, 5), size: width(80) },
                    React.createElement(
                        Well,
                        { style: { 'height': '90%' } },
                        React.createElement(
                            'h2',
                            null,
                            this.state.name
                        ),
                        spectate ? React.createElement(
                            'h4',
                            null,
                            spectators,
                            ' (You are spectating)'
                        ) : React.createElement(
                            'h4',
                            null,
                            spectators
                        ),
                        React.createElement(Chat.ChatBox, { ref: 'chat', show: true, full: true, pos: pos(2, 15.5), size: size(43, 62.5) }),
                        React.createElement(Chat.ChatInput, { show: true, lobbyChat: true, pos: pos(2, 80), size: width(43) }),
                        React.createElement(
                            Div,
                            { nonBlocking: true, pos: pos(48, 53), size: width(50), style: { 'pointer-events': 'auto', 'font-size': '1.4%;' } },
                            React.createElement(
                                Table,
                                { style: { width: '100%' } },
                                React.createElement(
                                    'tbody',
                                    null,
                                    _.map(_.range(1, numPlayers + 1), function (i) {
                                        return React.createElement(PlayerEntry, {
                                            disabled: disabled,
                                            player: i,
                                            info: _this.state.lobbyInfo.Players[i - 1],
                                            players: _this.state.lobbyInfo.Players,
                                            activePlayer: _this.props.params.training ? null : _this.state.activePlayer
                                        });
                                    })
                                )
                            )
                        ),
                        this.props.params.host && !this.props.params.training ? React.createElement(
                            Div,
                            { pos: pos(48, 5.2), size: size(16.5, 66.2) },
                            React.createElement(OptionList, { disabled: disabled, options: visibility,
                                onSelect: this.onLobbyTypeSelect, value: this.props.params.type })
                        ) : null,
                        this.state.player ? React.createElement(
                            Div,
                            { nonBlocking: true, pos: pos(48, 80), size: width(60) },
                            React.createElement(
                                'div',
                                { style: { 'float': 'left', 'pointer-events': 'auto' } },
                                React.createElement(
                                    'p',
                                    null,
                                    spectate ? React.createElement(
                                        ui.Button,
                                        { disabled: disabled, onClick: this.join },
                                        'Join'
                                    ) : React.createElement(
                                        ui.Button,
                                        { disabled: disabled, onClick: this.spectate },
                                        'Spectate'
                                    )
                                )
                            )
                        ) : null,
                        React.createElement(
                            Div,
                            { nonBlocking: true, pos: pos(38, 80), size: width(60) },
                            React.createElement(
                                'div',
                                { style: { 'float': 'right', 'pointer-events': 'auto' } },
                                React.createElement(
                                    'p',
                                    null,
                                    this.props.params.host ? React.createElement(
                                        ModalTrigger,
                                        { modal: React.createElement(MapPicker, { maps: this.state.maps, onPick: this.onMapPick, training: this.props.params.training }) },
                                        React.createElement(
                                            ui.Button,
                                            { disabled: disabled, bsStyle: 'primary' },
                                            'Choose map...'
                                        )
                                    ) : null,
                                    ' ',
                                    this.props.params.host ? React.createElement(
                                        ui.Button,
                                        { disabled: preventStart, onClick: this.onClickStart },
                                        'Start Game'
                                    ) : null,
                                    ' ',
                                    React.createElement(
                                        ui.Button,
                                        { disabled: disabled, onClick: this.leaveLobby },
                                        'Leave Lobby'
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