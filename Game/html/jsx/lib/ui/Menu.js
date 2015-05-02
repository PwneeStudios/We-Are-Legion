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
                        <Well style={{'height':'75%'}}>
                            <Nav bsStyle='pills' stacked style={{'pointer-events':'auto'}}>
                                {this.props.children}
                            </Nav>
                        </Well>
                    );
                    break;

                case 'nav':
                    body = (
                        <Nav bsStyle='pills' stacked style={{'pointer-events':'auto'}}>
                            {this.props.children}
                        </Nav>
                    );
                    break;

                case 'table':
                    body = (
                        <Table style={{width:'100%'}}><tbody>
                            {this.props.children}
                        </tbody></Table>
                    );
            }

            //var pos = pos(13,14);
            var menuPos = pos(65,14);

            return (
                <Div nonBlocking pos={menuPos} size={width(this.props.width)} style={{'font-size':'1%','pointer-events':'auto'}}>
                    {body}
                </Div>
            );
        }
    });
});