define(['lodash', 'react', 'interop', 'events',
        'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu', 'Components/InGameMenu', 'Components/OptionsMenu', 'Components/CreateGame'],
    function(_, React,interop, events,
            InGameUi, GameLobby, GameMenu, InGameMenu, OptionsMenu, CreateGame) {
 
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
            //body = <OptionsMenu />;
            body = <CreateGame />;
            //( )body = <FindGame />;
            //body = <GameLobby host lobbyPlayerNum={2} />;
            //body = <GameLobby lobbyPlayerNum={2} />;
            //body = <InGameUi />;
            //body = <InGameMenu />;

            return (
                <div>
                    {body}
                </div>
            );
        }
    });
});