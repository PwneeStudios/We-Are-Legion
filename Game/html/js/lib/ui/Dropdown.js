define(['lodash', 'react', 'react-bootstrap', 'ui/Item'], function(_, React, ReactBootstrap, Item) {
    var DropdownButton = ReactBootstrap.DropdownButton;

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
                selected: this.props.selected,
            };
        },

        onSelect: function(item) {
            this.setState({
                selected: item,
            });

            if (this.props.onSelect) {
                this.props.onSelect(item);
            }
        },
        
        render: function() {
            var self = this;

            var style = _.assign({}, {'pointer-events':'auto'}, this.props.style);
            var item = this.state.selected;
            console.log(item);

            return (
                React.createElement("div", {style: style}, 
                    React.createElement(DropdownButton, {title: item.selectedName || item.name}, 
                        _.map(this.props.choices, function(choice) { return (
                            React.createElement(Item, {item: choice, name: choice.name, onSelect: self.onSelect})
                        );})
                    )
                )
            );
        },
    });
});