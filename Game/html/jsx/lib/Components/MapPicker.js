define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Glyphicon = ReactBootstrap.Glyphicon;
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
    var Input = ui.Input;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var MapEntry = React.createClass({
        render: function() {
            return (
                <ListGroupItem href='#' onClick={this.props.onPick} onDoubleClick={this.props.onConfirm}>
                    {this.props.directory ?
                        <span><Glyphicon glyph='folder-open' />&nbsp;&nbsp;</span>
                        : null}
                    {this.props.name}
                </ListGroupItem>
            );
        },
    });

    return React.createClass({
        getInitialState: function() {
            if (this.props.getMaps) {
                this.props.maps = this.props.getMaps(this.props.directory);
            }

            return {
                up: [],
                maps: this.props.maps,
                path: [],
                file: '',
            };
        },

        componentDidMount: function() {
            this.enabled = false;
            //interop.disableGameInput();
            interop.toggleChat(true);
        },

        componentWillUpdate: function() {
            if (this.enabled) {
                this.enabled = false;
                //interop.disableGameInput();
                interop.toggleChat(true);
            }
        },
        
        componentWillUnmount: function() {
            this.enabled = true;
            //interop.enableGameInput();
            interop.toggleChat(false);
        },

        onPick: function(map) {
            this.setState({
                file: map,
            });

            if (this.props.onPick) {
                this.props.onPick(map);
            }
        },

        back: function() {
            var prev = this.state.up.pop();
            this.state.path.pop();

            this.setState({
                up: this.state.up,
                maps: prev,
                path: this.state.path,
                file: '',
            });
        },

        path: function() {
            var title = '';

            if (this.props.directory) {
                title = this.props.directory + '/';
            }

            for (var i = 0; i < this.state.path.length; i++) {
                title += this.state.path[i] + '/';
            }

            return title;
        },

        onConfirm: function() {
            var file = this.state.file;
            if (this.refs.input) {
                file = this.refs.input.state.value;
            }
            
            if (file === null || file.length === 0) {
                return;
            }
            
            var map = this.path() + file;

            this.props.onRequestHide();
            this.props.onConfirm(map);
        },

        render: function() {
            var _this = this;

            var mapEntrees = _.map(this.state.maps, function(map) {
                if (_.isObject(map)) {
                    var directory = map;

                    var onPick = function() {
                        _this.state.up.push(_this.state.maps);
                        _this.state.path.push(directory.name);

                        _this.setState({
                            up: _this.state.up,
                            maps: directory.list,
                            path: _this.state.path,
                        })
                    };

                    return (
                        <MapEntry directory name={directory.name} onPick={onPick} />
                    );
                } else {
                    var onPick = function() {
                        _this.onPick(map);
                    };

                    return (
                        <MapEntry name={map} onPick={onPick} onConfirm={_this.onConfirm} />
                    );
                }
            });

            var title = "\u00a0";
            if (this.props.showPath) {
                title += this.path();

                title = (
                    <h4 style={{'margin-top':'3px', 'margin-bottom':'0px', 'color':'white'}}>
                        {title}
                    </h4>
                );
            }

            return (
                <Modal {...this.props} bsStyle='primary' title={title} animation={false} >
                    <div className='modal-body' style={{'height':0.5*window.h + 'px'}}>
                        <div className='chat-background' style={{'width':'100%','overflow-y':'scroll','height':'100%','pointer-events':'auto'}}>
                            <ListGroup style={{'pointer-events':'auto','font-size': '1.4%'}}>
                                {mapEntrees}
                                
                                {this.state.up && this.state.up.length > 0 ? 
                                    <ListGroupItem href='#' onClick={this.back}>
                                        <Glyphicon glyph='arrow-left' />
                                    </ListGroupItem>
                                    : null}
                            </ListGroup>
                        </div>
                    </div>

                    <div className='modal-footer' style={{'font-size':'1.4%'}}>
                        {this.props.saveAs ?
                            <div style={{'width':'56%','display':'inline-block','float':'left'}}>
                                <Input value={this.state.file} ref='input' prefix='Name' />
                            </div>
                            : null}

                        {this.props.confirm ? 
                            <Button onClick={this.onConfirm}>{this.props.confirm}</Button>
                            : null}
                        <Button onClick={this.props.onRequestHide}>Close</Button>
                    </div>
                </Modal>
            );
        }
    });
}); 