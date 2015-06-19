define(['lodash', 'sound', 'react', 'react-bootstrap', 'ui/Item'], function(_, sound, React, ReactBootstrap, Item) {
    var DropdownButton = ReactBootstrap.DropdownButton;

    return React.createClass({
        mixins: [],

        componentWillReceiveProps: function(nextProps) {
            this.setState(this.getInitialState(nextProps));
        },
                
        getInitialState: function(props) {
            if (typeof props === 'undefined') {
                props = this.props;
            }

            return {
                selected: props.selected,
            };
        },

        onSelect: function(item) {
            sound.play.click();

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

            var className = null;
            if (this.props.scroll) {
                className = 'scroll-dropdown';
            }

            return (
                React.createElement("div", {style: style}, 
                    React.createElement(DropdownButton, {className: className, disabled: this.props.disabled, title: item.selectedName || item.name, 
                                    onMouseEnter: this.props.disabled ? null : sound.play.hover}, 
                        _.map(this.props.choices, function(choice) {
                            //var _choice = _.clone(choice);
                            return (
                                React.createElement(Item, {disabled: choice.taken, item: choice, name: choice.name, onSelect: self.onSelect})
                            );
                        })
                    )
                )
            );
        },
    });
});