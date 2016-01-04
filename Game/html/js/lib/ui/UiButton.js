'use strict';

define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'ui/RenderAtMixin'], function (_, sound, React, ReactBootstrap, interop, RenderAtMixin) {
    var OverlayTrigger = ReactBootstrap.OverlayTrigger;
    var Popover = ReactBootstrap.Popover;

    return React.createClass({
        mixins: [RenderAtMixin],

        getInitialState: function getInitialState() {
            return {
                preventTooltip: false
            };
        },

        onClick: function onClick() {
            if (this.props.onClick) {
                sound.play.click();
                this.props.onClick();
            }

            this.setState({
                preventTooltip: true
            });
        },

        onMouseLeave: function onMouseLeave() {
            this.setState({
                preventTooltip: false
            });
        },

        renderAt: function renderAt() {
            var image = this.props.image;
            image.aspect = image.height / image.width;

            var width = this.props.width;
            var height = width * image.aspect;

            var button = React.createElement('button', { className: 'UiButton', style: { backgroundImage: 'url(' + image.url + ')' }, onClick: this.onClick,
                onMouseEnter: interop.onOver, onMouseLeave: interop.onLeave });

            var body;
            if (this.props.overlay && !this.state.preventTooltip) {
                body = React.createElement(
                    OverlayTrigger,
                    { placement: 'top', overlay: this.props.overlay, delayShow: 420, delayHide: 50 },
                    button
                );
            } else {
                body = button;
            }

            var divStyle = {
                width: width + '%',
                height: 0,
                paddingBottom: height + '%',
                position: 'relative', 'float': 'left',
                'pointer-events': 'auto'
            };

            return React.createElement(
                'div',
                { style: divStyle, onMouseLeave: this.onMouseLeave },
                body
            );
        }
    });
});