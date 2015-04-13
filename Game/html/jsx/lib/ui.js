define(['lodash', 'ui/Div', 'ui/RenderAtMixin', 'ui/UiImage', 'ui/UiButton', 'ui/util'], function(_, Div, RenderAtMixin, UiImage, UiButton, util) {
    var ui = {
        Div: Div,
        
        UiImage: UiImage,
        UiButton: UiButton,
        
        RenderAtMixin: RenderAtMixin,
    };
    
    return _.assign({}, ui, util);
});