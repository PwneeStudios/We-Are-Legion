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

        render: function() {
            return (
                React.createElement(ListGroupItem, React.__spread({href: "#"},  this.props), this.children)
            );
        }
    });
});