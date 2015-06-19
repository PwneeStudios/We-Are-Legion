define(['lodash', 'sound', 'react', 'react-bootstrap'], function(_, sound, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        onClick: function(e) {
            if (this.props.to) {
                if (this.props.to === 'back') {
                    sound.play.back();
                    window.back();
                } else {
                    sound.play.click();
                    window.setScreen(this.props.to, this.props.params);
                }
            } else if (this.props.toMode) {
                sound.play.click();
                window.setMode(this.props.toMode, this.props.params);
            } else if (this.props.onClick) {
                sound.play.click();
                this.props.onClick(e);
            }
        },

        render: function() {
            return (
                <NavItem {...this.props} onClick={this.onClick} onMouseEnter={sound.play.hover}>
                    <h3>{this.props.children}</h3>
                </NavItem>
            );
        }
    });
});