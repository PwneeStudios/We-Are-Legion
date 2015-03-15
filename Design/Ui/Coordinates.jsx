// Export Layer Coordinates - Adobe Photoshop Script
// Description: Export x and y coordinates to comma seperated .txt file
// Requirements: Adobe Photoshop CS5 (have not tested on CS4)
// Version: 1.0-beta1.1, 8/31/2011
// Author: Chris DeLuca
// Company: Playmatics
// ===============================================================================
// Installation:
// 1. Place script in
//        Mac: '~/Applications/Adobe Photoshop CS#/Presets/Scripts/'
//        Win: 'C:\Program Files\Adobe\Adobe Photoshop CS#\Presets\Scripts\'
// 2. Restart Photoshop
// 3. Choose File > Scripts > Export Layer Coordinates Photoshop
// ===============================================================================

// Enables double-click launching from the Mac Finder or Windows Explorer
#target photoshop

// Bring application forward
app.bringToFront();

// Set active Document variable and decode name for output
var docRef = app.activeDocument;
var docName = decodeURI(activeDocument.name);

// Define pixels as unit of measurement
var defaultRulerUnits = preferences.rulerUnits;
preferences.rulerUnits = Units.PIXELS;

// Define variable for the number of layers in the active document
var layerNum = app.activeDocument.artLayers.length;

// Define varibles for x and y of layers
var defs = "";
var coords = "";

var w = 1920.0;
var h = 1080.0;
var a = w / h;

function _x(x){
    //return 2 * (x / w) - 1;
    return a - (2 * (x / h) - a);
}

function _y(y){
    return 1 - 2 * (y / h);
}

function vec(x, y){
    return "vec(a - " + _x(x) + "f," + _y(y) + "f)";
}

function recurseLayers(group){
    if (group.visible == false){
        return;
    }

    for (var i = group.layers.length - 1; i >= 0; i--){
        var layerRef = group.layers[i];

        if (!layerRef.visible){
            continue;
        }

        var bounds = layerRef.bounds;
        var x1 = bounds[0].value;
        var y1 = bounds[1].value;
        var x2 = bounds[2].value;
        var y2 = bounds[3].value;
        var name = layerRef.name;

        defs +=
            "Ui.Element(" +
                "\"" + name + "\"" +
            ");\n";

        defs +=
            "Ui.e.SetupPosition(" +
                vec(x1, y2) + "," + vec(x2, y1) +
            ");\n\n";

        if(layerRef.kind == LayerKind.TEXT){
            $.writeln('font: '+ layerRef.textItem.font +' font-size: ' + layerRef.textItem.size + ' color: #' + layerRef.textItem.color.rgb.hexValue);
        }
    
        if (isLayerSet(group.layers[i])){
            recurseLayers(group.layers[i]);
        }
    }
}

// Whether a layer is a group
function isLayerSet(layer){
    try{
        if (layer.layers.length > 0){
            return true;
        }
    }

    catch(err){
        return false;
    }
}

// Ask the user for the folder to export to
var FPath = Folder.selectDialog("Save exported coordinates to");

// Detect line feed type
if ($.os.search(/windows/i) !== -1){
    fileLineFeed = "Windows";
}
else{
    fileLineFeed = "Macintosh";
}

// Export to txt file
function writeFile(info){
    try{
        var f = new File(FPath + "/" + docName + ".txt");
        f.remove();
        f.open('a');
        f.lineFeed = fileLineFeed;
        f.write(info);
        f.close();
    }
    catch(e){
    }
}

// Run the functions
recurseLayers(docRef);
preferences.rulerUnits = defaultRulerUnits; // Set preferences back to user's defaults
writeFile(defs);

// Show results
if (FPath == null){
    alert("Export aborted", "Canceled");
}
else{
    alert("Exported " + layerNum + " layer's coordinates to " + FPath + "/" + docName + ".txt " + "using " + fileLineFeed + " line feeds.", "Success!");
}