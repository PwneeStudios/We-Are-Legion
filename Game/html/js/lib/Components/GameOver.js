'use strict';

define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'sound', 'Components/Chat', 'Components/MapPicker'], function (_, React, ReactBootstrap, interop, events, ui, sound, Chat, MapPicker) {

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

    var UnitBar = React.createClass({
        displayName: 'UnitBar',

        mixins: [RenderAtMixin],

        item: function item(image, scale, data) {
            return React.createElement(
                'td',
                null,
                React.createElement(UiImage, { width: 24.2 * scale, image: image }),
                React.createElement(
                    'p',
                    { style: { paddingLeft: '5%', 'pointer-events': 'none' } },
                    ' ',
                    data
                )
            );
        },

        renderAt: function renderAt() {
            var Images = playerImages[this.props.MyPlayerNumber];
            var Buildings = Images.Buildings;
            var Units = Images.Units;

            var name = this.props.info.Name || 'Player ' + this.props.MyPlayerNumber;

            return React.createElement(
                'tr',
                null,
                React.createElement(
                    'td',
                    null,
                    React.createElement(
                        'p',
                        { style: { 'font-family': 'Verdana, Geneva, sans-serif' } },
                        name
                    )
                ),
                this.item(Buildings.Barracks, 1.2, this.props.info.Barracks.Count),
                this.item(Units.Soldier, 0.85, this.props.info.Units),
                this.item(Buildings.GoldMine, 2, this.props.info.GoldMine.Count),
                this.item(GoldImage, 0.5, this.props.info.Gold),
                this.item(Buildings.JadeMine, 2, this.props.info.JadeMine.Count),
                this.item(JadeImage, 0.5, this.props.info.Jade)
            );
        }
    });

    return React.createClass({
        mixins: [],

        show: function show() {
            sound.play.slam();

            this.setState({
                show: true
            });
        },

        getInitialState: function getInitialState() {
            if (interop.InXna()) {
                setTimeout(this.show, 4700);

                return {
                    show: false
                };
            } else {
                this.props.params = { "victory": true, "winningTeam": 2, "info": [null, { "Name": "Player 1", "Number": 1, "GoldMine": { "Count": 0, "Bought": 0 }, "JadeMine": { "Count": 0, "Bought": 0 }, "Barracks": { "Count": 100, "Bought": 0 }, "Gold": 7500, "Jade": 10000, "Units": 100, "DragonLords": 1, "SpellCasts": { "Fireball": 0, "Skeletons": 0, "Necromancer": 0, "Terracotta": 0 }, "SpellCosts": { "Fireball": 1000, "Skeletons": 1000, "Necromancer": 1000, "Terracotta": 1000 }, "DragonLordAlive": true, "Params": { "Buildings": { "Barracks": { "GoldCost": 250, "CostIncrease": 50, "GoldPerTick": 0, "JadePerTick": 0, "CurrentGoldCost": 250, "UnitType": 0.0235294122, "Name": "Barracks" }, "GoldMine": { "GoldCost": 500, "CostIncrease": 100, "GoldPerTick": 3, "JadePerTick": 0, "CurrentGoldCost": 500, "UnitType": 0.02745098, "Name": "GoldMine" }, "JadeMine": { "GoldCost": 1000, "CostIncrease": 200, "GoldPerTick": 0, "JadePerTick": 3, "CurrentGoldCost": 1000, "UnitType": 0.03137255, "Name": "JadeMine" } }, "Barracks": { "GoldCost": 250, "CostIncrease": 50, "GoldPerTick": 0, "JadePerTick": 0, "CurrentGoldCost": 250, "UnitType": 0.0235294122, "Name": "Barracks" }, "GoldMine": { "GoldCost": 500, "CostIncrease": 100, "GoldPerTick": 3, "JadePerTick": 0, "CurrentGoldCost": 500, "UnitType": 0.02745098, "Name": "GoldMine" }, "JadeMine": { "GoldCost": 1000, "CostIncrease": 200, "GoldPerTick": 0, "JadePerTick": 3, "CurrentGoldCost": 1000, "UnitType": 0.03137255, "Name": "JadeMine" }, "StartGold": 750, "StartJade": 10000 } }, { "Name": "Player 2", "Number": 2, "GoldMine": { "Count": 0, "Bought": 0 }, "JadeMine": { "Count": 0, "Bought": 0 }, "Barracks": { "Count": 187, "Bought": 0 }, "Gold": 7500, "Jade": 10000, "Units": 187, "DragonLords": 2, "SpellCasts": { "Fireball": 0, "Skeletons": 0, "Necromancer": 0, "Terracotta": 0 }, "SpellCosts": { "Fireball": 0, "Skeletons": 0, "Necromancer": 0, "Terracotta": 0 }, "DragonLordAlive": true, "Params": { "Buildings": { "Barracks": { "GoldCost": 250, "CostIncrease": 50, "GoldPerTick": 0, "JadePerTick": 0, "CurrentGoldCost": 250, "UnitType": 0.0235294122, "Name": "Barracks" }, "GoldMine": { "GoldCost": 500, "CostIncrease": 100, "GoldPerTick": 3, "JadePerTick": 0, "CurrentGoldCost": 500, "UnitType": 0.02745098, "Name": "GoldMine" }, "JadeMine": { "GoldCost": 1000, "CostIncrease": 200, "GoldPerTick": 0, "JadePerTick": 3, "CurrentGoldCost": 1000, "UnitType": 0.03137255, "Name": "JadeMine" } }, "Barracks": { "GoldCost": 250, "CostIncrease": 50, "GoldPerTick": 0, "JadePerTick": 0, "CurrentGoldCost": 250, "UnitType": 0.0235294122, "Name": "Barracks" }, "GoldMine": { "GoldCost": 500, "CostIncrease": 100, "GoldPerTick": 3, "JadePerTick": 0, "CurrentGoldCost": 500, "UnitType": 0.02745098, "Name": "GoldMine" }, "JadeMine": { "GoldCost": 1000, "CostIncrease": 200, "GoldPerTick": 0, "JadePerTick": 3, "CurrentGoldCost": 1000, "UnitType": 0.03137255, "Name": "JadeMine" }, "StartGold": 750, "StartJade": 10000 } }, { "Name": "Player 3", "Number": 3, "GoldMine": { "Count": 0, "Bought": 0 }, "JadeMine": { "Count": 0, "Bought": 0 }, "Barracks": { "Count": 0, "Bought": 0 }, "Gold": 7500, "Jade": 10000, "Units": 0, "DragonLords": 0, "SpellCasts": { "Fireball": 0, "Skeletons": 0, "Necromancer": 0, "Terracotta": 0 }, "SpellCosts": { "Fireball": 0, "Skeletons": 0, "Necromancer": 0, "Terracotta": 0 }, "DragonLordAlive": false, "Params": { "Buildings": { "Barracks": { "GoldCost": 250, "CostIncrease": 50, "GoldPerTick": 0, "JadePerTick": 0, "CurrentGoldCost": 250, "UnitType": 0.0235294122, "Name": "Barracks" }, "GoldMine": { "GoldCost": 500, "CostIncrease": 100, "GoldPerTick": 3, "JadePerTick": 0, "CurrentGoldCost": 500, "UnitType": 0.02745098, "Name": "GoldMine" }, "JadeMine": { "GoldCost": 1000, "CostIncrease": 200, "GoldPerTick": 0, "JadePerTick": 3, "CurrentGoldCost": 1000, "UnitType": 0.03137255, "Name": "JadeMine" } }, "Barracks": { "GoldCost": 250, "CostIncrease": 50, "GoldPerTick": 0, "JadePerTick": 0, "CurrentGoldCost": 250, "UnitType": 0.0235294122, "Name": "Barracks" }, "GoldMine": { "GoldCost": 500, "CostIncrease": 100, "GoldPerTick": 3, "JadePerTick": 0, "CurrentGoldCost": 500, "UnitType": 0.02745098, "Name": "GoldMine" }, "JadeMine": { "GoldCost": 1000, "CostIncrease": 200, "GoldPerTick": 0, "JadePerTick": 3, "CurrentGoldCost": 1000, "UnitType": 0.03137255, "Name": "JadeMine" }, "StartGold": 750, "StartJade": 10000 } }, { "Name": "Player 4", "Number": 4, "GoldMine": { "Count": 0, "Bought": 0 }, "JadeMine": { "Count": 0, "Bought": 0 }, "Barracks": { "Count": 0, "Bought": 0 }, "Gold": 7500, "Jade": 10000, "Units": 0, "DragonLords": 0, "SpellCasts": { "Fireball": 0, "Skeletons": 0, "Necromancer": 0, "Terracotta": 0 }, "SpellCosts": { "Fireball": 0, "Skeletons": 0, "Necromancer": 0, "Terracotta": 0 }, "DragonLordAlive": false, "Params": { "Buildings": { "Barracks": { "GoldCost": 250, "CostIncrease": 50, "GoldPerTick": 0, "JadePerTick": 0, "CurrentGoldCost": 250, "UnitType": 0.0235294122, "Name": "Barracks" }, "GoldMine": { "GoldCost": 500, "CostIncrease": 100, "GoldPerTick": 3, "JadePerTick": 0, "CurrentGoldCost": 500, "UnitType": 0.02745098, "Name": "GoldMine" }, "JadeMine": { "GoldCost": 1000, "CostIncrease": 200, "GoldPerTick": 0, "JadePerTick": 3, "CurrentGoldCost": 1000, "UnitType": 0.03137255, "Name": "JadeMine" } }, "Barracks": { "GoldCost": 250, "CostIncrease": 50, "GoldPerTick": 0, "JadePerTick": 0, "CurrentGoldCost": 250, "UnitType": 0.0235294122, "Name": "Barracks" }, "GoldMine": { "GoldCost": 500, "CostIncrease": 100, "GoldPerTick": 3, "JadePerTick": 0, "CurrentGoldCost": 500, "UnitType": 0.02745098, "Name": "GoldMine" }, "JadeMine": { "GoldCost": 1000, "CostIncrease": 200, "GoldPerTick": 0, "JadePerTick": 3, "CurrentGoldCost": 1000, "UnitType": 0.03137255, "Name": "JadeMine" }, "StartGold": 750, "StartJade": 10000 } }] };

                return {
                    show: true
                };
            }
        },

        render: function render() {
            var _this = this;

            if (!this.state.show) {
                return React.createElement('div', null);
            }

            if (this.props.params.spectator) {
                var message = 'Match Over';
            } else {
                var message = this.props.params.victory ? 'Victory!' : 'Defeat!';
            }

            var players = _.range(1, 5);

            return React.createElement(
                'div',
                null,
                React.createElement(
                    Div,
                    { nonBlocking: true, pos: pos(10, 10), size: width(80) },
                    React.createElement(
                        Well,
                        { style: { 'height': '80%' } },
                        React.createElement(
                            Div,
                            { pos: pos(5, 2), size: width(90) },
                            React.createElement(
                                'h1',
                                { style: { float: 'left', fontSize: 52 } },
                                message
                            ),
                            React.createElement(
                                'h1',
                                { style: { float: 'right', fontSize: 30 } },
                                React.createElement(
                                    'span',
                                    { style: { float: 'right' } },
                                    'Team ',
                                    this.props.params.winningTeam,
                                    ' wins!'
                                )
                            )
                        ),
                        React.createElement(
                            Div,
                            { pos: pos(5, 20) },
                            React.createElement(
                                Table,
                                { style: { width: '90%' } },
                                React.createElement(
                                    'tbody',
                                    null,
                                    _.map(players, function (player, index) {
                                        return React.createElement(UnitBar, { MyPlayerNumber: player, info: _this.props.params.info[player] });
                                    })
                                )
                            )
                        ),
                        React.createElement(
                            Div,
                            { nonBlocking: true, pos: pos(36, 72), size: width(60) },
                            React.createElement(
                                'div',
                                { style: { 'float': 'right', 'pointer-events': 'auto' } },
                                React.createElement(
                                    'p',
                                    null,
                                    React.createElement(
                                        Button,
                                        { onClick: interop.returnToLobby },
                                        'Return to Lobby'
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