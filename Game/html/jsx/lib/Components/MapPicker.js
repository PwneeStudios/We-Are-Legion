define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    var ListGroup = ReactBootstrap.ListGroup;
    var ListGroupItem = ReactBootstrap.ListGroupItem;
    var Modal = ReactBootstrap.Modal;
    
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

    var MapEntry = React.createClass({
        render: function() {
            return (
                <ListGroupItem href='#'>{this.props.name}</ListGroupItem>
            );
        },
    });

    return React.createClass({
        render: function() {
            var maps = ['Beset', 'Clash of Madness', "Isn't This Nice?", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello", "Hello"];
            var mapEntrees = _.map(maps, function(map) {
                return (
                    <MapEntry name={map} />
                );
            });

            return (
                <Modal {...this.props} bsStyle='primary' title='&nbsp;' animation={false} >
                    <div className='modal-body' style={{'height':0.5*window.h + 'px'}}>
                        <div className='chat-background' style={{'width':'100%','overflow-y':'scroll','height':'100%','pointer-events':'auto'}}>
                            <ListGroup style={{'pointer-events':'auto','font-size': '1.4%'}}>
                                {mapEntrees}
                            </ListGroup>
                        </div>
                    </div>

                    <div className='modal-footer' style={{'font-size':'1.4%'}}>
                        <Button onClick={this.props.onRequestHide}>Close</Button>
                    </div>
                </Modal>
            );
        }
    });
}); 