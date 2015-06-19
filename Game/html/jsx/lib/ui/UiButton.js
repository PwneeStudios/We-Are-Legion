define(['lodash', 'sound', 'react', 'react-bootstrap', 'interop', 'ui/RenderAtMixin'], function(_, sound, React, ReactBootstrap, interop, RenderAtMixin) {
    var OverlayTrigger = ReactBootstrap.OverlayTrigger;
    var Popover = ReactBootstrap.Popover;
    
    return React.createClass({
        mixins: [RenderAtMixin],
        
        getInitialState: function() {
            return {
                preventTooltip: false,
            };
        },
        
        onClick: function() {
            if (this.props.onClick) {
                sound.play.click();
                this.props.onClick();
            }
            
            this.setState({
                preventTooltip: true,
            });
        },

        onMouseLeave: function() {
            this.setState({
                preventTooltip: false,
            });
        },
        
        renderAt: function() {            
            var image = this.props.image;
            image.aspect = image.height / image.width;
            
            var width = this.props.width;
            var height = width * image.aspect;
            
            var button = <button className="UiButton" style={{backgroundImage: 'url('+image.url+')'}} onClick={this.onClick} 
                          onMouseEnter={interop.onOver} onMouseLeave={interop.onLeave}/>;
            
            var body;
            if (this.props.overlay && !this.state.preventTooltip) {
                body = 
                    <OverlayTrigger placement="top" overlay={this.props.overlay} delayShow={420} delayHide={50}>
                        {button}
                    </OverlayTrigger>
            } else {
                body = button;
            }
            
            var divStyle = {
                width:width+'%',
                height:0,
                paddingBottom:height+'%',
                position:'relative', 'float':'left',
                'pointer-events':'auto',
            };
            
            return (
                <div style={divStyle} onMouseLeave={this.onMouseLeave} >
                    {body}
                </div>
            );
        }
    });
});