define(['lodash', 'react', 'react-bootstrap', 'ui/Item'], function(_, React, ReactBootstrap, Item) {
    var DropdownButton = ReactBootstrap.DropdownButton;

    return React.createClass({
        mixins: [],
                
        getInitialState: function() {
            return {
                value: this.props.selected.value,
                name: this.props.selected.name,
            };
        },

        onSelect: function(item) {
            var value = item.props.value;
            var name = item.props.name;

            //console.log(value);
            this.setState({
                value:value,
                name:name,
            });
            if (this.props.onSelect) {
                this.props.onSelect(value);
            }
        },
        
        render: function() {
            var self = this;

            var style = _.assign({}, {'pointer-events':'auto'}, this.props.style);

            return (
                <div style={style}>
                    <DropdownButton title={this.state.name}>
                        {_.map(this.props.choices, function(choice) { return (
                            <Item value={choice.value} name={choice.name} onSelect={self.onSelect}/>
                        );})}
                    </DropdownButton>
                </div>
            );
        },
    });
});