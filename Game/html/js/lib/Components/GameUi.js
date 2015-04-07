define(['lodash', 'react', 'react-addons', 'react-bootstrap', 'interop'], function(_, React, ReactAddons, ReactBootstrap, interop) {
    var Input = ReactBootstrap.Input;
    var OverlayTrigger = ReactBootstrap.OverlayTrigger;
    var Popover = ReactBootstrap.Popover;
    var Button = ReactBootstrap.Button;
    var PureRenderMixin = ReactAddons.PureRenderMixin;

    var updateEvent = [];
    window.values = {};
    window.update = function(values) {
        window.values = values;

        //values.PlayerInfo[1].Barracks.Count;
        
        _.each(updateEvent, function(item) {
            item.update(values);
        });        
    };

    var UpdateMixin = {
        componentDidMount: function() {
            updateEvent.push(this);
        },
        
        componentWillUnmount: function() {
            _.remove(updateEvent, function(e) { e === this; });
        },
    };

    var subImage = function(image, offset) {
        var sub = _.assign({}, image);
        sub.offset = offset;
        return sub;
    };

    var setGlobalImages = function() {
        window.SpellsSpritesheet = {width:300, height:300, dim:[1,4], url:'css/SpellIcons.png'};
        window.BuildingsSpritesheet = {width:96, height:96, dim:[5,10], url:'css/Buildings_1.png'};
        window.UnitSpritesheet = {width:32, height:32, dim:[32,96], url:'css/Soldier_1.png'};

        window.Spells = {
            Fireball: subImage(SpellsSpritesheet, [0,0]),
            Skeletons: subImage(SpellsSpritesheet, [0,1]),
            Necromancer: subImage(SpellsSpritesheet, [0,2]),
            Terracotta: subImage(SpellsSpritesheet, [0,3]),
        };

        window.GoldImage = {width:20, height:22, url:'css/Gold.png'};
        window.JadeImage = {width:20, height:22, url:'css/Jade.png'};    
    };

    var getPlayerImages = function(player) {
        var i = player - 1;
        
        return {        
            Buildings: {
                Barracks: subImage(BuildingsSpritesheet, [i+1,1]),
                GoldMine: subImage(BuildingsSpritesheet, [i+1,3]),
                JadeMine: subImage(BuildingsSpritesheet, [i+1,5]),
            },
            
            Units: {
                Soldier: subImage(UnitSpritesheet, [0,4*i]),
            },
        };
    };
    
    var makeTooltip = function(name) {
        return React.createElement("span", null, name, React.createElement("span", {style: {'float':'right'}}, "250"))
    };

    var setActions = function() {
        var buildingScale = .85;
        window.Actions = {
            Fireball: {
                image:Spells.Fireball,
                scale:1,
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Fireball')}, 
                        React.createElement("div", null, 
                            React.createElement("p", null, "This is a p test."), 
                            React.createElement("strong", null, "FIRE!"), " Everything will ", React.createElement("em", null, "burrrrnnn"), ". Ahhh-hahaha." + ' ' +
                            "Except dragonlords. They have anti-magic. Also, anything near a dragonlord. Again... uh, anti-magic. But, ", React.createElement("em", null, "everything else"), "... burrrrnnns.", 
                            React.createElement("br", null), React.createElement("br", null), 
                            "That includes your own soldiers, so be careful. For real."
                        )
                    ),
            },
            Skeletons: {
                image:Spells.Skeletons,
                scale:1,
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Raise Skeletal Army')}, 
                        React.createElement("strong", null, "Command the dead!"), " Raise an army of the dead. All corpses not being stomped on will rise up and fight for your cause in the area you select."
                    ),
            },
            Necromancer: {
                image:Spells.Necromancer,
                scale:1,
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Summon Necromancer')}, 
                        React.createElement("strong", null, "Have ", React.createElement("em", null, "someone else"), " command the dead!"), " Summon forth a single, skillful necromancer at a given location." + ' ' +
                        "This lord of death will raise any corpse near them into a skeletal warrior ready to thirst for blood and brains."
                    ),
            },
            Terracotta: {
                image:Spells.Terracotta,
                scale:1,
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Raise Terracotta Army')}, 
                        React.createElement("strong", null, "Clay soldiers! YESSSS."), " Mother Earth says: take my earth-warrior-children things! Use them to slay the filthy humans and/or animals!" + ' ' +
                        "Kill everything! Mother Earth AAANGRRY." + ' ' +
                        "Seriously. In a given ", React.createElement("strong", null, "open"), " area you select, summon forth an army of clay warriors to do your worst biddings."
                    ),
            },
            
            Barracks: {
                image:Buildings.Barracks,
                scale:buildingScale,
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Build Barracks')}, 
                        React.createElement("strong", null, "The engine of war."), " This building that dudes hang out in and train for battle and stuff. Also where new 'recruits' magically appear, ready for battle."
                    ),
            },
            GoldMine: {
                image:Buildings.GoldMine,
                scale:buildingScale,
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Build Gold Mine')}, 
                        React.createElement("strong", null, "Gooooolllld."), " Place this on a gold source on the map. Once built the mine will continuously generate gold for your mastermind campaign."
                    ),
            },
            JadeMine: {
                image:Buildings.JadeMine,
                scale:buildingScale,
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Build Jade Mine')}, 
                        React.createElement("strong", null, "Green is the color of... MAGIC."), " From Jade flows all magic, both real and imaginary. Place this jade mine on a jade source on the map." + ' ' +
                        "Once built the mine will continuously generate jade for you to use in super sweet ", React.createElement("strong", null, "Dragonlord spells"), "."
                    ),
            },
        };
    };
    
    var setPlayerImages = function() {
        window.playerImages = _.map(_.range(5), function(player) { return getPlayerImages(player); });
    };
    
    var setPlayer = function(player) {
        _.assign(window, window.playerImages[player]);
        setActions();
    };
    
    setGlobalImages();
    setPlayerImages();
    setPlayer(1);
    

    
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
            
            style = _.assign(style, this.props.pos, this.props.size, this.props.style);
        
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
        
        getInitialState: function() {
            return {
                preventTooltip: false,
            };
        },
        
        onClick: function() {
            if (this.props.onClick) {
                this.props.onClick();
            }
            
            this.setState({
                preventTooltip: true,
            });
        },

        onMouseOut: function() {
            this.setState({
                preventTooltip: false,
            });
        },
        
        renderAt: function() {            
            var image = this.props.image;
            image.aspect = image.height / image.width;
            
            var width = this.props.width;
            var height = width * image.aspect;
            
            var button = React.createElement("button", {className: "UiButton", style: {backgroundImage: 'url('+image.url+')'}, onClick: this.onClick});
            
            var body;
            if (this.props.overlay && !this.state.preventTooltip) {
                body = 
                    React.createElement(OverlayTrigger, {placement: "top", overlay: this.props.overlay, delayShow: 420, delayHide: 50}, 
                        button
                    )
            } else {
                body = button;
            }
            
            return (
                React.createElement("div", {style: {width:width+'%', height:0, paddingBottom:height+'%', position:'relative', 'float':'left'}, onMouseOut: this.onMouseOut}, 
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
            
            var offset = image.offset || this.props.offset;            
            var width = this.props.width;
            var height = width * image.aspect;
                        
            var background_x = 0, background_y = 0;
            if (offset) {
                background_x = image.dim[0] <= 1 ? 0 : 100 * offset[0] / (image.dim[0] - 1);
                background_y = image.dim[1] <= 1 ? 0 : 100 * offset[1] / (image.dim[1] - 1);
            }
            
            var backgroundPos = background_x + '%' + ' ' + background_y + '%';

            var background_x = 0, background_y = 0;
            background_size_x = image.dim && image.dim[0] >= 1 ? 100 * image.dim[0] : 100;
            background_size_y = image.dim && image.dim[1] >= 1 ? 100 * image.dim[1] : 100;
            var backgroundSize = background_size_x + '%' + ' ' + background_size_y + '%';
            
            var style = {backgroundImage: 'url('+image.url+')', backgroundPosition:backgroundPos, backgroundSize:backgroundSize};
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

        onClick: function() {
            if (interop.InXna()) {
                xna.ActionButtonPressed(this.props.name);
            }
        },

        renderAt: function() {
            var action = Actions[this.props.name];
            
            var noBlockingStyle = {'pointer-events':'none'};
            var pStyle = _.assign({}, noBlockingStyle, {fontSize: '90%', textAlign: 'right'});
            
            return (
                React.createElement(Div, {pos: pos(0,0,'relative'), size: size(7,100), style: {'float':'left','display':'inline-block'}}, 
                    React.createElement(UiButton, {width: 100, image: {width:160, height:160, url:'css/UiButton.png'}, 
                     onClick: this.onClick, 
                     overlay: action.tooltip}), 
                    
                    React.createElement(Div, {pos: pos(0,0), style: noBlockingStyle}, 
                        React.createElement(UiImage, {pos: pos(-1 + (100-90*action.scale)/2,-.5), width: 90*action.scale, image: action.image})
                    ), 

                    React.createElement(Div, {pos: pos(-16,8.5), size: width(100), style: pStyle}, React.createElement("p", null, "100"))
                )
            );
        },
    });
    
    var UnitBarItem = React.createClass({displayName: "UnitBarItem",
        render: function() {
            return (
                React.createElement(Div, {pos: pos(2+this.props.index * 14,0)}, React.createElement("p", null, this.props.data))
            );
        },
    });
    
    var UnitBar = React.createClass({displayName: "UnitBar",
        mixins: [RenderAtMixin, UpdateMixin],
                
        update: function(values) {
            this.setState({
                info: values.MyPlayerInfo,
            });
        },
                
        getInitialState: function() {
            return {
                info: null,
            };
        },
        
        item: function(p, image, scale, image_pos, data) {
            return (
                React.createElement(Div, {pos: p}, 
                    React.createElement(UiImage, {pos: image_pos, width: 4.2*scale, image: image}), 
                    React.createElement("p", {style: {paddingLeft:'5%'}}, 
                        data
                    )
                )
            );
        },
        
        renderAt: function() {
            var x = 2;
            var small = 13.2, big = 17.2;
            
            var Images = playerImages[this.props.MyPlayerNumber];
            var Buildings = Images.Buildings;
            var Units = Images.Units;
        
            return (
                React.createElement("div", null, 
                    React.createElement(UiImage, {width: 100, image: {width:869, height:60, url:'css/UnitBar.png'}}), 
                    React.createElement(Div, {pos: pos(0,.92)}, 
                        this.item(pos(x,0),        Buildings.Barracks, 1, pos(0,0), this.state.info ? this.state.info.Barracks.Count : 0), 
                        this.item(pos(x+=small,0), Units.Soldier,    .85, pos(.4,0), this.state.info ? this.state.info.Units : 0), 
                        this.item(pos(x+=big,0),   Buildings.GoldMine, 1, pos(0,0), this.state.info ? this.state.info.GoldMine.Count : 0), 
                        this.item(pos(x+=small,0), GoldImage,         .67, pos(1.2,.5), this.state.info ? this.state.info.Gold : 0), 
                        this.item(pos(x+=big,0),   Buildings.JadeMine, 1, pos(0,0), this.state.info ? this.state.info.JadeMine.Count : 0), 
                        this.item(pos(x+=small,0), JadeImage,         .67, pos(1.2,.5), this.state.info ? this.state.info.Jade : 0)

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
            };
        },
    
        onTextChange: function() {
            this.setState({
                value:this.refs.input.getValue()
            });
        },
        
        focus: function() {
            this.refs.input.getInputDOMNode().focus();
        },

        componentDidMount: function() {
            this.focus();
        },
        
        renderAt: function() {
            return (
                React.createElement(Input, {value: this.state.value, ref: "input", type: "text", addonBefore: "All", onChange: this.onTextChange})
            );
        },
    });

    var UnitBox = React.createClass({displayName: "UnitBox",
        mixins: [RenderAtMixin, UpdateMixin],

        update: function(values) {
            this.setState({
                value: values.UnitCount,
            });
        },

        getInitialState: function() {
            return {
                value: 0,
            };
        },
        
        renderAt: function() {
            return (
                React.createElement("div", null, 
                    React.createElement(UiImage, {pos: pos(0,0), width: 100, image: {width:502, height:157, url:'css/UnitBox.png'}}), 
                    React.createElement(Div, {pos: pos(-6,5)}, React.createElement("p", {style: {fontSize: '3.3%', textAlign: 'right'}}, this.state.value))
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
        mixins: [PureRenderMixin, UpdateMixin],
                
        update: function(values) {
            if (this.state.MyPlayerNumber === values.MyPlayerNumber &&
                this.state.ShowChat === values.ShowChat &&
                this.state.ShowAllPlayers === values.ShowAllPlayers) {
                
                return;
            }
            
            if (this.state.MyPlayerNumber !== values.MyPlayerNumber) {
                setPlayer(values.MyPlayerNumber);
            }
            
            this.setState({
                MyPlayerNumber: values.MyPlayerNumber,
                ShowChat: values.ShowChat,
                ShowAllPlayers: values.ShowAllPlayers,
            });
        },

        getInitialState: function() {
            /* Test PureRenderMixin
            var self = this;
            setInterval(function() {
                var player = self.state.MyPlayerNumber + 1;
                if (player > 4) player = 1;
                player = 1;
                
                self.setState({MyPlayerNumber: player});
            }, 200);*/
        
            return {
                MyPlayerNumber: 1,
                ShowChat: true,
                ShowAllPlayers: false,
            };
        },
        
        render: function() {
            var players = this.state.ShowAllPlayers ? _.range(1,5) : [this.state.MyPlayerNumber];

            console.log('render ' + this.state.MyPlayerNumber);
            
            return (
                React.createElement("div", null, 
                    React.createElement("div", null, 
                        _.map(players, function(player, index) {
                            return React.createElement(UnitBar, {MyPlayerNumber: player, pos: pos(50.5,.4 + index*4.2), size: width(50)});
                        })
                    ), 
                                        
                    /*<Minimap pos={pos(.2,79)} size={width(11)} />*/

                    React.createElement(Div, {pos: pos(15,0)}, 
                        this.state.ShowChat ? React.createElement(ChatInput, {pos: pos(.35,80), size: width(49)}) : null, 
                        
                        React.createElement(Div, {pos: pos(0,85)}, 
                            React.createElement(ActionButton, {name: "Fireball"}), 
                            React.createElement(ActionButton, {name: "Skeletons"}), 
                            React.createElement(ActionButton, {name: "Necromancer"}), 
                            React.createElement(ActionButton, {name: "Terracotta"}), 
                            
                            React.createElement(Gap, {width: "1"}), 
                            
                            React.createElement(ActionButton, {name: "Barracks"}), 
                            React.createElement(ActionButton, {name: "GoldMine"}), 
                            React.createElement(ActionButton, {name: "JadeMine"})
                        ), 
                        
                        React.createElement(UnitBox, {pos: pos(58,85), size: width(25)})
                    )
                )
            );
        }
    });
});