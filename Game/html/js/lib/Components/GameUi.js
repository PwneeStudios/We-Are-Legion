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

    var width = function(x) {
        return {
            width: x + '%',
        };
    };

    var Div = React.createClass({displayName: "Div",
        render: function() {
            var style = {
                width:'100%',
                height:'100%',
            };
            
            style = _.assign(style, this.props.pos, this.props.size);
        
            console.log(style);
        
            return (
                React.createElement("div", {style: style}, 
                    this.props.children
                )
            );
        }
    });

    var UiButtonMixin = {
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
    };

    var UiImageMixin = {
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
    };
    
    var UiImage = React.createClass({displayName: "UiImage",
        mixins: [UiButtonMixin],
    });
    
    var ActionButton = React.createClass({displayName: "ActionButton",
        mixins: [UiButtonMixin],
        
        getDefaultProps: function() {
            return {
                width: 7,
                
                image: {
                    width:160,
                    height:160,
                    url:'css/UiButton.png',
                },
            };
        },
    });

    var UnitBar = React.createClass({displayName: "UnitBar",
        mixins: [UiImageMixin],
        
        getDefaultProps: function() {
            return {
                width: 100,
                
                image: {
                    width:869,
                    height:60,
                    url:'css/UnitBar.png',
                },
            };
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
                React.createElement(Div, {pos: pos(15,0)}, 
                    React.createElement(Div, {pos: pos(20,5), size: width(50)}, 
                        React.createElement(UnitBar, null)
                    ), 

                    React.createElement(Div, {pos: pos(0,75), size: width(50)}, 
                        React.createElement(Input, {type: "text", addonBefore: "All"})
                    ), 
                    
                    React.createElement(Div, {pos: pos(0,80)}, 
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
            );
        }
    });
});