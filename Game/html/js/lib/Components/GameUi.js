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

    var UiButton = React.createClass({displayName: "UiButton",
        render: function() {
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
        render: function() {
            var image = this.props.image;
            image.aspect = image.height / image.width;
            
            var width = this.props.width;
            var height = width * image.aspect;
            
            var button = React.createElement("div", {className: "UiImage", style: {backgroundImage: 'url('+image.url+')'}});
            
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
    
    var ActionButton = React.createClass({displayName: "ActionButton",        
        render: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(UiButton, React.__spread({width: 7, image: {width:160, height:160, url:'css/UiButton.png'}},  this.props))
                )
            );
        },
    });

    var UnitBar = React.createClass({displayName: "UnitBar",        
        render: function() {
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
                    React.createElement(Div, {pos: pos(50.5,.4), size: width(50)}, 
                        React.createElement(UnitBar, null)
                    ), 

                    React.createElement(Div, {pos: pos(15,0)}, 
                        React.createElement(Div, {pos: pos(.35,80), size: width(49)}, 
                            React.createElement(Input, {type: "text", addonBefore: "All"})
                        ), 
                        
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