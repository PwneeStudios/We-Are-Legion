define(['lodash', 'react', 'react-bootstrap'], function(_, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        render: function() {
            return (
                <NavItem {...this.props}>
                    <h3>{this.props.children}</h3>
                </NavItem>
            );
        }
    });
});