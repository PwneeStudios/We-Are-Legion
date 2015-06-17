define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
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
    var RenderAtMixin = ui.RenderAtMixin;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },

        componentDidMount: function() {
            interop.disableGameInput();
        },
        
        componentWillUnmount: function() {
            interop.enableGameInput();
        },

        render: function() {
            return (
                React.createElement(Menu, null, 
                    React.createElement(MenuItem, {eventKey: 1, to: "back"}, "Return to game"), 
                    React.createElement(MenuItem, {eventKey: 2}, "Pause game"), 
                    React.createElement(MenuItem, {eventKey: 3, to: "options", params: {inGame:true}}, "Options"), 
                    React.createElement(MenuItem, {eventKey: 5, onClick: leaveGame}, "Leave Game")
                )
            );
        }
    });
}); 