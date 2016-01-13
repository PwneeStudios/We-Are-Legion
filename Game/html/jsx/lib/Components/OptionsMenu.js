define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, sound, React, ReactBootstrap, interop, events, ui, Chat) {
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
                window.invoke('Set' + this.props.variable, value);
            }
        },

        getInitialState: function() {
            return {value:0};
        },
        
        componentDidMount: function() {
            var _this = this;
            window['get' + this.props.variable] = value => _this.setState({value:value});

            if (interop.InXna()) {
                window.invoke('Get' + this.props.variable);
            }
        },
        
        componentWillUnmount: function() {
            window['get' + this.props.variable] = () => {};
        },

        render: function() {
            return (
                <tr style={{'background-color':'#1c1e22','pointer-events':'auto'}}>
                    <td className='menu-description'>{this.props.children}</td>
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
                window.invoke('Set' + this.props.variable, item.value);
            }
        },

        getInitialState: function() {
            return { };
        },
        
        componentDidMount: function() {
            var _this = this;
            window['get' + this.props.variable + 'Values'] = choices => {
                window['get' + this.props.variable] = value => {
                    _this.setState({
                        choices:choices,
                        value:value,
                    });
                };
            }
            
            if (interop.InXna()) {
                window.invoke('Get' + this.props.variable + 'Values');
                window.invoke('Get' + this.props.variable);
            }
        },
        
        componentWillUnmount: function() {
            window['get' + this.props.variable] = () => {};
            window['get' + this.props.variable + 'Values'] = () => {};
        },
        
        render: function() {
            var choices, value;

            choices = this.state.choices || this.props.choices;
            
            if (_.isUndefined(this.state.value)) {
                value = ' ';
            } else {
                value = this.state.choices[0];

                for (var i = 0; i < this.state.choices.length; i++) {
                    if (this.state.choices[i].value == this.state.value) {
                        value = this.state.choices[i];
                    }
                }
            }

            return (
                <tr style={{'background-color':'#1c1e22'}}>
                    <td className='menu-description'>{this.props.children}</td>
                    <td className='menu-cell-dropdown'>
                        <Dropdown
                            disabled={this.props.disabled}
                            scroll={this.props.scroll}
                            selected={value}
                            choices={choices}
                            onSelect={this.onSelect}
                            style={{'float':'right'}} />
                    </td>
                </tr>
            );
        }
    });

    var MenuButton = React.createClass({
        onClick: function() {
            sound.play.click();

            if (this.props.onClick) {
                this.props.onClick();
            }
        },

        render: function() {
            return (
                <tr style={{'background-color':'#1c1e22','pointer-events':'auto'}}>
                    <td></td>
                    <td>
                        <Button onClick={this.onClick} onMouseEnter={sound.play.hover} style={{'float':'right','width':'100%'}}>
                            {this.props.children}
                        </Button>
                    </td>
                </tr>
            );
        }
    });

    return React.createClass({
        mixins: [events.AllowBackMixin],
                
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

            var disableResolutions = false;
            /*if (interop.InXna()) {
                var value = interop.getFullscreen();
                if (value) {
                    disableResolutions = true;
                }
            }fixme*/

            var screenOptions = [
                <MenuDropdown disabled={disableResolutions} scroll variable='Resolution' choices={resolutionChoices}>Resolution</MenuDropdown>,
                <MenuDropdown variable='Fullscreen' choices={fullscreenChoices}>Fullscreen setting</MenuDropdown>,
            ];

            return (
                <Menu width={30} type='table'>
                    <MenuSlider variable='SoundVolume'>Sound</MenuSlider>
                    <MenuSlider variable='MusicVolume'>Music</MenuSlider>

                    {this.props.params.inGame ? null : screenOptions}

                    <MenuButton onClick={back}>Back</MenuButton>
                </Menu>
            );
        }
    });
}); 