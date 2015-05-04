define(['lodash', 'react', 'interop', 'events',
        'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu', 'Components/InGameMenu', 'Components/OptionsMenu', 'Components/CreateGame', 'Components/FindGame', 'Components/Manual'],
    function(_, React,interop, events,
            InGameUi, GameLobby, GameMenu, InGameMenu, OptionsMenu, CreateGame, FindGame, Manual) {
 
    return React.createClass({
        mixins: [events.SetModeMixin],

        onSetMode: function(mode) {
            console.log('hello');
            this.setMode(mode);
        },

        onSetScreen: function(screen) {
            this.setScreen(screen);
        },

        getInitialState: function() {
            window.setScreen = this.setScreen;
            window.setMode = this.setMode;
            window.back = this.back;
            window.refresh = this.refresh;
            
            window.modes = {};
            window.mode = null;
            window.screenHistory = null;

            return { };
        },

        componentDidMount: function() {
            setMode('none');
            //return;

            setMode('main-menu');
            setScreen('game-menu');
            //setScreen('options');
            //setScreen('game-lobby');
            setScreen('manual');

            //setMode('in-game');
            //setScreen('in-game-ui');
            //setScreen('in-game-menu');
        },

        refresh: function(e) {
            if (screenHistory.length > 0) {
                this.setScreen(screenHistory.pop());                
            }

            if (e) {
                e.preventDefault();
            }
        },

        back: function(e) {
            if (screenHistory.length > 0) {
                screenHistory.pop();
                this.setScreen(screenHistory.pop());                
            }

            if (e) {
                e.preventDefault();
            }
        },

        setMode: function(newMode) {
            if (mode === newMode) {
                return;
            }

            mode = newMode;
            if (!(mode in modes)) {
                modes[mode] = [];
            }

            screenHistory = modes[mode];
            this.refresh();
        },

        setScreen: function(screen) {
            screenHistory.push(screen);

            this.setState({
                screen:screen,
            });
        },

        render: function() {
            var body = null;

            switch (this.state.screen) {
                case 'game-menu': body = React.createElement(GameMenu, null); break;
                case 'options': body = React.createElement(OptionsMenu, null); break;
                case 'manual': body = React.createElement(Manual, null); break;
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