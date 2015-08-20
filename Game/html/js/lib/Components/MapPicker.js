define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Glyphicon = ReactBootstrap.Glyphicon;
    var Input = ReactBootstrap.Input;
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

    var MapEntry = React.createClass({displayName: "MapEntry",
        render: function() {
            return (
                React.createElement(ListGroupItem, {href: "#", onClick: this.props.onPick}, 
                    this.props.directory ?
                        React.createElement("span", null, React.createElement(Glyphicon, {glyph: "folder-open"}), "  ")
                        : null, 
                    this.props.name
                )
            );
        },
    });

    return React.createClass({
        getInitialState: function() {
            return {
                up: [],
                maps: this.props.maps,
                path: [],
                file: '',
            };
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
            for (var i = 0; i < this.state.path.length; i++) {
                title += '/' + this.state.path[i];
            }

            return title;
        },

        onConfirm: function() {
            var map = this.path() + '/' + this.state.file;
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
                        React.createElement(MapEntry, {directory: true, name: directory.name, onPick: onPick})
                    );
                } else {
                    var onPick = function() {
                        _this.onPick(map);
                    };

                    return (
                        React.createElement(MapEntry, {name: map, onPick: onPick})
                    );
                }
            });

            var title = "\u00a0";
            if (this.props.showPath) {
                title += this.path();

                title = (
                    React.createElement("h4", {style: {'margin-top':'3px', 'margin-bottom':'0px', 'color':'white'}}, 
                        title
                    )
                );
            }

            var inputStyle = {
                'pointer-events':'auto',
                'background-color':'lightgray',
            };

            return (
                React.createElement(Modal, React.__spread({},  this.props, {bsStyle: "primary", title: title, animation: false}), 
                    React.createElement("div", {className: "modal-body", style: {'height':0.5*window.h + 'px'}}, 
                        React.createElement("div", {className: "chat-background", style: {'width':'100%','overflow-y':'scroll','height':'100%','pointer-events':'auto'}}, 
                            React.createElement(ListGroup, {style: {'pointer-events':'auto','font-size': '1.4%'}}, 
                                mapEntrees, 
                                
                                this.state.up && this.state.up.length > 0 ? 
                                    React.createElement(ListGroupItem, {href: "#", onClick: this.back}, 
                                        React.createElement(Glyphicon, {glyph: "arrow-left"})
                                    )
                                    : null
                            )
                        )
                    ), 

                    React.createElement("div", {className: "modal-footer", style: {'font-size':'1.4%'}}, 
                        this.props.saveAs ?
                            React.createElement("div", {style: {'width':'56%','display':'inline-block','float':'left'}}, 
                                React.createElement(Input, {value: this.state.file, ref: "input", type: "text", 
                                    addonBefore: "Name", 
                                    style: inputStyle}
                                  )
                            )
                            : null, 

                        this.props.confirm ? 
                            React.createElement(Button, {onClick: this.onConfirm}, this.props.confirm)
                            : null, 
                        React.createElement(Button, {onClick: this.props.onRequestHide}, "Close")
                    )
                )
            );
        }
    });
}); 