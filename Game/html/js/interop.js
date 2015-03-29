
// A useful helper function that will let you create your GUI in a web browser
function InXNA()
{
	return typeof xna !== 'undefined';
}


handleDataFromXNA = function(data)
{
	$("#data").text(data);
}



var mouseDownOverHUD = false;
function docMouseDown(event)
{
	if(InXNA())
	{
		xna.OnMouseDown(mouseDownOverHUD, event.which - 1);
	}
	mouseDownOverHUD = false;
}

var mouseUpOverHUD = false;
function docMouseUp(event)
{
	if(InXNA())
	{
		xna.OnMouseUp(mouseUpOverHUD, event.which - 1);
	}
	mouseUpOverHUD = false;
}


$(document).ready(function()
{
	// Make button presses look cool
	$(".button").mousedown( function(event)
	{
		$(this).addClass("buttonPressed");
	});
	
	$(".button").mouseup( function(event)
	{
		$(this).removeClass("buttonPressed");
		if(!$(this).hasClass("disabled"))
		{
            console.log(this);
			XNACall($(this));
		}
	});
	
	$(".button").mouseleave( function(event)
	{
		$(this).removeClass("buttonPressed");
	});
	
	
	
	// Capture mouse up/down events so we can tell XNA if we've handled the action or not
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


function XNACall(param)
{
	if(InXNA())
	{
		var callString;
		if (typeof param === 'string')
		{
			callString = param;
		}
		else if (typeof param !== 'undefined' && typeof param.attr("call") !== 'undefined')
		{
			callString = param.attr("call");
		}
		else
		{
			return;
		}
		
		eval(callString);
	}
}




// Prevent the backspace key from navigating back.
// Grabbed from http://stackoverflow.com/a/2768256/536974
$(document).unbind('keydown').bind('keydown', function (event) {
    var doPrevent = false;
    if (event.keyCode === 8) {
        var d = event.srcElement || event.target;
        if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD')) 
             || d.tagName.toUpperCase() === 'TEXTAREA') {
            doPrevent = d.readOnly || d.disabled;
        }
        else {
            doPrevent = true;
        }
    }

    if (doPrevent) {
        event.preventDefault();
    }
});
