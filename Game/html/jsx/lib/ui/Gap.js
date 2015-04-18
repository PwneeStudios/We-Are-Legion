define(['lodash', 'react'], function(_, React) {
    return React.createClass({
        render: function() {
            return (
                <div style={{'display':'inline-block', 'float':'left','overflow':'hidden','height':'1px',width:this.props.width + '%',}} />
            );
        }
    });
});