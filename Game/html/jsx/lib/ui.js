define(['lodash', 'ui/Div', 'ui/Gap', 'ui/RenderAtMixin', 'ui/UiImage', 'ui/UiButton', 'ui/Dropdown', 'ui/Item', 'ui/MenuItem', 'ui/util'],
function(_, Div, Gap, RenderAtMixin, UiImage, UiButton, Dropdown, Item, MenuItem, util) {
    var ui = {
        Div: Div,
        Gap: Gap,
        
        UiImage: UiImage,
        UiButton: UiButton,

        Dropdown: Dropdown,
        Item: Item,

        MenuItem: MenuItem,
        
        RenderAtMixin: RenderAtMixin,
    };
    
    return _.assign({}, ui, util);
});