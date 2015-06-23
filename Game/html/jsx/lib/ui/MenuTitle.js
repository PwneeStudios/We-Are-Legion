define(['lodash', 'sound', 'react', 'react-bootstrap'], function(_, sound, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        render: function() {
            //<MenuItem className='title' disabled eventKey={1} href={null}>Are you sure?</MenuItem>
            return (
                <NavItem className='title' disabled href={null}>
                    <h3>{this.props.children}</h3>
                </NavItem>
            );
        }
    });
});