'use strict';

define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'ui/Item'], function (_, sound, React, ReactBootstrap, interop, Item) {
    var DropdownButton = ReactBootstrap.DropdownButton;

    return React.createClass({
        mixins: [],

        componentWillReceiveProps: function componentWillReceiveProps(nextProps) {
            this.setState(this.getInitialState(nextProps));
        },

        getInitialState: function getInitialState(props) {
            if (typeof props === 'undefined') {
                props = this.props;
            }

            return {
                selected: props.selected
            };
        },

        onSelect: function onSelect(item) {
            sound.play.click();

            this.setState({
                selected: item
            });

            if (this.props.onSelect) {
                this.props.onSelect(item);
            }
        },

        onOver: function onOver() {
            if (!this.props.disabled) {
                sound.play.hover();
            }

            interop.onOver();
        },

        render: function render() {
            var self = this;

            var style = _.assign({}, { 'pointer-events': 'auto' }, this.props.style);
            var item = this.state.selected;

            var className = null;
            if (this.props.scroll) {
                className = 'scroll-dropdown';
            }

            return React.createElement(
                'div',
                { style: style },
                React.createElement(
                    DropdownButton,
                    { className: className, disabled: this.props.disabled, title: item.selectedName || item.name,
                        onMouseEnter: this.onOver, onMouseLeave: interop.onLeave,
                        onMouseUp: interop.editorUiClicked,
                        dropup: this.props.dropup },
                    _.map(this.props.choices, function (choice) {
                        return React.createElement(Item, { disabled: choice.taken,
                            item: choice,
                            name: choice.name,
                            onSelect: choice.taken ? null : self.onSelect,
                            onMouseEnter: self.onOver, onMouseLeave: interop.onLeave });
                    })
                )
            );
        }
    });
});