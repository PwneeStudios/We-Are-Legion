require(['react', 'Components/GameUi'], function(React, GameUi) {
    React.render(
        React.createElement(GameUi, null),
        document.getElementById('main-div')
    );
});