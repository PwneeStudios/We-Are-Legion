({
    appDir: "../",
    baseUrl: 'js/lib',
    paths: {
        require: 'requirejs/require',
        react: 'react/react.min',
        main: '../main',
    },
    dir: "../../appdirectory-build",
    modules: [
        {
            name: "../app"
        }
    ]
})