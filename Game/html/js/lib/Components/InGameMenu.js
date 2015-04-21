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
    var RenderAtMixin = ui.RenderAtMixin;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var MenuItem = React.createClass({displayName: "MenuItem",
        render: function() {
            return (
                React.createElement(NavItem, React.__spread({},  this.props), 
                    React.createElement("h3", null, this.props.children)
                )
            );
        }
    });

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(Div, {nonBlocking: true, pos: pos(10,5), size: width(20), style: {'font-size':'1%','pointer-events':'auto'}}, 
                        React.createElement(Well, {style: {'height':'75%'}}, 
                            React.createElement(Nav, {bsStyle: "pills", stacked: true, style: {'pointer-events':'auto'}}, 
                                React.createElement(MenuItem, {eventKey: 1}, "Return to game"), 
                                React.createElement(MenuItem, {eventKey: 2}, "Pause game"), 
                                React.createElement(MenuItem, {eventKey: 3}, "Options"), 
                                React.createElement(MenuItem, {eventKey: 5}, "Quit Game")
                            )
                        )
                    )
                )
            );
        }
    });
}); 