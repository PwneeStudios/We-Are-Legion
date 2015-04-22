define(['lodash', 'react', 'react-bootstrap'], function(_, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        render: function() {
            return (
                React.createElement(NavItem, React.__spread({},  this.props), 
                    React.createElement("h3", null, this.props.children)
                )
            );
        }
    });
});