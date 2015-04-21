define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var RenderAtMixin = ui.RenderAtMixin;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var makeName = function(english, chinese) {
        return (
            <span>
                {english}
                <span style={{'text-align':'right', 'float':'right'}}>{chinese}</span>
            </span>
        );
    };

    var ChooseKingdom = React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            var choices = [
                {name: makeName('Kingdom of Wei',   '魏'), value:1, },
                {name: makeName('Kingdom of Shu',   '蜀'), value:3, },
                {name: makeName('Kingdom of Wu',    '吳'), value:4, },
                {name: makeName('Kingdom of Beast', '獸'), value:2, },
            ];

            return (
                <Dropdown value='Kingdom' choices={choices} />
            );
        },
    });

    var ChooseTeam = React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            var choices = [
                {name: makeName('Team 1', '一'), value:1, },
                {name: makeName('Team 2', '二'), value:3, },
                {name: makeName('Team 3', '三'), value:4, },
                {name: makeName('Team 4', '四'), value:2, },
            ];

            return (
                <Dropdown value='Team' choices={choices} />
            );
        },
    });

    var PlayerEntry = React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            return (
                <tr>
                    <td>Player {this.props.player}</td>
                    <td><ChooseTeam /></td>
                    <td><ChooseKingdom /></td>
                </tr>
            );
        },
    });

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        render: function() {
            return (
                <div>
                    <Div nonBlocking pos={pos(10,5)} size={width(80)}>
                        <Panel>
                            <h2>
                                Game Lobby
                            </h2>
                        </Panel>

                        <Well style={{'height':'75%'}}>
                            <Chat.ChatBox show full pos={pos(2, 17)} size={size(43,61)}/>
                            <Chat.ChatInput show pos={pos(2,80)} size={width(43)} />

                            <Div nonBlocking pos={pos(48,16.9)} size={width(50)} style={{'pointer-events':'auto', 'font-size': '1.4%;'}}>
                                <Table style={{width:'100%'}}><tbody>
                                    {/*<tr style={{'background-color':'#1c1e22'}}>
                                        <th></th>
                                        <th>國</th>
                                        <th>隊</th>
                                    </tr>*/}

                                    {_.map(_.range(1, 5), function(i) { return <PlayerEntry player={i} />; })}
                                </tbody></Table>
                            </Div>

                            <Div nonBlocking pos={pos(38,68)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        <Button>Choose map...</Button>
                                    </p>
                                </div>
                            </Div>

                            <Div nonBlocking pos={pos(38,80)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        <Button>Start Game</Button>
                                        &nbsp;
                                        <Button>Leave Lobby</Button>
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