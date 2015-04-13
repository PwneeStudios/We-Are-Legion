define(['react', 'ui/Div'], function(React, Div) {
    return {
        render: function() {
            if (this.props.pos) {
                return (
                    <Div {...this.props}>
                        {this.renderAt()}
                    </Div>
                );
            } else {
                return this.renderAt();
            }
        }
    };
});