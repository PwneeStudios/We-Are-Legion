define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, sound, React, ReactBootstrap, interop, events, ui, Chat) {
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
        mixin: [],

        getInitialState: function() {
            return {
            };
        },
        
        onClick: function() {
            setScreen('game-lobby', {
                host: false,
                lobbyIndex: this.props.data.Index,
            });
        },

        render: function() {
            return (
                React.createElement("tr", null, 
                    React.createElement("td", null, this.props.data.Name), 
                    React.createElement("td", null, this.props.data.MemberCount, " / ", this.props.data.Capacity), 
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
        mixins: [events.FindLobbiesMixin],

        onFindLobbies: function(values) {
            console.log('found lobbies');

            this.setState({
                loading: false,
                lobbies: values.Lobbies,
                online: values.Online,
            });
        },

        loadingState: {
            loading: true,
            lobbies: [],
        },

        findLobbies: function(friends) {
            interop.findLobbies(friends);

            this.setState(this.loadingState);
        },
                
        getInitialState: function() {
            interop.findLobbies(false);

            return this.loadingState;
        },

        onVisibilityChange: function(item) {
            var friends = item.value === 'friends';

            this.findLobbies(friends);
        },
        
        render: function() {
            var _this = this;

            var visibility = [
                {name:'Public games', value:'public'},
                {name:'Friend games', value:'friends'},
            ];

            var view = null;

            if (this.state.loading) {
                view = 'Loading...';
            } if (!this.state.online) {
                view = "Offline. Can't find lobbies.";
            } else {
                view = (
                    React.createElement(Table, {style: {width:'100%','pointer-events':'auto'}}, React.createElement("tbody", null, 
                        _.map(this.state.lobbies, function(lobby) {
                            return React.createElement(GameItem, {data: lobby});
                        })
                    ))
                );
            }

            return (
                React.createElement("div", null, 
                    React.createElement(Div, {nonBlocking: true, pos: pos(10,5), size: width(80)}, 
                        React.createElement(Well, {style: {'height':'90%'}}, 
                            /* Header */
                            React.createElement("h2", null, "Game list"), 

                            /* Game List */
                            React.createElement(Div, {className: "game-list", pos: pos(3.3,16.9), size: size(50,66.2), 
                                 style: {'overflow-y':'scroll','pointer-events':'auto','font-size': '1.4%'}}, 
                                 view
                            ), 

                            /* Game visibility type */
                            React.createElement(Div, {pos: pos(55.3,16.9), size: size(30,66.2)}, 
                                React.createElement(OptionList, {options: visibility, onSelect: this.onVisibilityChange})
                            ), 

                            /* Buttons */
                            React.createElement(Div, {nonBlocking: true, pos: pos(38,80), size: width(60)}, 
                                React.createElement("div", {style: {'float':'right', 'pointer-events':'auto'}}, 
                                    React.createElement("p", null, 
                                        React.createElement(ui.BackButton, null)
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