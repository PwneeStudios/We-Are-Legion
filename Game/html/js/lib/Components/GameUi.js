define(['lodash', 'react', 'interop', 'events', 'Components/InGameUi', 'Components/GameLobby'], function(_, React,interop, events, InGameUi, GameLobby) {
 
    return React.createClass({
        mixins: [],

        getInitialState: function() {
            return {
                screen:'game',
            };
        },

        render: function() {
            var body;
            body = React.createElement(InGameUi, null);
            //body = <GameLobby />;

            return (
                React.createElement("div", null, 
                    body
                )
            );
        }
    });
});