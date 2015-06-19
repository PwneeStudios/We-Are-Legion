define(['lodash', 'sound', 'react', 'react-bootstrap'], function(_, sound, React, ReactBootstrap) {
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
            } else if (this.props.onClick) {
                this.props.onClick(e);
            }
        },

        render: function() {
            return (
                <NavItem {...this.props} onClick={this.onClick} onMouseEnter={sound.onEvent('Menu_Back')}>
                    <h3>{this.props.children}</h3>
                </NavItem>
            );
        }
    });
});