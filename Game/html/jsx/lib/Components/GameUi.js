define(['lodash', 'react', 'react-bootstrap'], function(_, React, ReactBootstrap) {
    var Input = ReactBootstrap.Input;
    var OverlayTrigger = ReactBootstrap.OverlayTrigger;
    var Popover = ReactBootstrap.Popover;
    var Button = ReactBootstrap.Button;

    var pos = function(x, y, type) {
        if (typeof type === 'undefined') {
            type = 'absolute';
        }
    
        return {
            position:type,
            left: x + '%',
            top: y + '%',
        };
    };

    var width = function(x) {
        return {
            width: x + '%',
        };
    };

    var Div = React.createClass({
        render: function() {
            var style = {
                width:'100%',
                height:'100%',
            };
            
            style = _.assign(style, this.props.pos, this.props.size);
        
            console.log(style);
        
            return (
                <div style={style}>
                    {this.props.children}
                </div>
            );
        }
    });

    var UiButtonMixin = {
        render: function() {
            var image = this.props.image;
            image.aspect = image.height / image.width;
            
            var width = this.props.width;
            var height = width * image.aspect;
            
            var button = <button className="UiButton" style={{'background-image': 'url('+image.url+')'}} />;
            
            var body;
            if (this.props.overlay) {
                body = 
                    <OverlayTrigger placement="top" overlay={this.props.overlay} delayShow={300} delayHide={150}>
                        {button}
                    </OverlayTrigger>
            } else {
                body = button;
            }
            
            return (
                <div style={{width:width+'%', height:0, 'padding-bottom':height+'%', position:'relative', 'float':'left'}}>
                    {body}
                </div>
            );
        }
    };
    
    var UiButton = React.createClass({
        mixins: [UiButtonMixin],
        
        getDefaultProps: function() {
            return {
                width: 7,
                
                image: {
                    width:160,
                    height:160,
                    url:'css/UiButton.png',
                },
            };
        },
    });

    var Gap = React.createClass({
        render: function() {
            return (
                <div style={{'float':'left','overflow':'hidden','height':'1px',width:this.props.width + '%',}} />
            );
        }
    });
    
    return React.createClass({
        render: function() {
            var tooltip = <Popover title="Fireball"><strong>FIRE!</strong> Check this info.</Popover>;
        
            return (
                <Div pos={pos(15,0)}>
                    <Div pos={pos(0,75)} size={width(50)}>
                        <Input type="text" addonBefore="All" />
                    </Div>
                    
                    <Div pos={pos(0,80)}>
                        <UiButton overlay={tooltip}/>
                        <UiButton />
                        <UiButton />
                        <UiButton />
                        
                        <Gap width='1' />
                        
                        <UiButton />
                        <UiButton />
                        <UiButton />
                    </Div>
                </Div>
            );
        }
    });
});