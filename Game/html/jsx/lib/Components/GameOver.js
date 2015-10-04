define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'sound',
        'Components/Chat', 'Components/MapPicker'],
function(_, React, ReactBootstrap, interop, events, ui, sound,
         Chat, MapPicker) {

    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    var ListGroup = ReactBootstrap.ListGroup;
    var ListGroupItem = ReactBootstrap.ListGroupItem;
    var ModalTrigger = ReactBootstrap.ModalTrigger;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var OptionList = ui.OptionList;
    var RenderAtMixin = ui.RenderAtMixin;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var UnitBar = React.createClass({
        mixins: [RenderAtMixin],

        item: function(image, scale, data) {
            return (
                <td>
                    <UiImage width={24.2*scale} image={image} />
                    <p style={{paddingLeft:'5%', 'pointer-events': 'none'}}>
                        &nbsp;
                        {data}
                    </p>
                </td>
            );
        },
        
        renderAt: function() {
            var Images = playerImages[this.props.MyPlayerNumber];
            var Buildings = Images.Buildings;
            var Units = Images.Units;

            var name = this.props.info.Name || 'Player ' + this.props.MyPlayerNumber;
        
            return (
                <tr>
                    <td><p style={{'font-family':'Verdana, Geneva, sans-serif'}}>{name}</p></td>
                    {this.item(Buildings.Barracks, 1.2,  this.props.info.Barracks.Count)}
                    {this.item(Units.Soldier,      0.85, this.props.info.Units)}
                    {this.item(Buildings.GoldMine, 2,    this.props.info.GoldMine.Count)}
                    {this.item(GoldImage,          0.5,  this.props.info.Gold)}
                    {this.item(Buildings.JadeMine, 2,    this.props.info.JadeMine.Count)}
                    {this.item(JadeImage,          0.5,  this.props.info.Jade)}
                </tr>
            );
        },
    });

    return React.createClass({
        mixins: [],

        show: function() {
            sound.play.slam();

            this.setState({
                show: true,
            });
        },

        getInitialState: function() {
            if (interop.InXna()) {
                setTimeout(this.show, 4700);

                return {
                    show: false,
                };
            } else {
                this.props.params = {"victory":true,"winningTeam":2,"info":[null,{"Name":"Player 1","Number":1,"GoldMine":{"Count":0,"Bought":0},"JadeMine":{"Count":0,"Bought":0},"Barracks":{"Count":100,"Bought":0},"Gold":7500,"Jade":10000,"Units":100,"DragonLords":1,"SpellCasts":{"Fireball":0,"Skeletons":0,"Necromancer":0,"Terracotta":0},"SpellCosts":{"Fireball":1000,"Skeletons":1000,"Necromancer":1000,"Terracotta":1000},"DragonLordAlive":true,"Params":{"Buildings":{"Barracks":{"GoldCost":250,"CostIncrease":50,"GoldPerTick":0,"JadePerTick":0,"CurrentGoldCost":250,"UnitType":0.0235294122,"Name":"Barracks"},"GoldMine":{"GoldCost":500,"CostIncrease":100,"GoldPerTick":3,"JadePerTick":0,"CurrentGoldCost":500,"UnitType":0.02745098,"Name":"GoldMine"},"JadeMine":{"GoldCost":1000,"CostIncrease":200,"GoldPerTick":0,"JadePerTick":3,"CurrentGoldCost":1000,"UnitType":0.03137255,"Name":"JadeMine"}},"Barracks":{"GoldCost":250,"CostIncrease":50,"GoldPerTick":0,"JadePerTick":0,"CurrentGoldCost":250,"UnitType":0.0235294122,"Name":"Barracks"},"GoldMine":{"GoldCost":500,"CostIncrease":100,"GoldPerTick":3,"JadePerTick":0,"CurrentGoldCost":500,"UnitType":0.02745098,"Name":"GoldMine"},"JadeMine":{"GoldCost":1000,"CostIncrease":200,"GoldPerTick":0,"JadePerTick":3,"CurrentGoldCost":1000,"UnitType":0.03137255,"Name":"JadeMine"},"StartGold":750,"StartJade":10000}},{"Name":"Player 2","Number":2,"GoldMine":{"Count":0,"Bought":0},"JadeMine":{"Count":0,"Bought":0},"Barracks":{"Count":187,"Bought":0},"Gold":7500,"Jade":10000,"Units":187,"DragonLords":2,"SpellCasts":{"Fireball":0,"Skeletons":0,"Necromancer":0,"Terracotta":0},"SpellCosts":{"Fireball":0,"Skeletons":0,"Necromancer":0,"Terracotta":0},"DragonLordAlive":true,"Params":{"Buildings":{"Barracks":{"GoldCost":250,"CostIncrease":50,"GoldPerTick":0,"JadePerTick":0,"CurrentGoldCost":250,"UnitType":0.0235294122,"Name":"Barracks"},"GoldMine":{"GoldCost":500,"CostIncrease":100,"GoldPerTick":3,"JadePerTick":0,"CurrentGoldCost":500,"UnitType":0.02745098,"Name":"GoldMine"},"JadeMine":{"GoldCost":1000,"CostIncrease":200,"GoldPerTick":0,"JadePerTick":3,"CurrentGoldCost":1000,"UnitType":0.03137255,"Name":"JadeMine"}},"Barracks":{"GoldCost":250,"CostIncrease":50,"GoldPerTick":0,"JadePerTick":0,"CurrentGoldCost":250,"UnitType":0.0235294122,"Name":"Barracks"},"GoldMine":{"GoldCost":500,"CostIncrease":100,"GoldPerTick":3,"JadePerTick":0,"CurrentGoldCost":500,"UnitType":0.02745098,"Name":"GoldMine"},"JadeMine":{"GoldCost":1000,"CostIncrease":200,"GoldPerTick":0,"JadePerTick":3,"CurrentGoldCost":1000,"UnitType":0.03137255,"Name":"JadeMine"},"StartGold":750,"StartJade":10000}},{"Name":"Player 3","Number":3,"GoldMine":{"Count":0,"Bought":0},"JadeMine":{"Count":0,"Bought":0},"Barracks":{"Count":0,"Bought":0},"Gold":7500,"Jade":10000,"Units":0,"DragonLords":0,"SpellCasts":{"Fireball":0,"Skeletons":0,"Necromancer":0,"Terracotta":0},"SpellCosts":{"Fireball":0,"Skeletons":0,"Necromancer":0,"Terracotta":0},"DragonLordAlive":false,"Params":{"Buildings":{"Barracks":{"GoldCost":250,"CostIncrease":50,"GoldPerTick":0,"JadePerTick":0,"CurrentGoldCost":250,"UnitType":0.0235294122,"Name":"Barracks"},"GoldMine":{"GoldCost":500,"CostIncrease":100,"GoldPerTick":3,"JadePerTick":0,"CurrentGoldCost":500,"UnitType":0.02745098,"Name":"GoldMine"},"JadeMine":{"GoldCost":1000,"CostIncrease":200,"GoldPerTick":0,"JadePerTick":3,"CurrentGoldCost":1000,"UnitType":0.03137255,"Name":"JadeMine"}},"Barracks":{"GoldCost":250,"CostIncrease":50,"GoldPerTick":0,"JadePerTick":0,"CurrentGoldCost":250,"UnitType":0.0235294122,"Name":"Barracks"},"GoldMine":{"GoldCost":500,"CostIncrease":100,"GoldPerTick":3,"JadePerTick":0,"CurrentGoldCost":500,"UnitType":0.02745098,"Name":"GoldMine"},"JadeMine":{"GoldCost":1000,"CostIncrease":200,"GoldPerTick":0,"JadePerTick":3,"CurrentGoldCost":1000,"UnitType":0.03137255,"Name":"JadeMine"},"StartGold":750,"StartJade":10000}},{"Name":"Player 4","Number":4,"GoldMine":{"Count":0,"Bought":0},"JadeMine":{"Count":0,"Bought":0},"Barracks":{"Count":0,"Bought":0},"Gold":7500,"Jade":10000,"Units":0,"DragonLords":0,"SpellCasts":{"Fireball":0,"Skeletons":0,"Necromancer":0,"Terracotta":0},"SpellCosts":{"Fireball":0,"Skeletons":0,"Necromancer":0,"Terracotta":0},"DragonLordAlive":false,"Params":{"Buildings":{"Barracks":{"GoldCost":250,"CostIncrease":50,"GoldPerTick":0,"JadePerTick":0,"CurrentGoldCost":250,"UnitType":0.0235294122,"Name":"Barracks"},"GoldMine":{"GoldCost":500,"CostIncrease":100,"GoldPerTick":3,"JadePerTick":0,"CurrentGoldCost":500,"UnitType":0.02745098,"Name":"GoldMine"},"JadeMine":{"GoldCost":1000,"CostIncrease":200,"GoldPerTick":0,"JadePerTick":3,"CurrentGoldCost":1000,"UnitType":0.03137255,"Name":"JadeMine"}},"Barracks":{"GoldCost":250,"CostIncrease":50,"GoldPerTick":0,"JadePerTick":0,"CurrentGoldCost":250,"UnitType":0.0235294122,"Name":"Barracks"},"GoldMine":{"GoldCost":500,"CostIncrease":100,"GoldPerTick":3,"JadePerTick":0,"CurrentGoldCost":500,"UnitType":0.02745098,"Name":"GoldMine"},"JadeMine":{"GoldCost":1000,"CostIncrease":200,"GoldPerTick":0,"JadePerTick":3,"CurrentGoldCost":1000,"UnitType":0.03137255,"Name":"JadeMine"},"StartGold":750,"StartJade":10000}}]};

                return {
                    show: true,
                };
            }
        },

        render: function() {
            var _this = this;

            if (!this.state.show) {
                return (
                    <div>
                    </div>
                );
            }

            if (this.props.params.spectator) {
                var message = 'Match Over';
            } else {
                var message = this.props.params.victory ? 'Victory!' : 'Defeat!';
            }

            var players = _.range(1,5);

            return (
                <div>
                    <Div nonBlocking pos={pos(10,10)} size={width(80)}>
                        <Well style={{'height':'80%'}}>
                            <Div pos={pos(5,2)} size={width(90)}>
                                <h1 style={{float:'left',fontSize:52}}>
                                    {message}
                                </h1>

                                <h1 style={{float:'right',fontSize:30}}>
                                    <span style={{float:'right'}}>Team {this.props.params.winningTeam} wins!</span>
                                </h1>
                            </Div>

                            {/* Info */}
                            <Div pos={pos(5,20)}>
                                <Table style={{width:'90%'}}><tbody>
                                {_.map(players, function(player, index) {
                                    return (
                                        <UnitBar MyPlayerNumber={player} info={_this.props.params.info[player]} />
                                    );
                                })}
                                </tbody></Table>
                            </Div>

                            {/* Buttons */}
                            <Div nonBlocking pos={pos(36,72)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        <Button onClick={interop.returnToLobby}>Return to Lobby</Button>
                                    </p>
                                </div>
                            </Div>
                        </Well>
                    </Div>
                </div>
            );
        }
    });
}); 