define(['lodash', 'ui/Div', 'ui/Gap', 'ui/RenderAtMixin', 'ui/UiImage', 'ui/UiButton', 'ui/Dropdown', 'ui/Item', 'ui/MenuItem', 'ui/Menu', 'ui/OptionList', 'ui/util'],
function(_, Div, Gap, RenderAtMixin, UiImage, UiButton, Dropdown, Item, MenuItem, Menu, OptionList, util) {
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
        
        RenderAtMixin: RenderAtMixin,
    };
    
    return _.assign({}, ui, util);
});