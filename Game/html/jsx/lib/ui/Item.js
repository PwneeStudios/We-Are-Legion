define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop'], function(_, sound, React, ReactBootstrap, interop) {
    var MenuItem = ReactBootstrap.MenuItem;

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
            };
        },
        
        onSelect: function() {
            if (this.props.onSelect) {
                this.props.onSelect(this.props.item);
            }
        },

        onOver: function() {
            if (!this.props.disabled) {
                sound.play.listHover();
            }

            interop.onOver();
        },

        render: function() {
            var className = null;
            if (this.props.disabled) {
                className = 'disabled-item';
            }

            return (
                <MenuItem className={className} onClick={this.onSelect} width='1000px'
                          onMouseEnter={this.onOver} onMouseLeave={interop.onLeave} >
                    {this.props.name}
                </MenuItem>
            );
        },
    });
});