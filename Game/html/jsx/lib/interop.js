define(['jquery', 'lodash'], function($, _) {
    var interop = {
        InXna: function() {
            return typeof xna !== 'undefined';
        },
        
        xna: function() {
            return xna;
        },
        
        onOver: function() {
            console.log('hi')
            
            if (interop.InXna()) {
                xna.OnMouseOver();
            }
        },

        onLeave: function() {
            console.log('leave')
            
            if (interop.InXna()) {
                xna.OnMouseLeave();
            }
        },
    };
    
    return interop;
});