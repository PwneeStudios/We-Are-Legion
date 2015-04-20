define(['lodash', 'react', 'interop', 'events', 'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu'], function(_, React,interop, events, InGameUi, GameLobby, GameMenu) {
 
    return React.createClass({
        mixins: [],

        getInitialState: function() {
            return {
                screen:'game',
            };
        },

        render: function() {
            var body;
            //body = <InGameUi />;
            //body = <GameLobby />;
            body = <GameMenu />;

            return (
                <div>
                    {body}
                </div>
            );
        }
    });
});