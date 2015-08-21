define(['lodash', 'sound', 'react', 'react-bootstrap', 'ui/Div', 'ui/Gap', 'ui/RenderAtMixin', 'ui/UiImage', 'ui/UiButton', 'ui/Dropdown', 'ui/Item', 'ui/MenuItem', 'ui/Menu', 'ui/OptionList', 'ui/MenuTitle', 'ui/Input', 'ui/util'],
function(_, sound, React, ReactBootstrap, Div, Gap, RenderAtMixin, UiImage, UiButton, Dropdown, Item, MenuItem, Menu, OptionList, MenuTitle, Input, util) {
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
        MenuTitle: MenuTitle,

        Input: Input,

        OptionList: OptionList,

        back: function() {
            sound.play.back();
            window.back();
        },

        BackButton: React.createClass({
            render: function() {
                return (
                    <Button onMouseEnter={sound.play.hover} onClick={ui.back}>
                        Back
                    </Button>
                );
            },
        }),

        Button: React.createClass({
            onClick: function() {
                sound.play.click();

                if (this.props.onClick !== null) {
                    this.props.onClick();
                }
            },

            render: function() {
                return (
                    <Button
                        {...this.props}
                        disabled={this.props.disabled}
                        onClick={this.onClick}
                        onMouseEnter={sound.play.hover}>
                        {this.props.children}
                    </Button>
                );
            },
        }),
        
        RenderAtMixin: RenderAtMixin,
    };
    
    return _.assign({}, ui, util);
});