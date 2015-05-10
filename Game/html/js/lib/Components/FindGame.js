define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    var ListGroup = ReactBootstrap.ListGroup;
    var ListGroupItem = ReactBootstrap.ListGroupItem;
    
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

    var GameItem = React.createClass({displayName: "GameItem",
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        onClick: function() {
            setScreen('game-lobby', {host:false});
        },

        render: function() {
            return (
                React.createElement("tr", null, 
                    React.createElement("td", null, this.props.hostName), 
                    React.createElement("td", null, this.props.mapName), 
                    React.createElement("td", null, this.props.players), 
                    React.createElement("td", null, 
                        React.createElement(Button, {onClick: this.onClick}, 
                            "Join"
                        )
                    )
                )
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
            var _this = this;

            var visibility = [
                {name:'Public games', value:'public'},
                {name:'Friend games', value:'friend'},
            ];

            return (
                React.createElement("div", null, 
                    React.createElement(Div, {nonBlocking: true, pos: pos(10,5), size: width(80)}, 
                        React.createElement(Panel, null, 
                            React.createElement("h2", null, 
                                "Game list"
                            )
                        ), 

                        React.createElement(Well, {style: {'height':'75%'}}, 

                            /* Game List */
                            React.createElement(Div, {className: "game-list", pos: pos(3.3,16.9), size: size(50,66.2), 
                                 style: {'overflow-y':'scroll','pointer-events':'auto','font-size': '1.4%'}}, 
                                React.createElement(Table, {style: {width:'100%','pointer-events':'auto'}}, React.createElement("tbody", null, 
                                    _.map(_.range(1, 50), function(i) {
                                        return React.createElement(GameItem, {hostName: "cookin ash", mapName: "beset", players: "2/4"});
                                    })
                                ))
                            ), 

                            /* Game visibility type */
                            React.createElement(Div, {pos: pos(55.3,16.9), size: size(30,66.2)}, 
                                React.createElement(OptionList, {options: visibility})
                            ), 

                            /* Buttons */
                            React.createElement(Div, {nonBlocking: true, pos: pos(38,80), size: width(60)}, 
                                React.createElement("div", {style: {'float':'right', 'pointer-events':'auto'}}, 
                                    React.createElement("p", null, 
                                        React.createElement(Button, {onClick: back}, "Back")
                                    )
                                )
                            )

                        )
                    )
                )
            );
        }
    });
}); 