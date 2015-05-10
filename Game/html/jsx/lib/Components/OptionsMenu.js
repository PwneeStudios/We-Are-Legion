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

    var MenuItem = React.createClass({
        render: function() {
            return (
                <NavItem {...this.props}>
                    <h3>{this.props.children}</h3>
                </NavItem>
            );
        }
    });

    var MenuSlider = React.createClass({
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
                <tr style={{'background-color':'#1c1e22','pointer-events':'auto'}}>
                    <td>{this.props.children}</td>
                    <td>
                        <input style={{'float':'right','width':'100%'}}
                            ref='slider'
                            type='range'
                            value={this.state.value}
                            min={0}
                            max={1}
                            onChange={this.onChange}
                            step={0.05} />
                    </td>
                </tr>
            );
        }
    });

    var MenuDropdown = React.createClass({
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
                <tr style={{'background-color':'#1c1e22'}}>
                    <td>{this.props.children}</td>
                    <td className='menu-cell-dropdown'>
                        <Dropdown selected={value} choices={choices} onSelect={this.onSelect} style={{'float':'right'}} />
                    </td>
                </tr>
            );
        }
    });

    var MenuButton = React.createClass({
        render: function() {
            return (
                <tr style={{'background-color':'#1c1e22','pointer-events':'auto'}}>
                    <td></td>
                    <td>
                        <Button onClick={this.props.onClick} style={{'float':'right','width':'100%'}}>
                            {this.props.children}
                        </Button>
                    </td>
                </tr>
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
            ];

            var fullscreenChoices = [
                {name: 'Fullscreen', value:true},
                {name: 'Windowed', value:false},
            ];

            return (
                <Menu width={30} type='table'>
                    <MenuSlider variable='SoundVolume'>Sound</MenuSlider>
                    <MenuSlider variable='MusicVolume'>Music</MenuSlider>
                    <MenuDropdown variable='Resolution' choices={resolutionChoices}>Resolution</MenuDropdown>
                    <MenuDropdown variable='Fullscreen' choices={fullscreenChoices}>Fullscreen setting</MenuDropdown>
                    <MenuButton onClick={back}>Back</MenuButton>
                </Menu>
            );
        }
    });
}); 