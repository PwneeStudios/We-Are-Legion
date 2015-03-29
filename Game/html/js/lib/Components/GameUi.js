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

    var UnitBox = React.createClass({displayName: "UnitBox",
        mixins: [RenderAtMixin],
        
        renderAt: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(UiImage, {pos: pos(0,0), width: 100, image: {width:502, height:157, url:'css/UnitBox.png'}}), 
                    React.createElement(Div, {pos: pos(-6,5)}, React.createElement("p", {style: {fontSize: '3.3%', textAlign: 'right'}}, "162,581"))
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

    var Tooltips = {
        'Fireball': {
            tooltip:
                React.createElement(Popover, {className: "UiButton", title: "Fireball"}, 
                    React.createElement("p", null, "This is a p test."), 
                    React.createElement("strong", null, "FIRE!"), " Everything will ", React.createElement("em", null, "burrrrnnn"), ". Ahhh-hahaha." + ' ' +
                    "Um. Except dragonlords. They have anti-magic and stuff. Also, anything near a dragonlord. Again... uh, anti-magic. But, ", React.createElement("em", null, "everything else"), "... burrrrnnns. Including your own soldiers, so be careful. For real."
                ),
        },
        'Skeletons': {
            tooltip:
                React.createElement(Popover, {title: "Raise Skeletal Army"}, 
                    React.createElement("strong", null, "Command the dead!"), " Raise an army of the dead. All corpses not being stomped on will rise up and fight for your cause in the area you select."
                ),
        },
        'Necromancer': {
            tooltip:
                React.createElement(Popover, {title: "Summon Necromancer"}, 
                    React.createElement("strong", null, "Have ", React.createElement("em", null, "someone else"), " command the dead!"), " Summon forth a single, skillful necromancer at a given location." + ' ' +
                    "This lord of death will raise any corpse near them into a skeletal warrior ready to thirst for blood and brains."
                ),
        },
        'Terracotta': {
            tooltip:
                React.createElement(Popover, {title: "Raise Terracotta Army"}, 
                    React.createElement("strong", null, "Clay soldiers! YESSSS."), " Mother Earth says: take my earth-warrior-children things! Use them to slay the filthy humans and/or animals!" + ' ' +
                    "Kill everything! Mother Earth AAANGRRY." + ' ' +
                    "Seriously. In a given ", React.createElement("strong", null, "open"), " area you select, summon forth an army of clay warriors to do your worst biddings."
                ),
        },
        
        'Barracks': {
            tooltip:
                React.createElement(Popover, {title: "Build Barracks"}, 
                    React.createElement("strong", null, "The engine of war."), " This building that dudes hang out in and train for battle and stuff. Also where new 'recruits' magically appear, ready for battle."
                ),
        },
        'Goldmine': {
            tooltip:
                React.createElement(Popover, {title: "Build Gold Mine"}, 
                    React.createElement("strong", null, "Gooooolllld."), " Place this on a gold source on the map. Once built the mine will continuously generate gold for your mastermind campaign."
                ),
        },
        'Jademine': {
            tooltip:
                React.createElement(Popover, {title: "Build Jade Mine"}, 
                    React.createElement("strong", null, "Green is the color of... MAGIC."), " From Jade flows all magic, both real and imaginary. Place this jade mine on a jade source on the map." + ' ' +
                    "Once built the mine will continuously generate jade for you to use in super sweet ", React.createElement("strong", null, "Dragonlord spells"), "."
                ),
        },
    };
    
    return React.createClass({        
        render: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(UnitBar, {pos: pos(50.5,.4), size: width(50)}), 
                    React.createElement(Minimap, {pos: pos(.2,79), size: width(11)}), 

                    React.createElement(Div, {pos: pos(15,0)}, 
                        /*<ChatInput pos={pos(.35,80)} size={width(49)} />*/
                        
                        React.createElement(Div, {pos: pos(0,85)}, 
                            React.createElement(ActionButton, {overlay: Tooltips.Fireball.tooltip}), 
                            React.createElement(ActionButton, {overlay: Tooltips.Skeletons.tooltip}), 
                            React.createElement(ActionButton, {overlay: Tooltips.Necromancer.tooltip}), 
                            React.createElement(ActionButton, {overlay: Tooltips.Terracotta.tooltip}), 
                            
                            React.createElement(Gap, {width: "1"}), 
                            
                            React.createElement(ActionButton, {overlay: Tooltips.Barracks.tooltip}), 
                            React.createElement(ActionButton, {overlay: Tooltips.Goldmine.tooltip}), 
                            React.createElement(ActionButton, {overlay: Tooltips.Jademine.tooltip})
                        ), 
                        
                        React.createElement(UnitBox, {pos: pos(58,85), size: width(25)})
                    )
                )
            );
        }
    });
});