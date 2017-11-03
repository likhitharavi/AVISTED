// Write your Javascript code.
function DownloadFromSVG()
{
    var SvgString = document.getElementById("ImgFile").value;
    svgString2Image(svgString, 2 * 300, 2 * 300, 'png', save);

}
function save(dataBlob, filesize) {
    debugger;
    // var imgFile = document.getElementById("ImgFile");
    //  imgFile.value = dataBlob;
    /*var urlCreator = window.URL || window.webkitURL;
    var imageUrl = urlCreator.createObjectURL(dataBlob);
    document.querySelector("#ImgFile").src = imageUrl;*/
    ///document.querySelector("#ImgFile").value = JSON.stringify({ data: dataBlob });

    saveAs(dataBlob, 'AVISTEDChart.png'); // FileSaver.js function

}
function saveImg() {
    debugger;
    var tmp = document.getElementById("chart");
    var svgcheck = document.querySelectorAll("svg *");
    var nodes = tmp.childNodes.length;
    var svg = tmp.getElementsByTagName("svg")[0];

    var svgString = getSVGString(svg);
    debugger;
    document.getElementById("ImgFile").value = svgString;
    svgString2Image(svgString, 2 * 300, 2 * 300, 'png', save);

}
function save(dataBlob, filesize) {
    debugger;
    // var imgFile = document.getElementById("ImgFile");
    //  imgFile.value = dataBlob;
    /*var urlCreator = window.URL || window.webkitURL;
    var imageUrl = urlCreator.createObjectURL(dataBlob);
    document.querySelector("#ImgFile").src = imageUrl;*/
    ///document.querySelector("#ImgFile").value = JSON.stringify({ data: dataBlob });

    saveAs(dataBlob, 'AVISTEDChart.png'); // FileSaver.js function

}
// Below are the functions that handle actual exporting:
// getSVGString ( svgNode ) and svgString2Image( svgString, width, height, format, callback )
function getSVGString(svgNode) {
    debugger;
    svgNode.setAttribute('xlink', 'http://www.w3.org/1999/xlink');
    var cssStyleText = getCSSStyles(svgNode);
    appendCSS(cssStyleText, svgNode);

    var serializer = new XMLSerializer();
    var svgString = serializer.serializeToString(svgNode);
    svgString = svgString.replace(/(\w+)?:?xlink=/g, 'xmlns:xlink='); // Fix root xlink without namespace
    svgString = svgString.replace(/NS\d+:href/g, 'xlink:href'); // Safari NS namespace fix

    return svgString;

    function getCSSStyles(parentElement) {
        var selectorTextArr = [];
        debugger;
        // Add Parent element Id and Classes to the list
        selectorTextArr.push('#' + parentElement.id);
        for (var c = 0; c < parentElement.classList.length; c++)
            if (!contains('.' + parentElement.classList[c], selectorTextArr))
                selectorTextArr.push('.' + parentElement.classList[c]);

        // Add Children element Ids and Classes to the list
        var nodes = parentElement.getElementsByTagName("*");
        for (var i = 0; i < nodes.length; i++) {
            var id = nodes[i].id;
            if (!contains('#' + id, selectorTextArr))
                selectorTextArr.push('#' + id);

            var classes = nodes[i].classList;
            for (var c = 0; c < classes.length; c++)
                if (!contains('.' + classes[c], selectorTextArr))
                    selectorTextArr.push('.' + classes[c]);
        }

        // Extract CSS Rules
        var extractedCSSText = "";
        for (var i = 0; i < document.styleSheets.length; i++) {
            var s = document.styleSheets[i];

            try {
                if (!s.cssRules) continue;
            } catch (e) {
                if (e.name !== 'SecurityError') throw e; // for Firefox
                continue;
            }

            var cssRules = s.cssRules;
            for (var r = 0; r < cssRules.length; r++) {
                if (contains(cssRules[r].selectorText, selectorTextArr))
                    extractedCSSText += cssRules[r].cssText;
            }
        }


        return extractedCSSText;

        function contains(str, arr) {
            return arr.indexOf(str) === -1 ? false : true;
        }

    }


    function appendCSS(cssText, element) {
        var styleElement = document.createElement("style");
        styleElement.setAttribute("type", "text/css");
        styleElement.innerHTML = cssText;
        var refNode = element.hasChildNodes() ? element.children[0] : null;
        element.insertBefore(styleElement, refNode);
    }
}


function svgString2Image(svgString, width, height, format, callback) {
    debugger;
    var format = format ? format : 'png';

    var imgsrc = 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(svgString))); // Convert SVG string to data URL

    var canvas = document.createElement("canvas");
    var context = canvas.getContext("2d");

    canvas.width = width;
    canvas.height = height;

    var image = new Image();
    image.onload = function () {
        context.clearRect(0, 0, width, height);
        context.drawImage(image, 0, 0, width, height);

        canvas.toBlob(function (blob) {
            var filesize = Math.round(blob.length / 1024) + ' KB';
            if (callback) callback(blob, filesize);
        });


    };
    // document.getElementById("ImgFile").value = imgsrc;
    image.src = imgsrc;
}
function IndexLoad() {
    debugger; 
    var type = document.getElementById("Type").value;
    var parameter = $('#parameter').val();
    //var parms = "";
    var compare = $('#compare').val();

    var months = $('#month').val();
    var years = $('#year').val();
    var newParams= [];
    var yearly=0;
    var monthly=0;
    var i, j = 0, k;
    
    
    if (months !== null)
    {
    if (months.length > 1) {
        
        for (i = 0; i < 2; i++) {
            for (k = 0; k < parameter.length; k++) {
                newParams[j++] = parameter[k] + "-" + months[i];
            }
        }
        monthly =1;
    }
    if (years !== null  ) {
        if (months.length === 1 && years.length === 1) {
            monthly = 1;
            for (k = 0; k < parameter.length; k++) {
                newParams[j++] = parameter[k] + "-" + months[0];
            }
        }
    }
   /* else if (months.length === 1 && months[0]!=="")
    {
        for (i = 0; i <= parameter.length; i++) {
            newParams[j++] = parameter[i]  + "-" + months[0] ;
            

        }
        monthly=1;
    }*/
    }
    else if (years !== null && years.length > 1) {
        for (i = 0; i < 2; i++) {
            for (k = 0; k < parameter.length; k++) {
            newParams[j++] = parameter[k] + "-" + years[i];
        }
        }
        yearly=1;
    }
    
 
    if (type !== null && parameter.length>0) {
        switch (type) {
            case "Line":
                if (j !== 0)
                    compareVariablesLine(newParams, monthly, yearly);
                else
                    compareVariablesLine(parameter, monthly, yearly);
                break;
            case "Area":
                if (j !== 0)
                    compareVariablesArea(newParams, monthly, yearly);
                else
                    compareVariablesArea(parameter, monthly, yearly);
                break;           
            case "Bar":
               
                if (j !== 0)
                    compareBar(newParams, monthly, yearly);
                else
                    compareBar(parameter, monthly, yearly);
                    break;
            case "Scatter":
                if (j !== 0)
                    compareScatter(newParams, monthly, yearly);
                    else
                    compareScatter(parameter, monthly, yearly);
                break;
        }
    }

}

//compare Line
function compareVariablesLine(paramArray, monthly, yearly) {
    debugger;
    var dateformat = '%Y-%m-%d';
    var tickformat = '%Y-%m-%d';
    if (monthly === 1) {
        dateformat = '%d';
        tickformat = '%d';
    } else if (yearly===1) {
        dateformat = '%m-%d';
        tickformat = '%m-%d';
    }
    var chart = c3.generate({
        data: {
            
            keys:{
                x: 'date',
                value: paramArray,
                },
            xFormat: dateformat,
            url: baseURL+'Visualize/Load',
            mimeType: 'json'           
        },
        axis: {
            x: {
                type: 'timeseries',
                tick: {
                    format: tickformat
                }
            }
        }
    });
}

//compare Area
function compareVariablesArea(paramArray, monthly, yearly) {
    var dateformat = '%Y-%m-%d';
    var tickformat = '%Y-%m-%d';
    if (monthly === 1) {
        dateformat = '%d';
        tickformat = '%d';
    } else if (yearly === 1) {
        dateformat = '%m-%d';
        tickformat = '%m-%d';
    }
    var chart = c3.generate({
        data: {
            keys:{
                x: 'date',
                value: paramArray,
            },
            xFormat: dateformat,
            url: baseURL +'Visualize/Load',
            mimeType: 'json',
            type: 'area'
        },
        axis: {
            x: {
                type: 'timeseries',
                tick: {
                    format: tickformat
                }
            }
        }
    });
}

//compare bar
function compareBar(paramArray, monthly, yearly) {
    var dateformat = '%Y';
    var tickformat = '%Y';
    if (monthly === 1) {
        dateformat = '%d';
        tickformat = '%d';
    } else if (yearly === 1) {
        dateformat = '%m';
        tickformat = '%m';
    }
    var chart = c3.generate({
        data: {
            keys: {
                x: 'date',
                value: paramArray,
            },
            xFormat: '%Y',
            url: baseURL +'Visualize/Load',
            mimeType: 'json',
            type: 'bar'
        },
        bar: {
            width: {
                ratio: 0.5 // this makes bar width 50% of length between ticks
            }
            // or
            //width: 100 // this makes bar width 100px
        }
    });
}

//compare Scatter
function compareScatter(paramArray, monthly, yearly) {
    var dateformat = '%Y-%m-%d';
    var tickformat = '%Y-%m-%d';
    if (monthly === 1) {
        dateformat = '%d';
        tickformat = '%d';
    } else if (yearly === 1) {
        dateformat = '%m-%d';
        tickformat = '%m-%d';
    }

    var chart = c3.generate({
        data: {
            keys: {
                x: 'date',
                value: paramArray,
            },
            xFormat: dateformat,
            url: baseURL +'Visualize/Load',
            mimeType: 'json',
            type: 'scatter'
        },
        axis: {
            x: {
                type: 'timeseries',
                tick: {
                    format: tickformat
                }
            }
        }


    });
}

