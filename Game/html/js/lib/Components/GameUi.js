define(['lodash', 'react', 'react-bootstrap'], function(_, React, ReactBootstrap) {
    var Input = ReactBootstrap.Input;
    var OverlayTrigger = ReactBootstrap.OverlayTrigger;
    var Popover = ReactBootstrap.Popover;
    var Button = ReactBootstrap.Button;

    var pos = function(x, y, type) {
        if (typeof type === 'undefined') {
            type = 'absolute';
        }
    
        return {
            position:type,
            left: x + '%',
            top: y + '%',
        };
    };

    var size = function(x, y) {
        return {
            width: x + '%',
            height: y + '%',
        };
    };

    var width = function(x) {
        return size(x, 100);
    };

    var Div = React.createClass({displayName: "Div",
        render: function() {
            var style = {
                width:'100%',
                height:'100%',
            };
            
            style = _.assign(style, this.props.pos, this.props.size);
        
            return (
                React.createElement("div", {style: style}, 
                    this.props.children
                )
            );
        }
    });

    var RenderAtMixin = {
        render: function() {
            if (this.props.pos) {
                return (
                    React.createElement(Div, React.__spread({},  this.props), 
                        this.renderAt()
                    )
                );
            } else {
                return this.renderAt();
            }
        }
    };

    var UiButton = React.createClass({displayName: "UiButton",
        mixins: [RenderAtMixin],
        
        renderAt: function() {
            var image = this.props.image;
            image.aspect = image.height / image.width;
            
            var width = this.props.width;
            var height = width * image.aspect;
            
            var button = React.createElement("button", {className: "UiButton", style: {backgroundImage: 'url('+image.url+')'}});
            
            var body;
            if (this.props.overlay) {
                body = 
                    React.createElement(OverlayTrigger, {placement: "top", overlay: this.props.overlay, delayShow: 300, delayHide: 150}, 
                        button
                    )
            } else {
                body = button;
            }
            
            return (
                React.createElement("div", {style: {width:width+'%', height:0, paddingBottom:height+'%', position:'relative', 'float':'left'}}, 
                    body
                )
            );
        }
    });

    var UiImage = React.createClass({displayName: "UiImage",
        mixins: [RenderAtMixin],
    
        renderAt: function() {
            var image = this.props.image;
            image.aspect = image.height / image.width;
            
            var width = this.props.width;
            var height = width * image.aspect;
            
            var style = {backgroundImage: 'url('+image.url+')'};
            style = _.assign(style, this.props.style);
            
            var img = React.createElement("div", {className: "UiImage", style: style});
            
            var body;
            if (this.props.overlay) {
                body = 
                    React.createElement(OverlayTrigger, {placement: "top", overlay: this.props.overlay, delayShow: 300, delayHide: 150}, 
                        img
                    )
            } else {
                body = img;
            }
            
            return (
                React.createElement("div", {style: {width:width+'%', height:0, paddingBottom:height+'%', position:'relative', 'float':'left'}}, 
                    body
                )
            );
        }
    });
    
    var ActionButton = React.createClass({displayName: "ActionButton",
        mixins: [RenderAtMixin],
        
        renderAt: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(UiButton, React.__spread({width: 7, image: {width:160, height:160, url:'css/UiButton.png'}},  this.props))
                )
            );
        },
    });

    var UnitBar = React.createClass({displayName: "UnitBar",
        mixins: [RenderAtMixin],
        
        renderAt: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(UiImage, {width: 100, image: {width:869, height:60, url:'css/UnitBar.png'}}), 
                    React.createElement(Div, {pos: pos(0,.92)}, 
                        React.createElement(Div, {pos: pos(2,0)}, React.createElement("p", null, "100")), 
                        React.createElement(Div, {pos: pos(16,0)}, React.createElement("p", null, "100")), 
                        React.createElement(Div, {pos: pos(30,0)}, React.createElement("p", null, "100"))
                    )
                )
            );
        },
    });

    var Minimap = React.createClass({displayName: "Minimap",
        mixins: [RenderAtMixin],
        
        renderAt: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(UiImage, {pos: pos(0,0), width: 100, image: {width:245, height:254, url:'css/Minimap.png'}}), 
                    React.createElement(UiImage, {pos: pos(3.5,.35), width: 91, image: {width:245, height:254, url:'css/FakeMinimap.png'}, style: {position:'absolute',left:0,top:0,visibility:'hidden'}})
                )
            );
        },
    });

    var ChatInput = React.createClass({displayName: "ChatInput",
        mixins: [RenderAtMixin],
        
        getInitialState: function() {
            return {
                value: '',
            }
        },
    
        onTextChange: function() {
            this.setState({
                value:this.refs.input.getValue()
            });
        },

        renderAt: function() {
            return (
                React.createElement(Input, {value: this.state.value, ref: "input", type: "text", addonBefore: "All", onChange: this.onTextChange})
            );
        },
    });
    
    var Gap = React.createClass({displayName: "Gap",
        render: function() {
            return (
                React.createElement("div", {style: {'float':'left','overflow':'hidden','height':'1px',width:this.props.width + '%',}})
            );
        }
    });
    
    return React.createClass({        
        render: function() {
            var tooltip = React.createElement(Popover, {title: "Fireball"}, React.createElement("strong", null, "FIRE!"), " Check this info.");
        
            return (
                React.createElement("div", null, 
                    React.createElement(UnitBar, {pos: pos(50.5,.4), size: width(50)}), 
                    React.createElement(Minimap, {pos: pos(.2,79), size: width(11)}), 

                    React.createElement(Div, {pos: pos(15,0)}, 
                        React.createElement(ChatInput, {pos: pos(.35,80), size: width(49)}), 
                        
                        React.createElement(Div, {pos: pos(0,85)}, 
                            React.createElement(ActionButton, {overlay: tooltip}), 
                            React.createElement(ActionButton, null), 
                            React.createElement(ActionButton, null), 
                            React.createElement(ActionButton, null), 
                            
                            React.createElement(Gap, {width: "1"}), 
                            
                            React.createElement(ActionButton, null), 
                            React.createElement(ActionButton, null), 
                            React.createElement(ActionButton, null)
                        )
                    )
                )
            );
        }
    });
});