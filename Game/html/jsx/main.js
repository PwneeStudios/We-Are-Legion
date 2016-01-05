require(['jquery', 'react', 'Components/GameUi'], function($, React, GameUi) {

    var $style = $("<style type='text/css'>").appendTo('head'); 

    onresize = onload = function() {
    	log('on load!');

        window.w = window.innerWidth;
        window.h = window.innerHeight;
        
        //document.body.style.fontSize = window.w + 'px';
        document.body.style.fontSize = (1.5*window.h) + 'px';

        return;

        var
        	buttonFont = (0.0155*window.h) + 'px',
        	menuFont = (0.025*window.h) + 'px',
        	xPadding = (0.0155*window.h) + 'px',
        	yPadding = (0.011*window.h) + 'px';

        var css = "\
            .btn {\
                font-size: "+ buttonFont +";\
            }\
            .dropdown-menu li {\
            	font-size: "+ buttonFont +";\
            }\
            h3 {\
            	font-size: "+ menuFont +";\
            }\
        }";
/*
            button.dropdown-toggle.btn.btn-default {\
                padding-top: "+ yPadding +";\
                padding-bottom: "+ yPadding +";\
                padding-left: "+ xPadding +";\
                padding-right: "+ xPadding +";\
            }\
*/
        $style.html(css);
    };
    
    onscroll = function() {
        window.scrollTo(0, 0);
    };

    onload();

    React.render(
        <GameUi />,
        document.getElementById('main-div')
    );
});