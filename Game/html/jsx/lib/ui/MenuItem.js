define(['lodash', 'react', 'react-bootstrap'], function(_, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        onClick: function(e) {
            if (this.props.to) {
                if (this.props.to === 'back') {
                    window.back();
                } else {
                    window.setScreen(this.props.to);
                }
            }
        },

        render: function() {
            return (
                <NavItem {...this.props} onClick={this.onClick}>
                    <h3>{this.props.children}</h3>
                </NavItem>
            );
        }
    });
});