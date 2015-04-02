define(['lodash', 'react', 'react-bootstrap', 'interop'], function(_, React, ReactBootstrap, interop) {
    var Input = ReactBootstrap.Input;
    var OverlayTrigger = ReactBootstrap.OverlayTrigger;
    var Popover = ReactBootstrap.Popover;
    var Button = ReactBootstrap.Button;

    var updateEvent = [];
    window.update = function(values) {
        // if (values.unitCount && window.values.unitCount && values.unitCount !== window.values.unitCount)
            // console.log(values.unitCount);
            
        window.values = values;
        
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

    var Div = React.createClass({
        render: function() {
            var style = {
                width:'100%',
                height:'100%',
            };
            
            style = _.assign(style, this.props.pos, this.props.size, this.props.style);
        
            return (
                <div style={style}>
                    {this.props.children}
                </div>
            );
        }
    });

    var RenderAtMixin = {
        render: function() {
            if (this.props.pos) {
                return (
                    <Div {...this.props}>
                        {this.renderAt()}
                    </Div>
                );
            } else {
                return this.renderAt();
            }
        }
    };

    var UiButton = React.createClass({
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
            
            var button = <button className="UiButton" style={{backgroundImage: 'url('+image.url+')'}} onClick={this.onClick} />;
            
            var body;
            if (this.props.overlay && !this.state.preventTooltip) {
                body = 
                    <OverlayTrigger placement="top" overlay={this.props.overlay} delayShow={420} delayHide={50}>
                        {button}
                    </OverlayTrigger>
            } else {
                body = button;
            }
            
            return (
                <div style={{width:width+'%', height:0, paddingBottom:height+'%', position:'relative', 'float':'left'}} onMouseOut={this.onMouseOut} >
                    {body}
                </div>
            );
        }
    });

    var UiImage = React.createClass({
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
            
            var img = <div className="UiImage" style={style} />;
            
            var body;
            if (this.props.overlay) {
                body = 
                    <OverlayTrigger placement="top" overlay={this.props.overlay} delayShow={300} delayHide={150}>
                        {img}
                    </OverlayTrigger>
            } else {
                body = img;
            }
            
            return (
                <div style={{width:width+'%', height:0, paddingBottom:height+'%', position:'relative', 'float':'left'}}>
                    {body}
                </div>
            );
        }
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
            
            var noBlockingStyle = {'pointer-events':'none'};
            var pStyle = _.assign({}, noBlockingStyle, {fontSize: '90%', textAlign: 'right'});
            
            return (
                <Div pos={pos(0,0,'relative')} size={size(7,100)} style={{'float':'left','display':'inline-block'}}>
                    <UiButton width={100} image={{width:160, height:160, url:'css/UiButton.png'}}
                     onClick={this.onClick}
                     overlay={action.tooltip} />
                    
                    <Div pos={pos(0,0)} style={noBlockingStyle}>
                        <UiImage pos={pos(-1 + (100-90*action.scale)/2,-.5)} width={90*action.scale} image={action.image} />
                    </Div>

                    <Div pos={pos(-16,8.5)} size={width(100)} style={pStyle}><p>100</p></Div>
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
        
        item: function(pos, data) {
            return (
                <Div pos={pos}>
                    <p>
                        {data}
                    </p>
                </Div>
            );
        },
        
        renderAt: function() {
            var x = 2;
            var incr = 14;
        
            return (
                <div>
                    <UiImage width={100} image={{width:869, height:60, url:'css/UnitBar.png'}} />
                    <Div pos={pos(0,.92)}>
                        {this.item(pos(x,0), this.state.info ? this.state.info.Barracks.Count : 0)}
                        {this.item(pos(x+=incr,0), this.state.info ? this.state.info.Units : 0)}
                        {this.item(pos(x+=incr,0), this.state.info ? this.state.info.GoldMine.Count : 0)}
                        {this.item(pos(x+=incr,0), this.state.info ? this.state.info.Gold : 0)}
                        {this.item(pos(x+=incr,0), this.state.info ? this.state.info.JadeMine.Count : 0)}
                        {this.item(pos(x+=incr,0), this.state.info ? this.state.info.Jade : 0)}

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
                    <UiImage pos={pos(3.5,.35)} width={91} image={{width:245, height:254, url:'css/FakeMinimap.png'}} style={{position:'absolute',left:0,top:0,visibility:'hidden'}}/>
                </div>
            );
        },
    });

    var ChatInput = React.createClass({
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

        renderAt: function() {
            return (
                <Input value={this.state.value} ref="input" type="text" addonBefore="All" onChange={this.onTextChange} />
            );
        },
    });

    var UnitBox = React.createClass({
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
                <div>
                    <UiImage pos={pos(0,0)} width={100} image={{width:502, height:157, url:'css/UnitBox.png'}} />
                    <Div pos={pos(-6,5)}><p style={{fontSize: '3.3%', textAlign: 'right'}}>{this.state.value}</p></Div>
                </div>
            );
        },
    });

    var Gap = React.createClass({
        render: function() {
            return (
                <div style={{'float':'left','overflow':'hidden','height':'1px',width:this.props.width + '%',}} />
            );
        }
    });

    var makeTooltip = function(name) {
        return <span>{name}<span style={{'float':'right'}}>250</span></span>
    };

    var subImage = function(image, offset) {
        var sub = _.assign({}, image);
        sub.offset = offset;
        return sub;
    };

    var SpellsSpritesheet = {width:300, height:300, dim:[1,4], url:'css/SpellIcons.png'};
    var BuildingsSpritesheet = {width:96, height:96, dim:[5,10], url:'css/Buildings_1.png'};
    
    var Spells = {
        Fireball: subImage(SpellsSpritesheet, [0,0]),
        Skeletons: subImage(SpellsSpritesheet, [0,1]),
        Necromancer: subImage(SpellsSpritesheet, [0,2]),
        Terracotta: subImage(SpellsSpritesheet, [0,3]),
    };
    
    var Buildings = {
        Barracks: subImage(BuildingsSpritesheet, [1,1]),
        GoldMine: subImage(BuildingsSpritesheet, [1,3]),
        JadeMine: subImage(BuildingsSpritesheet, [1,5]),
    };
    
    
    var buildingScale = .85;
    var Actions = {
        Fireball: {
            image: Spells.Fireball,
            scale:1,
            tooltip:
                <Popover title={makeTooltip('Fireball')}>
                    <div>
                        <p>This is a p test.</p>
                        <strong>FIRE!</strong> Everything will <em>burrrrnnn</em>. Ahhh-hahaha.
                        Um. Except dragonlords. They have anti-magic and stuff. Also, anything near a dragonlord. Again... uh, anti-magic. But, <em>everything else</em>... burrrrnnns. Including your own soldiers, so be careful. For real.
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
    
    return React.createClass({        
        render: function() {
            return (
                <div>
                    <UnitBar pos={pos(50.5,.4)} size={width(50)} />
                    {/*<Minimap pos={pos(.2,79)} size={width(11)} />*/}

                    <Div pos={pos(15,0)}>
                        {/*<ChatInput pos={pos(.35,80)} size={width(49)} />*/}
                        
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