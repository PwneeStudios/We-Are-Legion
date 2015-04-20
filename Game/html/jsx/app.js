require.config({
    baseUrl: 'js/lib',
    paths: {
        require: 'requirejs/require',
        jquery: 'jquery/dist/jquery.min',
        react: 'react/react.min',
        //react: 'react/react-with-addons.min',
        'react-bootstrap': 'react-bootstrap/react-bootstrap.min',
        lodash: 'lodash/lodash.min',
        main: '../main',
    },
});

require(['main']);