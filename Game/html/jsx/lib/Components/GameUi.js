define(['lodash', 'react', 'interop', 'events',
        'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu', 'Components/InGameMenu', 'Components/OptionsMenu', 'Components/CreateGame'],
    function(_, React,interop, events,
            InGameUi, GameLobby, GameMenu, InGameMenu, OptionsMenu, CreateGame) {
 
    return React.createClass({
        mixins: [],

        getInitialState: function() {
            window.setScreen = this.setScreen;
            window.back = this.back;
            window.screenHistory = [];

            return { };
        },

        componentDidMount: function() {
            setScreen('game-menu');
        },

        back: function(e) {
            screenHistory.pop();
            this.setScreen(screenHistory.pop());

            if (e) {
                e.preventDefault();
            }
        },

        setScreen: function(screen) {
            screenHistory.push(screen);

            this.setState({
                screen:screen,
            });
        },

        render: function() {
            var body;

            switch (this.state.screen) {
                case 'game-menu': body = <GameMenu />; break;
                case 'options': body = <OptionsMenu />; break;
                case 'create-game': body = <CreateGame />; break;
                case 'find-game': body = <FindGame />; break;
                case 'game-lobby-host': body = <GameLobby host lobbyPlayerNum={2} />; break;
                case 'game-lobby': body = <GameLobby lobbyPlayerNum={2} />; break;
                
                case 'in-game-ui': body = <InGameUi />; break;
                case 'in-game-menu': body = <InGameMenu />; break;
            }        

            return (
                <div>
                    {body}
                </div>
            );
        }
    });
});