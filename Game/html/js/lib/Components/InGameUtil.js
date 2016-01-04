'use strict';

define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function (_, sound, React, ReactBootstrap, interop, events, ui, Chat) {
    var Input = ReactBootstrap.Input;
    var Popover = ReactBootstrap.Popover;
    var Button = ReactBootstrap.Button;
    var Glyphicon = ReactBootstrap.Glyphicon;

    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var RenderAtMixin = ui.RenderAtMixin;

    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var setGlobalImages = function setGlobalImages() {
        window.SpellsSpritesheet = { width: 300, height: 300, dim: [1, 4], url: 'css/SpellIcons.png' };
        window.BuildingsSpritesheet = { width: 96, height: 96, dim: [5, 10], url: 'css/Buildings_1.png' };
        window.UnitSpritesheet = { width: 32, height: 32, dim: [32, 96], url: 'css/Soldier_1.png' };
        window.TileSpritesheet = { width: 32, height: 32, dim: [32, 32], url: 'css/TileSet_1.png' };

        window.Spells = {
            Fireball: subImage(SpellsSpritesheet, [0, 0]),
            Skeletons: subImage(SpellsSpritesheet, [0, 1]),
            Necromancer: subImage(SpellsSpritesheet, [0, 2]),
            Terracotta: subImage(SpellsSpritesheet, [0, 3])
        };

        window.GoldImage = { width: 20, height: 22, url: 'css/Gold.png' };
        window.JadeImage = { width: 20, height: 22, url: 'css/Jade.png' };
    };

    var getPlayerImages = function getPlayerImages(player) {
        var i = player - 1;

        return {
            Buildings: {
                Barracks: subImage(BuildingsSpritesheet, [i + 1, 1]),
                GoldMine: subImage(BuildingsSpritesheet, [i + 1, 3]),
                JadeMine: subImage(BuildingsSpritesheet, [i + 1, 5])
            },

            Units: {
                Soldier: subImage(UnitSpritesheet, [0, 4 * i]),
                DragonLord: subImage(UnitSpritesheet, [0, 16 * 1 + 4 * i]),
                Necromancer: subImage(UnitSpritesheet, [0, 16 * 2 + 4 * i]),
                Skeleton: subImage(UnitSpritesheet, [0, 16 * 3 + 4 * i]),
                Terracotta: subImage(UnitSpritesheet, [0, 16 * 4 + 4 * i])
            },

            Tiles: {
                Dirt: subImage(TileSpritesheet, [0, 30]),
                Grass: subImage(TileSpritesheet, [0, 31]),
                Trees: subImage(TileSpritesheet, [0, 23])
            }
        };
    };

    var makeTooltip = function makeTooltip(name, key, hotkey) {
        return React.createElement(
            'span',
            null,
            name,
            React.createElement(
                'span',
                { style: { 'float': 'right', 'font-size': '60%' } },
                '(Hot key ',
                hotkey,
                ')'
            )
        );
    };

    var setPlayerImages = function setPlayerImages() {
        window.playerImages = _.map(_.range(5), function (player) {
            return getPlayerImages(player);
        });
    };

    var UnitBar = React.createClass({
        displayName: 'UnitBar',

        mixins: [RenderAtMixin, events.UpdateMixin],

        onUpdate: function onUpdate(values) {
            this.setState({
                info: values.MyPlayerInfo
            });
        },

        getInitialState: function getInitialState() {
            return {
                info: null
            };
        },

        item: function item(p, image, scale, image_pos, data) {
            return React.createElement(
                Div,
                { nonBlocking: true, pos: p },
                React.createElement(UiImage, { nonBlocking: true, pos: image_pos, width: 4.2 * scale, image: image }),
                React.createElement(
                    'p',
                    { style: { paddingLeft: '5%', 'pointer-events': 'none' } },
                    data
                )
            );
        },

        renderAt: function renderAt() {
            if (this.props.MyPlayerNumber <= 0) {
                return React.createElement('div', null);
            }

            var x = 2;
            var small = 13.2,
                big = 17.2;

            var Images = playerImages[this.props.MyPlayerNumber];
            var Buildings = Images.Buildings;
            var Units = Images.Units;

            return React.createElement(
                'div',
                null,
                React.createElement(UiImage, { width: 100, image: { width: 869, height: 60, url: 'css/UnitBar.png' } }),
                React.createElement(
                    Div,
                    { nonBlocking: true, pos: pos(0, 0.92) },
                    this.item(pos(x, 0), Buildings.Barracks, 1, pos(0, 0), this.state.info ? this.state.info.Barracks.Count : 0),
                    this.item(pos(x += small, 0), Units.Soldier, 0.85, pos(0.4, 0), this.state.info ? this.state.info.Units : 0),
                    this.item(pos(x += big, 0), Buildings.GoldMine, 1, pos(0, 0), this.state.info ? this.state.info.GoldMine.Count : 0),
                    this.item(pos(x += small, 0), GoldImage, 0.67, pos(1.2, 0.5), this.state.info ? this.state.info.Gold : 0),
                    this.item(pos(x += big, 0), Buildings.JadeMine, 1, pos(0, 0), this.state.info ? this.state.info.JadeMine.Count : 0),
                    this.item(pos(x += small, 0), JadeImage, 0.67, pos(1.2, 0.5), this.state.info ? this.state.info.Jade : 0)
                )
            );
        }
    });

    var MenuButton = React.createClass({
        displayName: 'MenuButton',

        mixins: [RenderAtMixin],

        renderAt: function renderAt() {
            var pStyle = { fontSize: '90%', textAlign: 'right' };

            return React.createElement(
                'div',
                null,
                React.createElement(
                    Div,
                    { nonBlocking: true, pos: pos(0, 0, 'absolute'), style: { 'float': 'right', 'pointer-events': 'auto' } },
                    React.createElement(
                        Button,
                        { style: { position: 'absolute', 'pointer-events': 'auto' },
                            onClick: function onClick() {
                                window.setScreen('in-game-menu');sound.play.click();
                            } },
                        React.createElement(Glyphicon, { glyph: 'arrow-up' })
                    )
                )
            );
        }
    });

    var UnitBox = React.createClass({
        displayName: 'UnitBox',

        mixins: [RenderAtMixin, events.UpdateMixin],

        onUpdate: function onUpdate(values) {
            this.setState({
                value: values.UnitCount
            });
        },

        getInitialState: function getInitialState() {
            return {
                value: 0
            };
        },

        renderAt: function renderAt() {
            return React.createElement(
                'div',
                null,
                React.createElement(UiImage, { pos: pos(0, 0), width: 100, image: { width: 502, height: 157, url: 'css/UnitBox.png' } }),
                React.createElement(
                    Div,
                    { nonBlocking: true, pos: pos(-6, 5) },
                    React.createElement(
                        'p',
                        { style: { fontSize: '3.3%', textAlign: 'right' } },
                        this.state.value
                    )
                )
            );
        }
    });

    return {
        setGlobalImages: setGlobalImages,
        getPlayerImages: getPlayerImages,
        makeTooltip: makeTooltip,
        setPlayerImages: setPlayerImages,
        UnitBar: UnitBar,
        MenuButton: MenuButton,
        UnitBox: UnitBox
    };
});