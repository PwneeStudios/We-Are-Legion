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
            
            var button = <button className="UiButton" style={{backgroundImage: 'url('+image.url+')'}} />;
            
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
                <div style={{width:width+'%', height:0, paddingBottom:height+'%', position:'relative', 'float':'left'}}>
                    {body}
                </div>
            );
        }
    };

    var UiImageMixin = {
        render: function() {
            var image = this.props.image;
            image.aspect = image.height / image.width;
            
            var width = this.props.width;
            var height = width * image.aspect;
            
            var button = <div className="UiImage" style={{backgroundImage: 'url('+image.url+')'}} />;
            
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
                <div style={{width:width+'%', height:0, paddingBottom:height+'%', position:'relative', 'float':'left'}}>
                    {body}
                </div>
            );
        }
    };
    
    var UiImage = React.createClass({
        mixins: [UiButtonMixin],
    });
    
    var ActionButton = React.createClass({
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

    var UnitBar = React.createClass({
        mixins: [UiImageMixin],
        
        getDefaultProps: function() {
            return {
                width: 100,
                
                image: {
                    width:869,
                    height:60,
                    url:'css/UnitBar.png',
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
                    <Div pos={pos(20,5)} size={width(50)}>
                        <UnitBar />
                    </Div>

                    <Div pos={pos(0,75)} size={width(50)}>
                        <Input type="text" addonBefore="All" />
                    </Div>
                    
                    <Div pos={pos(0,80)}>
                        <ActionButton overlay={tooltip}/>
                        <ActionButton />
                        <ActionButton />
                        <ActionButton />
                        
                        <Gap width='1' />
                        
                        <ActionButton />
                        <ActionButton />
                        <ActionButton />
                    </Div>
                </Div>
            );
        }
    });
});