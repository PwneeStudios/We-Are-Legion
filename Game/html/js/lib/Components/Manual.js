'use strict';

define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui'], function (_, React, ReactBootstrap, interop, events, ui) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Nav = ReactBootstrap.Nav;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    var CarouselItem = ReactBootstrap.CarouselItem;
    var Carousel = ReactBootstrap.Carousel;

    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var RenderAtMixin = ui.RenderAtMixin;
    var MenuItem = ui.MenuItem;
    var Menu = ui.Menu;

    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    return React.createClass({
        mixins: [events.AllowBackMixin],

        getInitialState: function getInitialState() {
            return {
                index: 0,
                direction: null
            };
        },

        handleSelect: function handleSelect(selectedIndex, selectedDirection) {
            this.setState({
                index: selectedIndex,
                direction: selectedDirection
            });
        },

        render: function render() {
            return React.createElement(
                Div,
                { pos: pos(0, 0), size: size(100, 100), style: { 'pointer-events': 'auto', 'background-color': 'black' } },
                React.createElement(
                    Carousel,
                    { activeIndex: this.state.index, direction: this.state.direction, onSelect: this.handleSelect },
                    React.createElement(
                        CarouselItem,
                        { style: { 'pointer-events': 'auto', 'font-size': '1.4%;' } },
                        React.createElement(UiImage, { width: 100, image: { width: 1920, height: 1080, url: 'css/Screen-Instructions.png' } })
                    ),
                    React.createElement(
                        CarouselItem,
                        { style: { 'pointer-events': 'auto', 'font-size': '1.4%;' } },
                        React.createElement(UiImage, { width: 100, image: { width: 1920, height: 1080, url: 'css/Screen-Getting-Started.png' } })
                    ),
                    React.createElement(
                        CarouselItem,
                        { style: { 'pointer-events': 'auto', 'font-size': '1.4%;' } },
                        React.createElement(UiImage, { width: 100, image: { width: 1920, height: 1080, url: 'css/Screen-Dragonlords.png' } })
                    ),
                    React.createElement(
                        CarouselItem,
                        { style: { 'pointer-events': 'auto', 'font-size': '1.4%;' } },
                        React.createElement(UiImage, { width: 100, image: { width: 1920, height: 1080, url: 'css/Screen-Spells.png' } })
                    ),
                    React.createElement(
                        CarouselItem,
                        { style: { 'pointer-events': 'auto', 'font-size': '1.4%;' } },
                        React.createElement(UiImage, { width: 100, image: { width: 1920, height: 1080, url: 'css/Screen-Credits.png' } })
                    )
                ),
                React.createElement(
                    Div,
                    { className: 'top', nonBlocking: true, pos: pos(90, 90), size: width(60) },
                    React.createElement(
                        'div',
                        { style: { 'pointer-events': 'auto' } },
                        React.createElement(
                            'p',
                            null,
                            React.createElement(ui.BackButton, null)
                        )
                    )
                )
            );
        }
    });
});