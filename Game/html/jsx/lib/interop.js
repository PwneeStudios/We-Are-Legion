define(['jquery', 'lodash'], function($, _) {
    function InXna()
    {
        return typeof xna !== 'undefined';
    }

    var mouseDownOverHUD = false;
    function docMouseDown(event)
    {
        if (InXna()) {
            xna.OnMouseDown(mouseDownOverHUD, event.which - 1);
        }
        mouseDownOverHUD = false;
    }

    var mouseUpOverHUD = false;
    function docMouseUp(event)
    {
        if(InXna())
        {
            xna.OnMouseUp(mouseUpOverHUD, event.which - 1);
        }
        mouseUpOverHUD = false;
    }

    $(document).ready(function()
    {
        $(document).mousedown( function(event)
        {
            docMouseDown(event);
        });
        
        $("body").mousedown( function(event)
        {
            mouseDownOverHUD = true;
        });
        
        $(document).mouseup( function(event)
        {
            docMouseUp(event);
        });
        
        $("body").mouseup( function(event)
        {
            mouseUpOverHUD = true;
        });
    });
      
    return {
        InXna: function() {
            return InXna();
        },
        
        xna: function() {
            return xna;
        },
    };
});