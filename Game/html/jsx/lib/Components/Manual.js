define(['lodash', 'react', 'react-bootstrap', 'interop', 'events', 'ui'], function(_, React, ReactBootstrap, interop, events, ui) {
    var Panel = ReactBootstrap.Panel;
    var Button = ReactBootstrap.Button;
    var Well = ReactBootstrap.Well;
    var Nav = ReactBootstrap.Nav;
    var Popover = ReactBootstrap.Popover;
    var Table = ReactBootstrap.Table;
    var CarouselItem = ReactBootstrap.CarouselItem;
    var Carousel = ReactBootstrap.Carousel;
    
    var Div = ui.Div;
    var Gap = ui.Gap;
    var UiImage = ui.UiImage;
    var UiButton = ui.UiButton;
    var Dropdown = ui.Dropdown;
    var RenderAtMixin = ui.RenderAtMixin;
    var MenuItem = ui.MenuItem;
    var Menu = ui.Menu;
    
    var pos = ui.pos;
    var size = ui.size;
    var width = ui.width;
    var subImage = ui.subImage;

    return React.createClass({
    getInitialState: function() {
        return {
            index: 0,
            direction: null
        };
    },

    handleSelect: function(selectedIndex, selectedDirection) {
        this.setState({
            index: selectedIndex,
            direction: selectedDirection
        });
    },

    render: function() {
        return (
            <Div pos={pos(0,0)} size={size(100,100)} style={{'pointer-events':'auto'}}>
                <Carousel activeIndex={this.state.index} direction={this.state.direction} onSelect={this.handleSelect}>
                    <CarouselItem style={{'pointer-events':'auto', 'font-size': '1.4%;'}}>
                        <UiImage width={100} image={{width:1920, height:1080, url:'css/Screen-Instructions.png'}}/>

                        <div className='carousel-caption'>
                            <h3>First slide label</h3>
                            <p>Nulla vitae elit libero, a pharetra augue mollis interdum.</p>
                        </div>
                    </CarouselItem>
                    <CarouselItem>
                        <UiImage width={100} image={{width:1920, height:1080, url:'css/Screen-Spells.png'}}/>

                        <div className='carousel-caption'>
                            <h3>Second slide label</h3>
                            <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p>
                        </div>
                    </CarouselItem>
                    <CarouselItem>
                        <UiImage width={100} image={{width:1920, height:1080, url:'css/Screen-Instructions.png'}}/>

                        <div className='carousel-caption'>
                            <h3>Third slide label</h3>
                            <p>Praesent commodo cursus magna, vel scelerisque nisl consectetur.</p>
                        </div>
                    </CarouselItem>
                </Carousel>

                {/* Buttons */}
                <Div className='top' nonBlocking pos={pos(90,90)} size={width(60)}>
                    <div style={{'pointer-events':'auto'}}>
                        <p>
                            <Button onClick={back}>Back</Button>
                        </p>
                    </div>
                </Div>
            </Div>
        );
    }
});
}); 