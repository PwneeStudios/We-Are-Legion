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
        render: function() {
            return (
                <tr style={{'background-color':'#1c1e22','pointer-events':'auto'}}>
                    <td>{this.props.children}</td>
                    <td>
                        <input style={{'float':'right','width':'100%'}}
                            type="range"
                            value={this.props.value}
                            min={0}
                            max={1}
                            onInput={null}
                            step={0.05} />
                    </td>
                </tr>
            );
        }
    });

    var MenuDropdown = React.createClass({
        render: function() {
            return (
                <tr style={{'background-color':'#1c1e22'}}>
                    <td>{this.props.children}</td>
                    <td className='menu-cell-dropdown'>
                        <Dropdown value={this.props.value} choices={this.props.choices} style={{'float':'right'}} />
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
                        <Button style={{'float':'right','width':'100%'}}>
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
                {name: '1920x1080', value:1},
            ];

            var fullscreenChoices = [
                {name: 'Fullscreen', value:1},
                {name: 'Windowed', value:2},
            ];

            return (
                <Menu width={30} type='table'>
                    <MenuSlider>Sound</MenuSlider>
                    <MenuSlider>Music</MenuSlider>
                    <MenuDropdown value={'1920x1080'} choices={resolutionChoices}>Resolution</MenuDropdown>
                    <MenuDropdown value={'Fullscreen'} choices={fullscreenChoices}>Fullscreen setting</MenuDropdown>
                    <MenuButton>Back</MenuButton>
                </Menu>
            );
        }
    });
}); 