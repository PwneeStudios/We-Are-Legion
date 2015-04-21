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

    var MenuSlider = React.createClass({displayName: "MenuSlider",
        render: function() {
            return (
                React.createElement("tr", {style: {'background-color':'#1c1e22','pointer-events':'auto'}}, 
                    React.createElement("td", null, this.props.children), 
                    React.createElement("td", null, 
                        React.createElement("input", {style: {'float':'right','width':'100%'}, 
                            type: "range", 
                            value: this.props.value, 
                            min: 0, 
                            max: 1, 
                            onInput: null, 
                            step: 0.05})
                    )
                )
            );
        }
    });

    var MenuDropdown = React.createClass({displayName: "MenuDropdown",
        render: function() {
            return (
                React.createElement("tr", {style: {'background-color':'#1c1e22'}}, 
                    React.createElement("td", null, this.props.children), 
                    React.createElement("td", {className: "menu-cell-dropdown"}, 
                        React.createElement(Dropdown, {value: this.props.value, choices: this.props.choices, style: {'float':'right'}})
                    )
                )
            );
        }
    });

    var MenuButton = React.createClass({displayName: "MenuButton",
        render: function() {
            return (
                React.createElement("tr", {style: {'background-color':'#1c1e22','pointer-events':'auto'}}, 
                    React.createElement("td", null), 
                    React.createElement("td", null, 
                        React.createElement(Button, {style: {'float':'right','width':'100%'}}, 
                            this.props.children
                        )
                    )
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
            var resolutionChoices = [
                {name: '1920x1080', value:1},
            ];

            var fullscreenChoices = [
                {name: 'Fullscreen', value:1},
                {name: 'Windowed', value:2},
            ];

            return (
                React.createElement("div", {className: "menu"}, 
                    React.createElement(Div, {nonBlocking: true, pos: pos(10,5), size: width(30), style: {'font-size':'1%','pointer-events':'auto'}}, 
                        React.createElement(Table, {style: {width:'100%'}}, React.createElement("tbody", null, 
                            React.createElement(MenuSlider, null, "Sound"), 
                            React.createElement(MenuSlider, null, "Music"), 
                            React.createElement(MenuDropdown, {value: '1920x1080', choices: resolutionChoices}, "Resolution"), 
                            React.createElement(MenuDropdown, {value: 'Fullscreen', choices: fullscreenChoices}, "Fullscreen setting"), 
                            React.createElement(MenuButton, null, "Back")
                        ))
                    )
                )
            );
        }
    });
}); 