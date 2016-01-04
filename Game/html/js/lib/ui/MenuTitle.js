'use strict';

define(['lodash', 'sound', 'react', 'react-bootstrap'], function (_, sound, React, ReactBootstrap) {
    var NavItem = ReactBootstrap.NavItem;

    return React.createClass({
        render: function render() {
            //<MenuItem className='title' disabled eventKey={1} href={null}>Are you sure?</MenuItem>
            return React.createElement(
                NavItem,
                { className: 'title', disabled: true, href: null },
                React.createElement(
                    'h3',
                    null,
                    this.props.children
                )
            );
        }
    });
});