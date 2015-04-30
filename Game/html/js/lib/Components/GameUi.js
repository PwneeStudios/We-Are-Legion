define(['lodash', 'react', 'interop', 'events',
        'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu', 'Components/InGameMenu', 'Components/OptionsMenu', 'Components/CreateGame'],
    function(_, React,interop, events,
            InGameUi, GameLobby, GameMenu, InGameMenu, OptionsMenu, CreateGame) {
 
    return React.createClass({
        mixins: [],

        getInitialState: function() {
            window.setScreen = this.setScreen;

            return {
                screen:'game-menu',
                //screen:'in-game-menu',
            };
        },

        setScreen: function(screen) {
            this.setState({
                screen:screen,
            });
        },

        render: function() {
            var body;

            switch (this.state.screen) {
                case 'game-menu': body = React.createElement(GameMenu, null); break;
                case 'options': body = React.createElement(OptionsMenu, null); break;
                case 'create-game': body = React.createElement(CreateGame, null); break;
                case 'find-game': body = React.createElement(FindGame, null); break;
                case 'game-lobby-host': body = React.createElement(GameLobby, {host: true, lobbyPlayerNum: 2}); break;
                case 'game-lobby': body = React.createElement(GameLobby, {lobbyPlayerNum: 2}); break;
                
                case 'in-game-ui': body = React.createElement(InGameUi, null); break;
                case 'in-game-menu': body = React.createElement(InGameMenu, null); break;
            }        

            return (
                React.createElement("div", null, 
                    body
                )
            );
        }
    });
});