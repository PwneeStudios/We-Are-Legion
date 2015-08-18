define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat', 'Components/InGameUtil'], function(_, sound, React, ReactBootstrap, interop, events, ui, Chat, InGameUtil) {
    var Input = ReactBootstrap.Input;
    var Popover = ReactBootstrap.Popover;
    var Button = ReactBootstrap.Button;
    var Glyphicon = ReactBootstrap.Glyphicon;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var RenderAtMixin = ui.RenderAtMixin;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    makeTooltip = InGameUtil.makeTooltip;

    var setActions = function() {
        var buildingScale = 0.835;
        var buildingY = 0.75;
        window.Actions = {
            Fireball: {
                image:Spells.Fireball,
                scale:1,
                hotkey:'1',
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Fireball', 'Fireball', '1')}, 
                        React.createElement("div", null, 
                            "Fire! Everything will ", React.createElement("em", null, "burrrrnnn"), ". Ahhh-hahaha." + ' ' +
                            "Except dragonlords. They have anti-magic. Also, anything near a dragonlord. Also necromancers. Again... uh, anti-magic. But, ", React.createElement("em", null, "everything else"), "... burrrrnnns.", 
                            React.createElement("br", null), React.createElement("br", null), 
                            "That includes your own soldiers, so be careful. For real."
                        )
                    ),
            },
            Skeletons: {
                image:Spells.Skeletons,
                scale:1,
                hotkey:'2',
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Raise Skeletal Army', 'Skeletons', '2')}, 
                        "Command the dead! Raise an army of the dead. All corpses not being stomped on will rise up and fight for your cause in the area you select."
                    ),
            },
            Necromancer: {
                image:Spells.Necromancer,
                scale:1,
                hotkey:'3',
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Summon Necromancer', 'Necromancer', '3')}, 
                        "Have ", React.createElement("em", null, "someone else"), " command the dead! Summon forth a single, skillful necromancer at a given location." + ' ' +
                        "This lord of death will raise any corpse near them into a skeletal warrior ready to thirst for blood and brains."
                    ),
            },
            Terracotta: {
                image:Spells.Terracotta,
                scale:1,
                hotkey:'4',
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Raise Terracotta Army', 'Terracotta', '4')}, 
                        "Clay soldiers! Yesssss. Mother Earth says: take my earth-warrior-children things! Use them to slay the filthy humans and/or animals!" + ' ' +
                        "Kill everything! Mother Earth AAANGRRY." + ' ' +
                        "Seriously. In a given open area you select, summon forth an army of clay warriors to do your worst biddings."
                    ),
            },
            
            Barracks: {
                image:Buildings.Barracks,
                scale:buildingScale,
                y:buildingY,
                hotkey:'B',
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Build Barracks', 'Barracks', 'B')}, 
                        "The engine of war. This building that dudes hang out in and train for battle and stuff. Also where new 'recruits' magically appear, ready for battle."
                    ),
            },
            GoldMine: {
                image:Buildings.GoldMine,
                scale:buildingScale,
                y:buildingY,
                hotkey:'G',
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Build Gold Mine', 'GoldMine', 'G')}, 
                        "Place this on a gold source on the map. Once built the mine will continuously generate gold for your mastermind campaign."
                    ),
            },
            JadeMine: {
                image:Buildings.JadeMine,
                scale:buildingScale,
                y:buildingY,
                hotkey:'J',
                tooltip:
                    React.createElement(Popover, {title: makeTooltip('Build Jade Mine', 'JadeMine', 'J')}, 
                        "Green is the color of... MAGIC. From Jade flows all magic, both real and imaginary. Place this jade mine on a jade source on the map." + ' ' +
                        "Once built the mine will continuously generate jade for you to use in super sweet Dragonlord Spells."
                    ),
            },
        };
    };
        
    var setPlayer = function(player) {
        _.assign(window, window.playerImages[player]);
        setActions();
    };
    
    InGameUtil.setGlobalImages();
    InGameUtil.setPlayerImages();
    setPlayer(1);
    
    var Cost = React.createClass({displayName: "Cost",
        mixins: [events.SetParamsMixin],

        onSetParams: function(values) {
            var goldCost = 0, jadeCost = 0;

            if (values.SpellCosts[this.props.name]) {
                jadeCost = values.SpellCosts[this.props.name];
            } else {
                var data = values.Buildings[this.props.name];

                goldCost = data.CurrentGoldCost || data.GoldCost || 0;
                jadeCost = data.CurrentJadeCost || data.JadeCost || 0;
            }
            
            if (this.state.goldCost === goldCost && this.state.jadeCost === jadeCost) {
                return;
            }

            this.setState({
                goldCost: goldCost,
                jadeCost: jadeCost,
            });
        },

        getInitialState: function() {
            if (!interop.InXna()) {
                return {
                    goldCost: 1000,
                };
            } else {
                return {
                };
            }
        },

        render: function() {
            return (
                React.createElement("span", null, 
                    React.createElement("p", null, 
                        this.state.goldCost || this.state.jadeCost
                    ), 
                    React.createElement(UiImage, {nonBlocking: true, pos: pos(30,0.075), width: 16, image: this.state.goldCost ? GoldImage : JadeImage})
                )
            );
        },
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
            
            var pStyle = {fontSize: '90%', textAlign: 'right'};
            var costY = 4.55 * window.w / window.h;

            var hotkeyStyle = {
                //'text-shadow': '1px 1px #555',
                'fontSize': '1%',
                'color': 'rgba(255, 255, 255, 0.71)',
            };
            
            return (
                React.createElement(Div, {pos: pos(0,0,'relative'), size: size(7,100), style: {'float':'left','display':'inline-block'}}, 
                    React.createElement(UiButton, {width: 100, image: {width:160, height:160, url:'css/UiButton.png'}, 
                     onClick: this.onClick, 
                     overlay: action.tooltip}), 
                    
                    React.createElement(Div, {nonBlocking: true, pos: pos(0,0)}, 
                        React.createElement(UiImage, {nonBlocking: true, pos: pos(-1 + (100-90*action.scale)/2,-0.5+action.y), width: 90*action.scale, image: action.image})
                    ), 

                    React.createElement(Div, {nonBlocking: true, pos: pos(-16,costY), size: width(100), style: pStyle}, React.createElement(Cost, {name: this.props.name})), 

                    React.createElement(Div, {pos: pos(76,0.8)}, 
                        React.createElement("p", {style: hotkeyStyle}, 
                            action.hotkey
                        )
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
                    React.createElement(UiImage, {pos: pos(3.5,0.35), width: 91, image: {width:245, height:254, url:'css/FakeMinimap.png'}, style: {position:'absolute',left:0,top:0,visibility:'hidden'}})
                )
            );
        },
    });
    
    return React.createClass({
        mixins: [events.UpdateMixin, events.ShowUpdateMixin],

        componentDidMount: function() {
            this.enabled = true;
            interop.enableGameInput();
        },

        componentWillUpdate: function() {
            if (!this.enabled) {
                this.enabled = true;
                interop.enableGameInput();
            }
        },
        
        componentWillUnmount: function() {
            this.enabled = false;
            interop.disableGameInput();
        },

        onShowUpdate: function(values) {
            if (this.state.ShowChat === values.ShowChat &&
                this.state.ShowAllPlayers === values.ShowAllPlayers) {

                return;
            }
            
            this.setState({
                ShowChat: values.ShowChat,
                ShowAllPlayers: values.ShowAllPlayers,
            });
        },
        
        onUpdate: function(values) {
            if (this.state.MyPlayerNumber === values.MyPlayerNumber) {                
                return;
            }
            
            if (this.state.MyPlayerNumber !== values.MyPlayerNumber) {
                setPlayer(values.MyPlayerNumber);
            }
            
            this.setState({
                MyPlayerNumber: values.MyPlayerNumber,
            });
        },

        getInitialState: function() {
            return {
                MyPlayerNumber: 1,
                ShowChat: true,
                ShowAllPlayers: false,
            };
        },
        
        lerp: function(x1, y1, x2, y2, t)
        {
            var width = x2 - x1;
            s = (t - x1) / width;

            return y2 * s + y1 * (1 - s);
        },

        render: function() {
            var players = this.state.ShowAllPlayers ? _.range(1,5) : [this.state.MyPlayerNumber];

            var aspect = window.w / window.h;
            var xOffset = this.lerp(1.6, 2, 1, 5.6, aspect);

            return (
                React.createElement("div", null, 
                    React.createElement(Div, {pos: pos(0,0)}, 
                        _.map(players, function(player, index) {
                            return React.createElement(InGameUtil.UnitBar, {MyPlayerNumber: player, pos: pos(50.5,0.4 + index*4.2), size: width(50)});
                        })
                    ), 
                                        
                    /*<Minimap pos={pos(.2,79)} size={width(11)} />*/

                    React.createElement(InGameUtil.MenuButton, {pos: pos(0.5,0.4), size: width(50)}), 

                    React.createElement(Div, {pos: pos(15,0)}, 
                        React.createElement(Chat.ChatInput, {pos: pos(0.35+xOffset,80), size: width(49)}), 

                        /*<Chat.ChatBox pos={pos(.38, this.state.ShowChat ? 80 : 85)} size={width(38)}/>*/
                        React.createElement(Chat.ChatBox, {pos: pos(0.38+xOffset, 78), size: width(38)}), 
                        
                        React.createElement(Div, {pos: pos(0+xOffset,85)}, 
                            React.createElement(ActionButton, {name: "Fireball"}), 
                            React.createElement(ActionButton, {name: "Skeletons"}), 
                            React.createElement(ActionButton, {name: "Necromancer"}), 
                            React.createElement(ActionButton, {name: "Terracotta"}), 
                            
                            React.createElement(Gap, {width: "1"}), 
                            
                            React.createElement(ActionButton, {name: "Barracks"}), 
                            React.createElement(ActionButton, {name: "GoldMine"}), 
                            React.createElement(ActionButton, {name: "JadeMine"})
                        ), 
                        
                        React.createElement(InGameUtil.UnitBox, {pos: pos(58,85), size: width(25)})
                    )
                )
            );
        }
    });
});