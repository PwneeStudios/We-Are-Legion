define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui',
        'Components/Chat', 'Components/InGameUtil', 'Components/MapPicker'],
function(_, sound, React, ReactBootstrap, interop, events, ui,
         Chat, InGameUtil, MapPicker) {
    var Input = ReactBootstrap.Input;
    var Popover = ReactBootstrap.Popover;
    var Button = ReactBootstrap.Button;
    var Glyphicon = ReactBootstrap.Glyphicon;
    var OverlayTrigger = ReactBootstrap.OverlayTrigger;
    var ModalTrigger = ReactBootstrap.ModalTrigger;
    var Modal = ReactBootstrap.Modal;
    var OverlayMixin = ReactBootstrap.OverlayMixin;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var RenderAtMixin = ui.RenderAtMixin;
    var Dropdown = ui.Dropdown;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var makeTooltip = InGameUtil.makeTooltip;

    var setActions = function() {
        var buildingScale = 0.835;
        var buildingY = 0.75;
        var tileScale = 0.85;
        var tileY = 1.5;

        window.Actions = {
            Soldier: {
                image:Units.Soldier,
                scale:1,
                hotkey:'R',
                tooltip:
                    <Popover title={makeTooltip('Soldier', 'Soldier', 'R')}>
                        <div>
                            One of many. This is the stock soldier unit that comprises any proper Legion.
                        </div>
                    </Popover>,
            },
            DragonLord: {
                image:Units.DragonLord,
                scale:1,
                hotkey:'T',
                tooltip:
                    <Popover title={makeTooltip('Dragon Lord', 'Dragonlord', 'T')}>
                        The king piece of your Legion. Your Dragon Lord is the source of your magic casting.
                        He also has an anti-magic radius that prevents harmful spells from taking effect near him.
                        In Regicide mode if he is killed you lose the match.
                    </Popover>,
            },
            Necromancer: {
                image:Units.Necromancer,
                scale:1,
                hotkey:'Y',
                tooltip:
                    <Popover title={makeTooltip('Necromancer', 'Necromancer', 'Y')}>
                        This lord of death will raise any corpse near them into a skeletal warrior ready to thirst for blood and brains.
                    </Popover>,
            },
            Skeleton: {
                image:Units.Skeleton,
                scale:1,
                hotkey:'U',
                tooltip:
                    <Popover title={makeTooltip('Skeleton', 'Skeleton', 'U')}>
                        Skeletons are the resurrected warriors of the fallen masses. They do not leave corpses for further resurrection when they die.
                    </Popover>,
            },
            Terracotta: {
                image:Units.Terracotta,
                scale:1,
                hotkey:'I',
                tooltip:
                    <Popover title={makeTooltip('Terracotta Soldier', 'Terracotta', 'I')}>
                        This clay warrior is summoned forth directly from the earth beneath your Dragon Lord's feet to fight for your masses.
                    </Popover>,
            },

            Barracks: {
                image:Buildings.Barracks,
                scale:buildingScale,
                y:buildingY,
                hotkey:'B',
                tooltip:
                    <Popover title={makeTooltip('Build Barracks', 'Barracks', 'B')}>
                        The engine of war. This building that dudes hang out in and train for battle and stuff. Also where new 'recruits' magically appear, ready for battle.
                    </Popover>,
            },
            GoldMine: {
                image:Buildings.GoldMine,
                scale:buildingScale,
                y:buildingY,
                hotkey:'G',
                tooltip:
                    <Popover title={makeTooltip('Build Gold Mine', 'GoldMine', 'G')}>
                        Place this on a gold source on the map. Once built the mine will continuously generate gold for your mastermind campaign.
                    </Popover>,
            },
            JadeMine: {
                image:Buildings.JadeMine,
                scale:buildingScale,
                y:buildingY,
                hotkey:'J',
                tooltip:
                    <Popover title={makeTooltip('Build Jade Mine', 'JadeMine', 'J')}>
                        Green is the color of... MAGIC. From Jade flows all magic, both real and imaginary. Place this jade mine on a jade source on the map.
                        Once built the mine will continuously generate jade for you to use in super sweet Dragonlord Spells.
                    </Popover>,
            },

            Dirt: {
                image:Tiles.Dirt,
                scale:tileScale,
                y:tileY,
                hotkey:'C',
                tooltip:
                    <Popover title={makeTooltip('Dirt', 'Dirt', 'C')}>
                        Dirt tile. Units can walk on this tile type.
                    </Popover>,
            },
            Grass: {
                image:Tiles.Grass,
                scale:tileScale,
                y:tileY,
                hotkey:'V',
                tooltip:
                    <Popover title={makeTooltip('Grass', 'Grass', 'V')}>
                        Grass tile. Units can walk on this tile type.
                    </Popover>,
            },
            Trees: {
                image:Tiles.Trees,
                scale:tileScale,
                y:tileY,
                hotkey:'N',
                tooltip:
                    <Popover title={makeTooltip('Trees', 'Trees', 'N')}>
                        Tree tile. Units cannot walk on or through this tile type.
                    </Popover>,
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
    
    var ActionButton = React.createClass({
        mixins: [RenderAtMixin],

        onClick: function() {
            if (this.props.tile) {
                interop.setTilePaint(this.props.tile)
            } else if (this.props.unit) {
                interop.setUnitPaint(this.props.unit)
            } else if (interop.InXna()) {
                interop.actionButtonPressed(this.props.name);
            }
        },

        renderAt: function() {
            var action = Actions[this.props.name];
            
            var hotkeyStyle = {
                'fontSize': '1%',
                'color': 'rgba(255, 255, 255, 0.71)',
            };
            
            return (
                <Div pos={pos(0,0,'relative')} size={size(7,100)} style={{'float':'left','display':'inline-block'}}>
                    <UiButton width={100} image={{width:160, height:160, url:'css/UiButton.png'}}
                     onClick={this.onClick}
                     overlay={action.tooltip} />
                    
                    <Div nonBlocking pos={pos(0,0)}>
                        <UiImage nonBlocking pos={pos(-1 + (100-90*action.scale)/2,-0.5+action.y)} width={90*action.scale} image={action.image} />
                    </Div>

                    <Div pos={pos(76,0.8)}>
                        <p style={hotkeyStyle}>
                            {action.hotkey}
                        </p>
                    </Div>
                </Div>
            );
        },
    });

    var TopButton = React.createClass({
        mixins: [RenderAtMixin],

        makeTooltip: function(name, hotkey) {
            return (
                <h3>
                    {name}
                    &nbsp;&nbsp;&nbsp;

                    <span style={{'float':'right','font-size':'80%'}}>
                        (Hot key {hotkey})
                    </span>
                </h3>
            );
        },

        onClick: function() {
            if (this.props.onClick) {
                this.props.onClick();
            }

            interop.editorUiClicked();
            sound.play.click();
        },
                        
        renderAt: function() {
            var button = (
                <Button style={{'pointer-events':'auto', 'float':'left','display':'inline-block'}}
                        onClick={this.onClick}
                        onMouseEnter={interop.onOver} onMouseLeave={interop.onLeave} >
                    {this.props.children}
                </Button>
            );

            if (this.props.tooltip) {
                var overlay = (
                    <Popover>
                        {this.makeTooltip(this.props.tooltip,this.props.hotkey)}
                    </Popover>
                );

                return (
                    <OverlayTrigger placement={this.props.placement || 'bottom'} overlay={overlay} delayShow={420} delayHide={50}>
                        {button}
                    </OverlayTrigger>
                );
            } else {
                return button;
            }
        },
    });

    var MapName = React.createClass({
        mixins: [RenderAtMixin],
                        
        renderAt: function() {
            var style = {
                'pointer-events':'auto',
                'background-color':'lightgray',
            };

            return (
                <div style={{'width':'43%'}}>
                    <Input value={this.props.name} ref="input" type="text"
                        addonBefore='Map'
                        style={style}
                      />
                </div>
            );
        },
    });

    var Choose = React.createClass({
        mixins: [],

        componentWillReceiveProps: function(nextProps) {
            this.setState(this.getInitialState(nextProps));
        },
                
        getInitialState: function(props) {
            if (typeof props === 'undefined') {
                props = this.props;
            }

            var self = this;
            var selected = props.choices[0];

            _.forEach(props.choices, function(choice) {
                if (choice.value === props.value) {
                    selected = choice;
                }
            });

            return {
                selected: selected,
            };
        },
        
        onSelected: function(item) {
            if (this.props.onSelect) {
                this.props.onSelect(item);
            }

            this.setState({
                selected: item,
            });
        },

        render: function() {
            return (
                <Dropdown style={{'float':'left'}} dropup
                          disabled={this.props.disabled}
                          selected={this.state.selected}
                          choices={this.props.choices}
                          onSelect={this.onSelected} />
            );
        },
    });

    var playerChoices = [
        { name: 'Neutral', selectedName: 'Neutral', value: 0},
        { name: 'Player 1', selectedName: 'Player 1', value: 1},
        { name: 'Player 2', selectedName: 'Player 2', value: 2},
        { name: 'Player 3', selectedName: 'Player 3', value: 3},
        { name: 'Player 4', selectedName: 'Player 4', value: 4},
    ];

    var paintChoices = [
        { name: 'Single Point', selectedName: 'Single Point', value: 4},
        { name: 'Area, Dense', selectedName: 'Area, Dense', value: 1},
        { name: 'Area, Every Other', selectedName: 'Area, Every Other', value: 2},
    ];

    return React.createClass({
        mixins: [OverlayMixin, events.UpdateMixin, events.UpdateEditorMixin, events.ShowUpdateMixin, events.Command],

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

        onUpdateEditor: function(values) {
            //log(JSON.stringify(values));
            this.setState(values);
        },

        onCommand: function(command) {
            log(command);
            if (command === 'save-as') {
                this.refs.saveAs.getDOMNode().click();
            } else if (command === 'save') {
                this.refs.save.getDOMNode().click();
            } else if (command === 'load') {
                this.refs.load.getDOMNode().click();
            }
        },

        getInitialState: function() {
            var maps = [];

            return {
                MyPlayerNumber: 1,
                ShowAllPlayers: false,
                Maps:maps,

                isModalOpen: false,
            };
        },
        
        lerp: function(x1, y1, x2, y2, t)
        {
            var width = x2 - x1;
            var s = (t - x1) / width;

            return y2 * s + y1 * (1 - s);
        },

        setPlayer: function(item) {
            interop.setPlayer(item.value);
        },

        setPaintChoice: function(item) {
            interop.setPaintChoice(item.value);
        },

        handleToggle: function() {
            this.setState({
                isModalOpen: !this.state.isModalOpen
            });
        },

        doNothing: function() {

        },

        renderOverlay: function() {
            if (!this.state.isModalOpen) {
                return <span/>;
            }

            return (
                <Modal bsStyle='primary' onRequestHide={this.doNothing}>
                    <div className='modal-body'>
                        <h3>
                            Your map is being saved...
                        </h3>
                    </div>
                </Modal>
            );
        },

        save: function(map) {
            var _this = this;

            this.setState({
                isModalOpen: true,
            });

            if (interop.InXna()) {
                var wait = 1000;
            } else {
                // If we're not in-game then we will simulate a save time.
                var wait = 1000;
            }

            setTimeout(function() {
                interop.saveMap(map);

                _this.setState({
                    isModalOpen: false,
                });
            }, wait);
        },

        render: function() {
            var players = this.state.ShowAllPlayers ? _.range(1,5) : [this.state.MyPlayerNumber];

            var aspect = window.w / window.h;
            var xOffset = this.lerp(1.6, 2, 1, 5.6, aspect);

            var mapPicker = (
                <MapPicker
                    showPath
                    getMaps={interop.getMaps}
                    confirm='Load'
                    onConfirm={interop.loadMap}
                    directory=''
                  />
            );

            var saveAs = (
                <MapPicker
                    showPath saveAs
                    getMaps={interop.getMaps}
                    confirm='Save'
                    onConfirm={this.save}
                    directory='Custom'
                  />
            );

            return (
                <div>
                    <Div pos={pos(0,0)}>
                        {_.map(players, function(player, index) {
                            return <InGameUtil.UnitBar MyPlayerNumber={player} pos={pos(50.5,0.4 + index*4.2)} size={width(50)} />;
                        })}
                    </Div>
                                        
                    <InGameUtil.MenuButton pos={pos(0.5,0.4)} size={width(50)} />
                    <Div pos={pos(3.5,0.4)}>
                        <TopButton onClick={interop.toggleEditor} size={width(50)}
                            hotkey='P'
                            placement='right'
                            tooltip={this.state.EditorActive ? 'Return to editor' : 'Play test this map'} >
                            {this.state.EditorActive ? 'Play Test' : 'Edit'}
                        </TopButton>
                        <Gap width='0.2' />
                        <TopButton onClick={interop.createNewMap} tooltip='New Map' hotkey='Ctrl-N' size={width(50)}><Glyphicon glyph='new-window' /></TopButton>
                        <TopButton onClick={this.save} ref='save' tooltip='Save' hotkey='Ctrl-S' size={width(50)}><Glyphicon glyph='floppy-save' /></TopButton>

                        <ModalTrigger modal={saveAs} ref='saveAs'>
                            <TopButton tooltip='Save as...' hotkey='Shift-Ctrl-S' size={width(50)}><Glyphicon glyph='floppy-saved' /></TopButton>
                        </ModalTrigger>

                        <ModalTrigger modal={mapPicker} ref='load'>
                            <TopButton tooltip='Load...' hotkey='Ctrl-L' size={width(50)}><Glyphicon glyph='open' /></TopButton>
                        </ModalTrigger>

                        <Gap width='0.2' />
                        <MapName name={this.state.MapName} size={width(20)} />
                    </Div>

                    <Div pos={pos(15,0)}>
                        <Div nonBlocking pos={pos(0.42+xOffset,79)} size={width(50)} style={{'pointer-events':'auto', 'font-size': '1.4%;'}}>
                            <Choose onSelect={this.setPlayer} disabled={this.props.disabled} choices={playerChoices}
                                    value={this.state.MyPlayerNumber} {...this.props} />
                            <Gap width='1.53' />
                            <Choose onSelect={this.setPaintChoice} disabled={this.props.disabled} choices={paintChoices}
                                    value={this.state.UnitPlaceStyle} {...this.props} />
                        </Div>

                        <Div pos={pos(0+xOffset,85)}>
                            <ActionButton name='Dirt' tile={2} />
                            <ActionButton name='Grass' tile={1} />
                            <ActionButton name='Trees' tile={5} />
                            
                            <Gap width='2.33' />

                            <ActionButton name='Soldier' unit={1} />
                            <ActionButton name='DragonLord' unit={2} />
                            <ActionButton name='Necromancer' unit={3} />
                            <ActionButton name='Skeleton' unit={4} />
                            <ActionButton name='Terracotta' unit={5} />

                            <Gap width='2.33' />
                            
                            <ActionButton name='Barracks' />
                            <ActionButton name='GoldMine' />
                            <ActionButton name='JadeMine' />
                        </Div>
                        
                        <InGameUtil.UnitBox pos={pos(62.0,72.5)} size={width(20.62)} />
                    </Div>
                </div>
            );
        }
    });
});