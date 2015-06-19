define(['lodash', 'sound', 'react', 'react-bootstrap', 'ui/Div', 'ui/Gap', 'ui/RenderAtMixin', 'ui/UiImage', 'ui/UiButton', 'ui/Dropdown', 'ui/Item', 'ui/MenuItem', 'ui/Menu', 'ui/OptionList', 'ui/util'],
function(_, sound, React, ReactBootstrap, Div, Gap, RenderAtMixin, UiImage, UiButton, Dropdown, Item, MenuItem, Menu, OptionList, util) {
    var Button = ReactBootstrap.Button;

    var ui = {
        Div: Div,
        Gap: Gap,
        
        UiImage: UiImage,
        UiButton: UiButton,

        Dropdown: Dropdown,
        Item: Item,

        Menu: Menu,
        MenuItem: MenuItem,

        OptionList: OptionList,

        back: function() {
            sound.play.back();
            window.back();
        },

        BackButton: function() {
            return (
                <Button onMouseEnter={sound.play.hover} onClick={ui.back}>Back</Button>
            );
        },
        
        RenderAtMixin: RenderAtMixin,
    };
    
    return _.assign({}, ui, util);
});