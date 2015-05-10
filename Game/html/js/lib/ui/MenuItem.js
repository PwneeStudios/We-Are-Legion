define(['lodash', 'react', 'react-bootstrap'], function(_, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        onClick: function(e) {
            if (this.props.to) {
                if (this.props.to === 'back') {
                    window.back();
                } else {
                    window.setScreen(this.props.to, this.props.params);
                }
            } else if (this.props.toMode) {
                window.setMode(this.props.toMode, this.props.params);
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