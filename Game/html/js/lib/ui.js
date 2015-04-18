define(['lodash', 'ui/Div', 'ui/Gap', 'ui/RenderAtMixin', 'ui/UiImage', 'ui/UiButton', 'ui/Dropdown', 'ui/Item', 'ui/util'],
function(_, Div, Gap, RenderAtMixin, UiImage, UiButton, Dropdown, Item, util) {
    var ui = {
        Div: Div,
        Gap: Gap,
        
        UiImage: UiImage,
        UiButton: UiButton,

        Dropdown: Dropdown,
        Item: Item,
        
        RenderAtMixin: RenderAtMixin,
    };
    
    return _.assign({}, ui, util);
});