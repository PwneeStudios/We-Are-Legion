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

    var MenuItem = React.createClass({
        render: function() {
            return (
                <NavItem {...this.props}>
                    <h3>{this.props.children}</h3>
                </NavItem>
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
                <div>
                    <Div nonBlocking pos={pos(10,5)} size={width(20)} style={{'font-size':'1%','pointer-events':'auto'}}>
                        <Well style={{'height':'75%'}}>
                            <Nav bsStyle='pills' stacked style={{'pointer-events':'auto'}}>
                                <MenuItem eventKey={1}>Return to game</MenuItem>
                                <MenuItem eventKey={2}>Pause game</MenuItem>
                                <MenuItem eventKey={3}>Options</MenuItem>
                                <MenuItem eventKey={5}>Quit Game</MenuItem>
                            </Nav>
                        </Well>
                    </Div>
                </div>
            );
        }
    });
}); 