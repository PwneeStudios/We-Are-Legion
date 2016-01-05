define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'events', 'ui', 'Components/Chat'], function(_, sound, React, ReactBootstrap, interop, events, ui, Chat) {
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
        mixin: [],

        getInitialState: function() {
            return {
            };
        },
        
        onClick: function() {
            if (this.props.data.GameStarted) {
                interop.watchGame(this.props.data.Index);
            } else {
                setScreen('game-lobby', {
                    host: false,
                    lobbyIndex: this.props.data.Index,
                });
            }
        },

        render: function() {
            log('a wild lobby');
            log(JSON.stringify(this.props.data));

            var info = this.props.data.NumPlayers + ' / ' + this.props.data.MaxPlayers;
            if (this.props.data.NumSpectators) {
                info = <span>{info} &nbsp;&nbsp; {'+' + JSON.stringify(this.props.data.NumSpectators)}</span>;
            }

            return (
                <tr>
                    <td>{this.props.data.Name}</td>
                    {/*<td>{this.props.data.MemberCount} / {this.props.data.Capacity}</td>*/}
                    <td>{info}</td>
                    <td>
                        <Button onClick={this.onClick}>
                            {this.props.data.GameStarted ? 'Watch' : 'Join'}
                        </Button>
                    </td>
                </tr>
            );
        },
    });

    return React.createClass({
        mixins: [events.FindLobbiesMixin, events.AllowBackMixin],

        onFindLobbies: function(values) {
            log('found lobbies');

            this.setState({
                loading: false,
                lobbies: values.Lobbies,
                online: values.Online,
            });
        },

        loadingState: {
            loading: true,
            lobbies: [],
        },

        refreshLobbies: function() {
            this.refresh();
            this.refs.refreshButton.getDOMNode().blur();
        },

        findLobbies: function(friendsFlag) {
            var _this = this;

            this.refresh = function() {
                interop.findLobbies(friendsFlag);
                _this.setState(_this.loadingState);
            };
            
            this.refresh();

        },
                
        getInitialState: function() {
            this.findLobbies(false);

            return this.loadingState;
        },

        onVisibilityChange: function(item) {
            var friends = item.value === 'friends';

            this.findLobbies(friends);
        },
        
        render: function() {
            var _this = this;

            var visibility = [
                {name:'Public games', value:'public'},
                {name:'Friend games', value:'friends'},
            ];

            var view = null;

            if (this.state.loading) {
                view = 'Loading...';
            } if (!this.state.online) {
                view = "Offline. Can't find lobbies.";
            } else {
                view = (
                    <Table style={{width:'100%','pointer-events':'auto'}}><tbody>
                        {_.map(this.state.lobbies, function(lobby) {
                            return <GameItem data={lobby} />;
                        })}
                    </tbody></Table>
                );
            }

            return (
                <div>
                    <Div nonBlocking pos={pos(10,5)} size={width(80)}>
                        <Well style={{'height':'90%'}}>
                            {/* Header */}
                            <h2>Game list</h2>

                            {/* Game List */}
                            <Div className='game-list' pos={pos(3.3,16.9)} size={size(50,66.2)}
                                 style={{'overflow-y':'scroll','pointer-events':'auto','font-size': '1.4%'}}>
                                 {view}
                            </Div>

                            {/* Game visibility type */}
                            <Div pos={pos(55.3,16.9)} size={size(30,66.2)}>
                                <OptionList options={visibility} onSelect={this.onVisibilityChange} />

                                {/* Refresh */}
                                <div style={{'pointer-events':'auto'}}>
                                    <p>
                                        <Button ref='refreshButton' onClick={this.refreshLobbies}>
                                            Refresh
                                        </Button>
                                    </p>
                                </div>
                            </Div>

                            {/* Buttons */}
                            <Div nonBlocking pos={pos(38,80)} size={width(60)}>
                                <div style={{'float':'right', 'pointer-events':'auto'}}>
                                    <p>
                                        <ui.BackButton />
                                    </p>
                                </div>
                            </Div>

                        </Well>
                    </Div>
                </div>
            );
        }
    });
}); 