define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, sound, React, ReactBootstrap, interop, events, ui, Chat) {
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
        return <span>{name}<span style={{'float':'right'}}>250</span></span>;
    };

    var setActions = function() {
        var buildingScale = 0.85;
        window.Actions = {
            Fireball: {
                image:Spells.Fireball,
                scale:1,
                tooltip:
                    <Popover title={makeTooltip('Fireball')}>
                        <div>
                            <p>This is a p test.</p>
                            <strong>FIRE!</strong> Everything will <em>burrrrnnn</em>. Ahhh-hahaha.
                            Except dragonlords. They have anti-magic. Also, anything near a dragonlord. Again... uh, anti-magic. But, <em>everything else</em>... burrrrnnns.
                            <br /><br />
                            That includes your own soldiers, so be careful. For real.
                        </div>
                    </Popover>,
            },
            Skeletons: {
                image:Spells.Skeletons,
                scale:1,
                tooltip:
                    <Popover title={makeTooltip('Raise Skeletal Army')}>
                        <strong>Command the dead!</strong> Raise an army of the dead. All corpses not being stomped on will rise up and fight for your cause in the area you select.
                    </Popover>,
            },
            Necromancer: {
                image:Spells.Necromancer,
                scale:1,
                tooltip:
                    <Popover title={makeTooltip('Summon Necromancer')}>
                        <strong>Have <em>someone else</em> command the dead!</strong> Summon forth a single, skillful necromancer at a given location.
                        This lord of death will raise any corpse near them into a skeletal warrior ready to thirst for blood and brains.
                    </Popover>,
            },
            Terracotta: {
                image:Spells.Terracotta,
                scale:1,
                tooltip:
                    <Popover title={makeTooltip('Raise Terracotta Army')}>
                        <strong>Clay soldiers! YESSSS.</strong> Mother Earth says: take my earth-warrior-children things! Use them to slay the filthy humans and/or animals!
                        Kill everything! Mother Earth AAANGRRY.
                        Seriously. In a given <strong>open</strong> area you select, summon forth an army of clay warriors to do your worst biddings.
                    </Popover>,
            },
            
            Barracks: {
                image:Buildings.Barracks,
                scale:buildingScale,
                tooltip:
                    <Popover title={makeTooltip('Build Barracks')}>
                        <strong>The engine of war.</strong> This building that dudes hang out in and train for battle and stuff. Also where new 'recruits' magically appear, ready for battle.
                    </Popover>,
            },
            GoldMine: {
                image:Buildings.GoldMine,
                scale:buildingScale,
                tooltip:
                    <Popover title={makeTooltip('Build Gold Mine')}>
                        <strong>Gooooolllld.</strong> Place this on a gold source on the map. Once built the mine will continuously generate gold for your mastermind campaign.
                    </Popover>,
            },
            JadeMine: {
                image:Buildings.JadeMine,
                scale:buildingScale,
                tooltip:
                    <Popover title={makeTooltip('Build Jade Mine')}>
                        <strong>Green is the color of... MAGIC.</strong> From Jade flows all magic, both real and imaginary. Place this jade mine on a jade source on the map.
                        Once built the mine will continuously generate jade for you to use in super sweet <strong>Dragonlord spells</strong>.
                    </Popover>,
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
    

    var Cost = React.createClass({
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
            return {
            };
        },

        render: function() {
            return (
                <p>{this.state.goldCost || this.state.jadeCost}</p>
            );
        },
    });    
    
    var ActionButton = React.createClass({
        mixins: [RenderAtMixin],

        onClick: function() {
            if (interop.InXna()) {
                xna.ActionButtonPressed(this.props.name);
            }
        },

        renderAt: function() {
            var action = Actions[this.props.name];
            
            var pStyle = {fontSize: '90%', textAlign: 'right'};
            
            return (
                <Div pos={pos(0,0,'relative')} size={size(7,100)} style={{'float':'left','display':'inline-block'}}>
                    <UiButton width={100} image={{width:160, height:160, url:'css/UiButton.png'}}
                     onClick={this.onClick}
                     overlay={action.tooltip} />
                    
                    <Div nonBlocking pos={pos(0,0)}>
                        <UiImage nonBlocking pos={pos(-1 + (100-90*action.scale)/2,-0.5)} width={90*action.scale} image={action.image} />
                    </Div>

                    <Div nonBlocking pos={pos(-16,8.5)} size={width(100)} style={pStyle}><Cost name={this.props.name} /></Div>
                </Div>
            );
        },
    });
    
    var UnitBarItem = React.createClass({
        render: function() {
            return (
                <Div pos={pos(2+this.props.index * 14,0)}><p>{this.props.data}</p></Div>
            );
        },
    });
    
    var UnitBar = React.createClass({
        mixins: [RenderAtMixin, events.UpdateMixin],
                
        onUpdate: function(values) {
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
                <Div nonBlocking pos={p}>
                    <UiImage nonBlocking pos={image_pos} width={4.2*scale} image={image} />
                    <p style={{paddingLeft:'5%', 'pointer-events': 'none'}}>
                        {data}
                    </p>
                </Div>
            );
        },
        
        renderAt: function() {
            var x = 2;
            var small = 13.2, big = 17.2;
            
            var Images = playerImages[this.props.MyPlayerNumber];
            var Buildings = Images.Buildings;
            var Units = Images.Units;
        
            return (
                <div>
                    <UiImage width={100} image={{width:869, height:60, url:'css/UnitBar.png'}} />
                    <Div nonBlocking pos={pos(0,0.92)}>
                        {this.item(pos(x,0),        Buildings.Barracks, 1,    pos(0,0),     this.state.info ? this.state.info.Barracks.Count : 0)}
                        {this.item(pos(x+=small,0), Units.Soldier,      0.85, pos(0.4,0),   this.state.info ? this.state.info.Units : 0)}
                        {this.item(pos(x+=big,0),   Buildings.GoldMine, 1,    pos(0,0),     this.state.info ? this.state.info.GoldMine.Count : 0)}
                        {this.item(pos(x+=small,0), GoldImage,          0.67, pos(1.2,0.5), this.state.info ? this.state.info.Gold : 0)}
                        {this.item(pos(x+=big,0),   Buildings.JadeMine, 1,    pos(0,0),     this.state.info ? this.state.info.JadeMine.Count : 0)}
                        {this.item(pos(x+=small,0), JadeImage,          0.67, pos(1.2,0.5), this.state.info ? this.state.info.Jade : 0)}
                    </Div>
                </div>
            );
        },
    });

    var MenuButton = React.createClass({
        mixins: [RenderAtMixin],
                        
        renderAt: function() {
            var pStyle = {fontSize: '90%', textAlign: 'right'};

            return (
                <div>
                    <Div nonBlocking pos={pos(0,0,'absolute')} style={{'float':'right', 'pointer-events':'auto'}}>
                        <Button style={{position:'absolute', 'pointer-events':'auto'}}
                                onClick={function() { window.setScreen('in-game-menu'); sound.play.click(); }}>
                            <Glyphicon glyph='arrow-up' />
                        </Button>
                    </Div>
                </div>
            );
        },
    });

    var Minimap = React.createClass({
        mixins: [RenderAtMixin],
        
        renderAt: function() {
            return (
                <div>
                    <UiImage pos={pos(0,0)} width={100} image={{width:245, height:254, url:'css/Minimap.png'}} />
                    <UiImage pos={pos(3.5,0.35)} width={91} image={{width:245, height:254, url:'css/FakeMinimap.png'}} style={{position:'absolute',left:0,top:0,visibility:'hidden'}}/>
                </div>
            );
        },
    });

    var UnitBox = React.createClass({
        mixins: [RenderAtMixin, events.UpdateMixin],

        onUpdate: function(values) {
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
                <div>
                    <UiImage pos={pos(0,0)} width={100} image={{width:502, height:157, url:'css/UnitBox.png'}} />
                    <Div nonBlocking pos={pos(-6,5)}><p style={{fontSize: '3.3%', textAlign: 'right'}}>{this.state.value}</p></Div>
                </div>
            );
        },
    });
    
    return React.createClass({
        mixins: [events.UpdateMixin, events.ShowUpdateMixin],

        componentDidMount: function() {
            interop.enableGameInput();
        },
        
        componentWillUnmount: function() {
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
        
        render: function() {
            var players = this.state.ShowAllPlayers ? _.range(1,5) : [this.state.MyPlayerNumber];

            return (
                <div>
                    <Div pos={pos(0,0)}>
                        {_.map(players, function(player, index) {
                            return <UnitBar MyPlayerNumber={player} pos={pos(50.5,0.4 + index*4.2)} size={width(50)} />;
                        })}
                    </Div>
                                        
                    {/*<Minimap pos={pos(.2,79)} size={width(11)} />*/}

                    <MenuButton pos={pos(0.5,0.4)} size={width(50)} />

                    <Div pos={pos(15,0)}>
                        <Chat.ChatInput pos={pos(0.35,80)} size={width(49)} />

                        {/*<Chat.ChatBox pos={pos(.38, this.state.ShowChat ? 80 : 85)} size={width(38)}/>*/}
                        <Chat.ChatBox pos={pos(0.38, 78)} size={width(38)}/>
                        
                        <Div pos={pos(0,85)}>
                            <ActionButton name='Fireball' />
                            <ActionButton name='Skeletons' />
                            <ActionButton name='Necromancer' />
                            <ActionButton name='Terracotta' />
                            
                            <Gap width='1' />
                            
                            <ActionButton name='Barracks' />
                            <ActionButton name='GoldMine' />
                            <ActionButton name='JadeMine' />
                        </Div>
                        
                        <UnitBox pos={pos(58,85)} size={width(25)} />
                    </Div>
                </div>
            );
        }
    });
});