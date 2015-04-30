define(['lodash', 'react', 'react-bootstrap', 'ui/util', 'ui/Div'], function(_, React, ReactBootstrap, util, Div) {
    var Nav = ReactBootstrap.Nav;
    var NavItem = ReactBootstrap.NavItem;
    var Table = ReactBootstrap.Table;

    var pos = util.pos;
    var size = util.size;
    var width = util.width;
    var subImage = util.subImage;

    return React.createClass({
        getDefaultProps: function() {
            return {
                width:25,
                type:'nav',
            };
        },

        render: function() {
            var body;

            switch (this.props.type) {
                case 'well':
                    body = (
                        React.createElement(Well, {style: {'height':'75%'}}, 
                            React.createElement(Nav, {bsStyle: "pills", stacked: true, style: {'pointer-events':'auto'}}, 
                                this.props.children
                            )
                        )
                    );
                    break;

                case 'nav':
                    body = (
                        React.createElement(Nav, {bsStyle: "pills", stacked: true, style: {'pointer-events':'auto'}}, 
                            this.props.children
                        )
                    );
                    break;

                case 'table':
                    body = (
                        React.createElement(Table, {style: {width:'100%'}}, React.createElement("tbody", null, 
                            this.props.children
                        ))
                    );
            }

            //<div className='menu'>
            return (
                React.createElement(Div, {nonBlocking: true, pos: pos(13,14), size: width(this.props.width), style: {'font-size':'1%','pointer-events':'auto'}}, 
                    body
                )
            );
        }
    });
});