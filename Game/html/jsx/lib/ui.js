define(['lodash', 'ui/Div', 'ui/Gap', 'ui/RenderAtMixin', 'ui/UiImage', 'ui/UiButton', 'ui/util'], function(_, Div, Gap, RenderAtMixin, UiImage, UiButton, util) {
    var ui = {
        Div: Div,
        Gap: Gap,
        
        UiImage: UiImage,
        UiButton: UiButton,
        
        RenderAtMixin: RenderAtMixin,
    };
    
    return _.assign({}, ui, util);
});