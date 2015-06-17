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
        onChange: function(e) {
            var value = this.refs.slider.getDOMNode().value;
            this.setState({value:value});

            if (interop.InXna()) {
                xna['Set' + this.props.variable](value);
            }
        },

        getInitialState: function() {
            var value = 0.0;
            if (interop.InXna()) {
                value = xna['Get' + this.props.variable]();
            }

            return {value:value};
        },

        render: function() {
            return (
                React.createElement("tr", {style: {'background-color':'#1c1e22','pointer-events':'auto'}}, 
                    React.createElement("td", null, this.props.children), 
                    React.createElement("td", null, 
                        React.createElement("input", {style: {'float':'right','width':'100%'}, 
                            ref: "slider", 
                            type: "range", 
                            value: this.state.value, 
                            min: 0, 
                            max: 1, 
                            onChange: this.onChange, 
                            step: 0.05})
                    )
                )
            );
        }
    });

    var MenuDropdown = React.createClass({displayName: "MenuDropdown",
        onSelect: function(item) {
            if (interop.InXna()) {
                xna['Set' + this.props.variable](item.value);
            }
        },

        render: function() {
            var choices, value;

            if (interop.InXna()) {
                value = xna['Get' + this.props.variable]();                
                choices = interop.get('Get' + this.props.variable + 'Values');
                choices = choices || this.props.choices;

                var item = xna['Get' + this.props.variable]();
                value = _.find(choices, function(o) {return o.value === value;});

                if (!value) {
                    value = choices[0];
                }
            } else {
                choices = this.props.choices;
                value = choices[0];
            }

            return (
                React.createElement("tr", {style: {'background-color':'#1c1e22'}}, 
                    React.createElement("td", null, this.props.children), 
                    React.createElement("td", {className: "menu-cell-dropdown"}, 
                        React.createElement(Dropdown, {scroll: this.props.scroll, selected: value, choices: choices, onSelect: this.onSelect, style: {'float':'right'}})
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
                        React.createElement(Button, {onClick: this.props.onClick, style: {'float':'right','width':'100%'}}, 
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
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
                {name: '1920x1080', value:[1920,1080]},
            ];

            var fullscreenChoices = [
                {name: 'Fullscreen', value:true},
                {name: 'Windowed', value:false},
            ];

            var screenOptions = [
                React.createElement(MenuDropdown, {scroll: true, variable: "Resolution", choices: resolutionChoices}, "Resolution"),
                React.createElement(MenuDropdown, {variable: "Fullscreen", choices: fullscreenChoices}, "Fullscreen setting"),
            ];

            return (
                React.createElement(Menu, {width: 30, type: "table"}, 
                    React.createElement(MenuSlider, {variable: "SoundVolume"}, "Sound"), 
                    React.createElement(MenuSlider, {variable: "MusicVolume"}, "Music"), 

                    this.props.params.inGame ? null : screenOptions, 

                    React.createElement(MenuButton, {onClick: back}, "Back")
                )
            );
        }
    });
}); 