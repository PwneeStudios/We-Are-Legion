require.config({
    baseUrl: 'js/lib',
    paths: {
        require: 'requirejs/require',
        react: 'react/react.min',
        'react-bootstrap': 'react-bootstrap/react-bootstrap.min',
        lodash: 'lodash/lodash.min',
        main: '../main',
    },
});

require(['main']);