define(['lodash', 'react', 'interop', 'events',
        'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu', 'Components/InGameMenu', 'Components/OptionsMenu', 'Components/CreateGame', 'Components/FindGame', 'Components/Manual', 'Components/GameOver'],
    function(_, React,interop, events,
            InGameUi, GameLobby, GameMenu, InGameMenu, OptionsMenu, CreateGame, FindGame, Manual, GameOver) {
 
    return React.createClass({
        mixins: [events.SetModeMixin],

        onSetMode: function(mode) {
            this.setMode(mode);
        },

        onSetScreen: function(screen) {
            this.setScreen(screen);
        },

        leaveGame: function() {
            if (interop.InXna()) {
                interop.xna().LeaveGame();
            } else {
                removeMode("in-game");
                removeMode("main-menu");

                setMode("main-menu");
                setScreen("game-menu");
            }
        },

        quitApp: function() {
            if (interop.InXna()) {
                interop.xna().QuitApp();
            } else {
                return;
            }
        },

        getInitialState: function() {
            window.leaveGame = this.leaveGame;
            window.quitApp = this.quitApp;

            window.setScreen = this.setScreen;
            window.setMode = this.setMode;
            window.back = this.back;
            window.refresh = this.refresh;
            window.removeMode = this.removeMode;
            
            window.modes = {};
            window.mode = null;
            window.screenHistory = null;

            return { };
        },

        componentDidMount: function() {
            setMode('none');
            //return;

            //setMode('main-menu');
            //setScreen('game-menu');
            //setScreen('options');
            //setScreen('game-lobby', {host:true});
            //setScreen('game-lobby', {host:false});
            //setScreen('manual');
            //setScreen('find-game');

            setMode('in-game');
            setScreen('victory');
            //setScreen('defeat');
            //setScreen('in-game-ui');
            //setScreen('in-game-menu');
        },

        refresh: function(e) {
            if (screenHistory.length > 0) {
                var prev = screenHistory.pop();
                this.setScreen(prev.screen, prev.params);
            }

            if (e) {
                e.preventDefault();
            }
        },

        back: function(e) {
            if (screenHistory.length > 0) {
                screenHistory.pop();

                var prev = screenHistory.pop();
                this.setScreen(prev.screen, prev.params);
            }

            if (e) {
                e.preventDefault();
            }
        },

        removeMode: function(mode) {
            _.remove(modes, function(_mode) { return _mode === mode; });
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

        setScreen: function(screen, params) {
            screenHistory.push({screen:screen,params:params});

            this.setState({
                screen:screen,
                params:params,
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
                case 'game-lobby': body = React.createElement(GameLobby, null); break;
                
                case 'in-game-ui': body = React.createElement(InGameUi, null); break;
                case 'victory': body = React.createElement(GameOver, {victory: true}); break;
                case 'defeat': body = React.createElement(GameOver, {victory: false}); break;
                case 'in-game-menu': body = React.createElement(InGameMenu, null); break;
            }

            if (body) {
                body.props.params = this.state.params;
            }

            return (
                React.createElement("div", null, 
                    body
                )
            );
        }
    });
});