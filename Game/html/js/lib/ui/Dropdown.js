define(['lodash', 'react', 'react-bootstrap', 'ui/Item'], function(_, React, ReactBootstrap, Item) {
    var DropdownButton = ReactBootstrap.DropdownButton;

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
                value: this.props.value,
            };
        },

        onSelect: function(item) {
            console.log(item.props.value);
        },
        
        render: function() {
            var self = this;

            return (
                React.createElement("div", {style: {'pointer-events':'auto'}}, 
                    React.createElement(DropdownButton, {title: this.state.value}, 
                        _.map(this.props.choices, function(choice) { return (
                            React.createElement(Item, {value: choice.value, name: choice.name, onSelect: self.onSelect})
                        );})
                    )
                )
            );
        },
    });
});