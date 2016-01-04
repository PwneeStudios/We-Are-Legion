define(['lodash', 'react', 'interop', 'ui/RenderAtMixin'], function(_, React, interop, RenderAtMixin) {
    return React.createClass({
        mixins: [RenderAtMixin],

        renderAt: function() {
            var image = this.props.image;
            image.aspect = image.height / image.width;
            
            var offset = image.offset || this.props.offset;            
            var width = this.props.width;
            var height = width * image.aspect;
                        
            var background_x = 0, background_y = 0;
            if (offset) {
                background_x = image.dim[0] <= 1 ? 0 : 100 * offset[0] / (image.dim[0] - 1);
                background_y = image.dim[1] <= 1 ? 0 : 100 * offset[1] / (image.dim[1] - 1);
            }
            
            var backgroundPos = background_x + '%' + ' ' + background_y + '%';

            var background_x = 0, background_y = 0;
            var background_size_x = image.dim && image.dim[0] >= 1 ? 100 * image.dim[0] : 100;
            var background_size_y = image.dim && image.dim[1] >= 1 ? 100 * image.dim[1] : 100;
            var backgroundSize = background_size_x + '%' + ' ' + background_size_y + '%';
            
            var style = {backgroundImage: 'url('+image.url+')', backgroundPosition:backgroundPos, backgroundSize:backgroundSize};
            style = _.assign(style, this.props.style);
            
            var img = <div className="UiImage" style={style} />;
            
            var body;
            if (this.props.overlay) {
                body = 
                    <OverlayTrigger placement="top" overlay={this.props.overlay} delayShow={300} delayHide={150}>
                        {img}
                    </OverlayTrigger>
            } else {
                body = img;
            }
            
            var divStyle = {
                'pointer-events':'auto',
                width:width+'%',
                height:0,
                paddingBottom:height+'%',
                position:'relative',
                'float':'left',
            };
            
            if (this.props.nonBlocking) {
                divStyle['pointer-events'] = 'none';
            }

            return (
                <div style={divStyle} onMouseOver={this.props.nonBlocking ? null : interop.onOver} onMouseLeave={this.props.nonBlocking ? null : interop.onLeave}>
                    {body}
                </div>
            );
        }
    });
});