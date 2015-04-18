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
            body = <InGameUi />;
            //body = <GameLobby />;

            return (
                <div>
                    {body}
                </div>
            );
        }
    });
});