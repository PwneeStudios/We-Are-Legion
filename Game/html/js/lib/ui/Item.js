define(['lodash', 'react', 'react-bootstrap'], function(_, React, ReactBootstrap) {
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

        render: function() {
            return (
                React.createElement(MenuItem, {onClick: this.onSelect, width: "1000px"}, 
                    this.props.name
                )
            );
        },
    });
});