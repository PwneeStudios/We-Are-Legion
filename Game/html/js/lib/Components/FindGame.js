'use strict';

define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function (_, sound, React, ReactBootstrap, interop, events, ui, Chat) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    var ListGroup = ReactBootstrap.ListGroup;
    var ListGroupItem = ReactBootstrap.ListGroupItem;

    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var OptionList = ui.OptionList;
    var RenderAtMixin = ui.RenderAtMixin;

    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    var GameItem = React.createClass({
        displayName: 'GameItem',

        mixin: [],

        getInitialState: function getInitialState() {
            return {};
        },

        onClick: function onClick() {
            if (this.props.data.GameStarted) {
                interop.watchGame(this.props.data.Index);
            } else {
                setScreen('game-lobby', {
                    host: false,
                    lobbyIndex: this.props.data.Index
                });
            }
        },

        render: function render() {
            log('a wild lobby');
            log(JSON.stringify(this.props.data));

            var info = this.props.data.NumPlayers + ' / ' + this.props.data.MaxPlayers;
            if (this.props.data.NumSpectators) {
                info = React.createElement(
                    'span',
                    null,
                    info,
                    '    ',
                    '+' + JSON.stringify(this.props.data.NumSpectators)
                );
            }

            return React.createElement(
                'tr',
                null,
                React.createElement(
                    'td',
                    null,
                    this.props.data.Name
                ),
                React.createElement(
                    'td',
                    null,
                    info
                ),
                React.createElement(
                    'td',
                    null,
                    React.createElement(
                        Button,
                        { onClick: this.onClick },
                        this.props.data.GameStarted ? 'Watch' : 'Join'
                    )
                )
            );
        }
    });

    return React.createClass({
        mixins: [events.FindLobbiesMixin, events.AllowBackMixin],

        onFindLobbies: function onFindLobbies(values) {
            log('found lobbies');

            this.setState({
                loading: false,
                lobbies: values.Lobbies,
                online: values.Online
            });
        },

        loadingState: {
            loading: true,
            lobbies: []
        },

        refreshLobbies: function refreshLobbies() {
            this.refresh();
            this.refs.refreshButton.getDOMNode().blur();
        },

        findLobbies: function findLobbies(friendsFlag) {
            var _this = this;

            this.refresh = function () {
                interop.findLobbies(friendsFlag);
                _this.setState(_this.loadingState);
            };

            this.refresh();
        },

        getInitialState: function getInitialState() {
            this.findLobbies(false);

            return this.loadingState;
        },

        onVisibilityChange: function onVisibilityChange(item) {
            var friends = item.value === 'friends';

            this.findLobbies(friends);
        },

        render: function render() {
            var _this = this;

            var visibility = [{ name: 'Public games', value: 'public' }, { name: 'Friend games', value: 'friends' }];

            var view = null;

            if (this.state.loading) {
                view = 'Loading...';
            }if (!this.state.online) {
                view = "Offline. Can't find lobbies.";
            } else {
                view = React.createElement(
                    Table,
                    { style: { width: '100%', 'pointer-events': 'auto' } },
                    React.createElement(
                        'tbody',
                        null,
                        _.map(this.state.lobbies, function (lobby) {
                            return React.createElement(GameItem, { data: lobby });
                        })
                    )
                );
            }

            return React.createElement(
                'div',
                null,
                React.createElement(
                    Div,
                    { nonBlocking: true, pos: pos(10, 5), size: width(80) },
                    React.createElement(
                        Well,
                        { style: { 'height': '90%' } },
                        React.createElement(
                            'h2',
                            null,
                            'Game list'
                        ),
                        React.createElement(
                            Div,
                            { className: 'game-list', pos: pos(3.3, 16.9), size: size(50, 66.2),
                                style: { 'overflow-y': 'scroll', 'pointer-events': 'auto', 'font-size': '1.4%' } },
                            view
                        ),
                        React.createElement(
                            Div,
                            { pos: pos(55.3, 16.9), size: size(30, 66.2) },
                            React.createElement(OptionList, { options: visibility, onSelect: this.onVisibilityChange }),
                            React.createElement(
                                'div',
                                { style: { 'pointer-events': 'auto' } },
                                React.createElement(
                                    'p',
                                    null,
                                    React.createElement(
                                        Button,
                                        { ref: 'refreshButton', onClick: this.refreshLobbies },
                                        'Refresh'
                                    )
                                )
                            )
                        ),
                        React.createElement(
                            Div,
                            { nonBlocking: true, pos: pos(38, 80), size: width(60) },
                            React.createElement(
                                'div',
                                { style: { 'float': 'right', 'pointer-events': 'auto' } },
                                React.createElement(
                                    'p',
                                    null,
                                    React.createElement(ui.BackButton, null)
                                )
                            )
                        )
                    )
                )
            );
        }
    });
});