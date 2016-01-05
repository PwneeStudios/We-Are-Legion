'use strict';

define(['lodash', 'react', 'interop', 'events', 'Components/InGameUi', 'Components/GameLobby', 'Components/GameMenu', 'Components/InGameMenu', 'Components/OptionsMenu', 'Components/CreateGame', 'Components/FindGame', 'Components/Manual', 'Components/GameOver', 'Components/GamePaused', 'Components/ConfirmLeaveGame', 'Components/Disconnected', 'Components/Waiting', 'Components/DisconnectedFromLobby', 'Components/Failed', 'Components/Desync', 'Components/EditorUi'], function (_, React, interop, events, InGameUi, GameLobby, GameMenu, InGameMenu, OptionsMenu, CreateGame, FindGame, Manual, GameOver, GamePaused, ConfirmLeaveGame, Disconnected, Waiting, DisconnectedFromLobby, Failed, Desync, EditorUi) {

    return React.createClass({
        mixins: [events.SetModeMixin],

        onSetMode: function onSetMode(mode) {
            this.setMode(mode);
        },

        onSetScreen: function onSetScreen(screen) {
            this.setScreen(screen);
        },

        leaveGame: function leaveGame() {
            if (interop.InXna()) {
                interop.leaveGame();
            } else {
                removeMode("in-game");
                removeMode("main-menu");

                setMode("main-menu");
                setScreen("game-menu");
            }
        },

        quitApp: function quitApp() {
            if (interop.InXna()) {
                interop.quitApp();
            } else {
                return;
            }
        },

        getInitialState: function getInitialState() {
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

            return {};
        },

        onKeyDown: function onKeyDown(e) {
            // When the user presses the escape key...
            if (e.keyCode == 27) {
                if (canGoBack() && window.onEscape) {
                    window.onEscape();
                }
            }
        },

        dumpState: function dumpState() {
            if (interop.InXna()) {
                var state = {
                    mode: window.mode,
                    modes: window.modes
                };

                var dump = JSON.stringify(state);
                interop.dumpState(dump);
            }
        },

        restoreState: function restoreState(state) {
            if (interop.InXna()) {
                var _state = JSON.parse(state);

                window.mode = _state.mode;
                window.modes = _state.modes;

                window.refresh();
            }
        },

        componentDidMount: function componentDidMount() {
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

        screenHistory: function screenHistory() {
            if (mode) {
                return modes[mode];
            } else {
                return [];
            }
        },

        refresh: function refresh(e) {
            if (this.screenHistory().length > 0) {
                var prev = this.screenHistory().pop();

                prev.params.fromRefresh = true;
                this.setScreen(prev.screen, prev.params);
            }

            if (e) {
                e.preventDefault();
            }
        },

        allowBack: function allowBack() {
            this.backEnabled = true;
        },

        preventBack: function preventBack() {
            this.backEnabled = false;
        },

        canGoBack: function canGoBack() {
            return this.backEnabled && this.screenHistory().length > 1;
        },

        back: function back(e) {
            if (this.screenHistory().length > 1) {
                this.screenHistory().pop();

                var prev = this.screenHistory().pop();
                this.setScreen(prev.screen, prev.params);
            }

            if (e) {
                e.preventDefault();
            }
        },

        removeMode: function removeMode(mode) {
            if (mode in modes) {
                modes[mode] = [];
            }
        },

        setMode: function setMode(newMode) {
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

        setScreen: function setScreen(screen, params) {
            if (typeof params === 'undefined') {
                params = {};
            }

            this.screenHistory().push({ screen: screen, params: params });

            this.setState({
                screen: screen,
                params: params
            });

            log('setting screen to ' + screen);
        },

        render: function render() {
            var body = null;

            log('render screen ' + this.state.screen + ' ' + JSON.stringify(this.state.params));

            switch (this.state.screen) {
                case 'game-menu':
                    body = React.createElement(GameMenu, null);break;
                case 'options':
                    body = React.createElement(OptionsMenu, null);break;
                case 'manual':
                    body = React.createElement(Manual, null);break;
                case 'create-game':
                    body = React.createElement(CreateGame, null);break;
                case 'find-game':
                    body = React.createElement(FindGame, null);break;
                case 'game-lobby':
                    body = React.createElement(GameLobby, null);break;

                case 'in-game-ui':
                    body = React.createElement(InGameUi, null);break;
                case 'editor-ui':
                    body = React.createElement(EditorUi, null);break;
                case 'game-over':
                    body = React.createElement(GameOver, null);break;
                case 'in-game-menu':
                    body = React.createElement(InGameMenu, null);break;
                case 'game-paused':
                    body = React.createElement(GamePaused, null);break;
                case 'confirm-leave-game':
                    body = React.createElement(ConfirmLeaveGame, null);break;
                case 'disconnected':
                    body = React.createElement(Disconnected, null);break;
                case 'disconnected-from-lobby':
                    body = React.createElement(DisconnectedFromLobby, null);break;
                case 'failed':
                    body = React.createElement(Failed, null);break;
                case 'waiting':
                    body = React.createElement(Waiting, null);break;
                case 'desync':
                    body = React.createElement(Desync, null);break;

                case 'none':
                    body = null;break;
            }

            if (body) {
                body.props.params = this.state.params;
            }

            return React.createElement(
                'div',
                null,
                body
            );
        }
    });
});