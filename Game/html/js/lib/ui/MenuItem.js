define(['lodash', 'react', 'react-bootstrap'], function(_, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        onClick: function(e) {
            if (this.props.to) {
                window.setScreen(this.props.to);
            }
        },

        render: function() {
            return (
                React.createElement(NavItem, React.__spread({},  this.props, {onClick: this.onClick}), 
                    React.createElement("h3", null, this.props.children)
                )
            );
        }
    });
});