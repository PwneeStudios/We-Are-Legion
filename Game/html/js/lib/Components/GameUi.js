define(['lodash', 'react', 'interop', 'events',
        'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu', 'Components/InGameMenu', 'Components/OptionsMenu'],
    function(_, React,interop, events,
            InGameUi, GameLobby, GameMenu, InGameMenu, OptionsMenu) {
 
    return React.createClass({
        mixins: [],

        getInitialState: function() {
            return {
                screen:'game',
            };
        },

        render: function() {
            var body;
            //body = <GameMenu />;
            body = React.createElement(OptionsMenu, null);
            //( )body = <CreateGame />;
            //( )body = <FindGame />;
            //body = <GameLobby />;
            //body = <InGameUi />;
            //body = <InGameMenu />;

            return (
                React.createElement("div", null, 
                    body
                )
            );
        }
    });
});