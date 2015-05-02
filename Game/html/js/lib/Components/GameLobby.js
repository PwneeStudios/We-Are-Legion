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
                value: this.props.default,
            };
        },
        
        render: function() {
            if (this.props.activePlayer == this.props.player) {
                return (
                    React.createElement(Dropdown, {value: this.state.value, choices: this.props.choices})
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
                    React.createElement("td", null, React.createElement(Choose, React.__spread({choices: teamChoices, default: "Choose team"},  this.props))), 
                    React.createElement("td", null, React.createElement(Choose, React.__spread({choices: kingdomChoices, default: "Choose kingdom"},  this.props)))
                )
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
                React.createElement("div", null, 
                    React.createElement(Div, {nonBlocking: true, pos: pos(10,5), size: width(80)}, 
                        React.createElement(Panel, null, 
                            React.createElement("h2", null, 
                                "Game Lobby"
                            )
                        ), 

                        React.createElement(Well, {style: {'height':'75%'}}, 

                            /* Chat */
                            React.createElement(Chat.ChatBox, {show: true, full: true, pos: pos(2, 17), size: size(43,61)}), 
                            React.createElement(Chat.ChatInput, {show: true, pos: pos(2,80), size: width(43)}), 

                            /* Player Table */
                            React.createElement(Div, {nonBlocking: true, pos: pos(48,16.9), size: width(50), style: {'pointer-events':'auto', 'font-size': '1.4%;'}}, 
                                React.createElement(Table, {style: {width:'100%'}}, React.createElement("tbody", null, 
                                    _.map(_.range(1, 5), function(i) {
                                        return React.createElement(PlayerEntry, {player: i, activePlayer: _this.props.lobbyPlayerNum});
                                    })
                                ))
                            ), 

                            /* Map */
                            React.createElement(Div, {nonBlocking: true, pos: pos(38,68), size: width(60)}, 
                                React.createElement("div", {style: {'float':'right', 'pointer-events':'auto'}}, 
                                    React.createElement("p", null, 
                                        this.props.host ? React.createElement(Button, null, "Choose map...") : null
                                    )
                                )
                            ), 

                            /* Buttons */
                            React.createElement(Div, {nonBlocking: true, pos: pos(38,80), size: width(60)}, 
                                React.createElement("div", {style: {'float':'right', 'pointer-events':'auto'}}, 
                                    React.createElement("p", null, 
                                        this.props.host ? React.createElement(Button, null, "Start Game") : null, 
                                        " ", 
                                        React.createElement(Button, {onClick: back}, "Leave Lobby")
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