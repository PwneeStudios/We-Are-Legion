define(['lodash', 'react', 'react-bootstrap', 'ui/util', 'ui/Div'], function(_, React, ReactBootstrap, util, Div) {
    var ListGroup = ReactBootstrap.ListGroup;
    var ListGroupItem = ReactBootstrap.ListGroupItem;

    var pos = util.pos;
    var size = util.size;
    var width = util.width;
    var subImage = util.subImage;

    return React.createClass({
        getDefaultProps: function() {
            return {
            };
        },

        getInitialState: function() {
            return {
                value: this.props.value || this.props.options[0].value,
            };
        },

        onSelect: function(item) {
            this.setState({
                value: item.value,
            });
        },

        render: function() {
            var _this = this;

            var options = _.map(this.props.options, function(option) {
                return (
                    React.createElement(ListGroupItem, {active: option.value===_this.state.value, 
                                   href: "#", 
                                   style: _this.props.disabled ? {cursor:'default'} : {}, 
                                   onClick: _this.props.disabled ? null : function() { _this.onSelect(option); }}, 
                        option.name
                    )
                );
            });

            var style = {
                'pointer-events': this.props.disabled ? 'none' : 'auto',
                'font-size': '1.4%',
            };

            return (
                React.createElement(ListGroup, {style: style}, 
                    options
                )
            );
        }
    });
});