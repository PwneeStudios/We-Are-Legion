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
        make('Team 2', '二', 3),
        make('Team 3', '三', 4),
        make('Team 4', '四', 2),
    ];

    var Choose = React.createClass({displayName: "Choose",
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
                    React.createElement(Dropdown, {disabled: this.props.disabled, 
                              selected: this.state.selected, 
                              choices: this.props.choices, 
                              onSelected: this.onSelected})
                );
            } else {
                return (
                    React.createElement("span", null, this.props.choices[0].selectedName)
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
            return (
                React.createElement("tr", null, 
                    React.createElement("td", null, "Player ", this.props.player), 
                    React.createElement("td", null, React.createElement(Choose, React.__spread({disabled: this.props.disabled, choices: teamChoices, default: "Choose team"},  this.props))), 
                    React.createElement("td", null, React.createElement(Choose, React.__spread({disabled: this.props.disabled, choices: kingdomChoices, default: "Choose kingdom"},  this.props)))
                )
            );
        },
    });

    return React.createClass({
        mixins: [events.LobbyMixin],

        onLobbyUpdate: function(values) {
            var loading = values.LobbyMapLoading;
            
            if (loading === this.state.loading) {
                return;
            }

            this.setState({
                loading:loading,
            });
        },
                
        getInitialState: function() {
            return {
                lobbyPlayerNum: 3,
                loading: false,
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

            var visibility = [
                {name:'Public game', value:'public'},
                {name:'Friends only', value:'friend'},
                {name:'Private', value:'private'},
            ];

            var disabled = this.state.starting || this.state.loading;

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
                            React.createElement(Chat.ChatInput, {show: true, pos: pos(2,80), size: width(43)}), 

                            /* Player Table */
                            React.createElement(Div, {nonBlocking: true, pos: pos(48,16.9), size: width(50), style: {'pointer-events':'auto', 'font-size': '1.4%;'}}, 
                                React.createElement(Table, {style: {width:'100%'}}, React.createElement("tbody", null, 
                                    _.map(_.range(1, 5), function(i) {
                                        return (
                                            React.createElement(PlayerEntry, {disabled: disabled, 
                                                         player: i, 
                                                         activePlayer: _this.state.lobbyPlayerNum})
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
                                            React.createElement(Button, {disabled: disabled, onClick: this.onClickStart}, "Start Game")
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