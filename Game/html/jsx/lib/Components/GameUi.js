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
            
            window.dumpState = this.dumpState;
            window.restoreState = this.restoreState;

            window.setScreen = this.setScreen;
            window.setMode = this.setMode;
            window.back = this.back;
            window.refresh = this.refresh;
            window.removeMode = this.removeMode;

            window.modes = {};
            window.mode = null;

            return { };
        },

        dumpState: function() {
            if (interop.InXna()) {
                var state = {
                    mode:window.mode,
                    modes:window.modes,
                };

                var dump = JSON.stringify(state);
                interop.xna().DumpState(dump);
            }
        },

        restoreState: function(state) {
            if (interop.InXna()) {
                var _state = JSON.parse(state);

                window.mode = _state.mode;
                window.modes = _state.modes;

                window.refresh();
            }
        },

        componentDidMount: function() {
            setMode('none');
            //return;

            setMode('main-menu');
            setScreen('game-menu');
            //setScreen('options');
            //setScreen('game-lobby', {host:true});
            //setScreen('game-lobby', {host:false});
            //setScreen('manual');
            //setScreen('find-game');

            //setMode('in-game');
            //setScreen('gameOver', {victory:false});
            //setScreen('gameOver', {victory:true});
            //setScreen('in-game-ui');
            //setScreen('in-game-menu');
        },

        screenHistory: function() {
            if (mode) {
                return modes[mode];
            } else {
                return [];
            }
        },

        refresh: function(e) {
            if (this.screenHistory().length > 0) {
                var prev = this.screenHistory().pop();

                this.setScreen(prev.screen, prev.params);
            }

            if (e) {
                e.preventDefault();
            }
        },

        back: function(e) {
            if (this.screenHistory().length > 0) {
                this.screenHistory().pop();

                var prev = this.screenHistory().pop();
                this.setScreen(prev.screen, prev.params);
            }

            if (e) {
                e.preventDefault();
            }
        },

        removeMode: function(mode) {
            if (mode in modes) {
                modes[mode] = [];
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

            this.refresh();
        },

        setScreen: function(screen, params) {
            if (typeof params === 'undefined') {
                params = { };
            }

            this.screenHistory().push({screen:screen,params:params});

            this.setState({
                screen:screen,
                params:params,
            });
        },

        render: function() {
            var body = null;

            console.log('render screen ' + this.state.screen + ' ' + JSON.stringify(this.state.params));

            switch (this.state.screen) {
                case 'game-menu': body = <GameMenu />; break;
                case 'options': body = <OptionsMenu />; break;
                case 'manual': body = <Manual />; break;
                case 'create-game': body = <CreateGame />; break;
                case 'find-game': body = <FindGame />; break;
                case 'game-lobby': body = <GameLobby />; break;
                
                case 'in-game-ui': body = <InGameUi />; break;
                case 'gameOver': body = <GameOver />; break;
                case 'in-game-menu': body = <InGameMenu />; break;
            }

            if (body) {
                body.props.params = this.state.params;
            }

            return (
                <div>
                    {body}
                </div>
            );
        }
    });
});