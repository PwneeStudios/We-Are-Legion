define(['react', 'ui/Div'], function(React, Div) {
    return {
        render: function() {
            if (this.props.pos) {
                return (
                    React.createElement(Div, React.__spread({},  this.props), 
                        this.renderAt()
                    )
                );
            } else {
                return this.renderAt();
            }
        }
    };
});