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
                    <ListGroupItem href='#' active={option.value===_this.state.value}
                     onClick={function() { _this.onSelect(option); }}>
                        {option.name}
                    </ListGroupItem>
                );
            });

            return (
                <ListGroup style={{'pointer-events':'auto','font-size': '1.4%'}}>
                    {options}
                </ListGroup>
            );
        }
    });
});