'use strict';

define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function (_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Nav = ReactBootstrap.Nav;
    var NavItem = ReactBootstrap.NavItem;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;

    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var Menu = ui.Menu;
    var MenuItem = ui.MenuItem;
    var MenuTitle = ui.MenuTitle;
    var RenderAtMixin = ui.RenderAtMixin;

    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    return React.createClass({
        mixins: [],

        getInitialState: function getInitialState() {
            return {};
        },

        unpause: function unpause(e) {
            interop.requestUnpause();
        },

        render: function render() {
            return React.createElement(
                Menu,
                null,
                React.createElement(
                    MenuTitle,
                    null,
                    'Game paused'
                ),
                React.createElement(
                    MenuItem,
                    { disabled: !this.props.params.canUnpause, eventKey: 1, onClick: this.unpause },
                    'Unpause'
                )
            );
        }
    });
});