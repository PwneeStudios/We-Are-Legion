define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui',
        'Components/Chat', 'Components/MapPicker'],
function(_, React, ReactBootstrap, interop, events, ui,
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

    return React.createClass({
        mixins: [],

        getInitialState: function() {
            return {
            };
        },

        render: function() {
            var _this = this;

            return (
                React.createElement("div", null, 
                    React.createElement(Div, {nonBlocking: true, pos: pos(10,10), size: width(80)}, 
                        React.createElement(Well, {style: {'height':'80%'}}, 
                            React.createElement("h1", null, 
                                this.props.params.victory ? 'Victory!' : 'Defeat!'
                            ), 

                            this.props.winningTeam ?
                            React.createElement("h2", null, 
                                 "Team ", this.props.params.winningTeam, " wins!"
                            )
                            : null, 

                            /* Buttons */
                            React.createElement(Div, {nonBlocking: true, pos: pos(36,72), size: width(60)}, 
                                React.createElement("div", {style: {'float':'right', 'pointer-events':'auto'}}, 
                                    React.createElement("p", null, 
                                        React.createElement(Button, {onClick: leaveGame}, "Leave Game")
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