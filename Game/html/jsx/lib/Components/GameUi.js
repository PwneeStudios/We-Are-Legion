define(['lodash', 'react', 'interop', 'events',
        'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu', 'Components/InGameMenu', 'Components/OptionsMenu', 'Components/CreateGame', 'Components/FindGame', 'Components/Manual',
        'Components/GameOver', 'Components/GamePaused', 'Components/ConfirmLeaveGame', 'Components/Disconnected', 'Components/Waiting', 'Components/DisconnectedFromLobby', 'Components/Failed', 'Components/Desync',
        'Components/EditorUi'],
    function(_, React, interop, events,
            InGameUi, GameLobby, GameMenu, InGameMenu, OptionsMenu, CreateGame, FindGame, Manual,
            GameOver, GamePaused, ConfirmLeaveGame, Disconnected, Waiting, DisconnectedFromLobby, Failed, Desync,
            EditorUi) {
 
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
                interop.leaveGame();
            } else {
                removeMode("in-game");
                removeMode("main-menu");

                setMode("main-menu");
                setScreen("game-menu");
            }
        },

        quitApp: function() {
            if (interop.InXna()) {
                interop.quitApp();
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
            window.canGoBack = this.canGoBack;
            window.allowBack = this.allowBack;
            window.preventBack = this.preventBack;
            window.refresh = this.refresh;
            window.removeMode = this.removeMode;

            document.body.onkeydown = this.onKeyDown;

            window.modes = {};
            window.mode = null;

            return { };
        },

        onKeyDown: function(e) {
            // When the user presses the escape key...
            if (e.keyCode == 27) {
                if (canGoBack() && window.onEscape) {
                    window.onEscape();
                }
            }
        },

        dumpState: function() {
            if (interop.InXna()) {
                var state = {
                    mode:window.mode,
                    modes:window.modes,
                };

                var dump = JSON.stringify(state);
                interop.dumpState(dump);
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

            setMode('main-menu');
            setScreen('game-menu');

            // Test menus
            //setScreen('options');
            //setScreen('game-lobby', {host:true});
            //setScreen('game-lobby', {host:false});
            //setScreen('manual');
            //setScreen('find-game');

            // Test in-game
            //setMode('in-game');
            //setScreen('game-over', {victory:false});
            //setScreen('game-over', {victory:true});
            //setScreen('in-game-ui');
            //setScreen('editor-ui');
            //setScreen('in-game-menu');
            //setScreen('confirm-leave-game');
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

                prev.params.fromRefresh = true;
                this.setScreen(prev.screen, prev.params);
            }

            if (e) {
                e.preventDefault();
            }
        },

        allowBack: function() {
            this.backEnabled = true;
        },

        preventBack: function() {
            this.backEnabled = false;
        },

        canGoBack: function() {
            return this.backEnabled && this.screenHistory().length > 1;
        },

        back: function(e) {
            if (this.screenHistory().length > 1) {
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

            log('setting mode to ' + newMode);

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

            log('setting screen to ' + screen);
        },

        render: function() {
            var body = null;

            log('render screen ' + this.state.screen + ' ' + JSON.stringify(this.state.params));

            switch (this.state.screen) {
                case 'game-menu': body = <GameMenu />; break;
                case 'options': body = <OptionsMenu />; break;
                case 'manual': body = <Manual />; break;
                case 'create-game': body = <CreateGame />; break;
                case 'find-game': body = <FindGame />; break;
                case 'game-lobby': body = <GameLobby />; break;
                
                case 'in-game-ui': body = <InGameUi />; break;
                case 'editor-ui': body = <EditorUi />; break;
                case 'game-over': body = <GameOver />; break;
                case 'in-game-menu': body = <InGameMenu />; break;
                case 'game-paused': body = <GamePaused />; break;
                case 'confirm-leave-game': body = <ConfirmLeaveGame />; break;
                case 'disconnected': body = <Disconnected />; break;
                case 'disconnected-from-lobby': body = <DisconnectedFromLobby />; break;
                case 'failed': body = <Failed />; break;
                case 'waiting': body = <Waiting />; break;
                case 'desync': body = <Desync />; break;

                case 'none': body = null; break;
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