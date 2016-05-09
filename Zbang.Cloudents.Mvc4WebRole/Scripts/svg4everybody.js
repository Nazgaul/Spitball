//!function (root, factory) {
//    "function" == typeof define && define.amd ? // AMD. Register as an anonymous module unless amdModuleId is set
//    define([], function () {
//        return root.svg4everybody = factory();
//    }) : "object" == typeof exports ? module.exports = factory() : root.svg4everybody = factory();
//}(this, function () {
//    /*! svg4everybody v2.0.3 | github.com/jonathantneal/svg4everybody */
//    function embed(svg, target) {
//        // if the target exists
//        if (target) {
//            // create a document fragment to hold the contents of the target
//            var fragment = document.createDocumentFragment(), viewBox = !svg.getAttribute("viewBox") && target.getAttribute("viewBox");
//            // conditionally set the viewBox on the svg
//            viewBox && svg.setAttribute("viewBox", viewBox);
//            // copy the contents of the clone into the fragment
//            for (// clone the target
//            var clone = target.cloneNode(!0) ; clone.childNodes.length;) {
//                fragment.appendChild(clone.firstChild);
//            }
//            // append the fragment into the svg
//            svg.appendChild(fragment);
//        }
//    }
//    function loadreadystatechange(xhr) {
//        // listen to changes in the request
//        xhr.onreadystatechange = function () {
//            // if the request is ready
//            if (4 === xhr.readyState) {
//                // get the cached html document
//                var cachedDocument = xhr._cachedDocument;
//                // ensure the cached html document based on the xhr response
//                cachedDocument || (cachedDocument = xhr._cachedDocument = document.implementation.createHTMLDocument(""),
//                cachedDocument.body.innerHTML = xhr.responseText, xhr._cachedTarget = {}), // clear the xhr embeds list and embed each item
//                xhr._embeds.splice(0).map(function (item) {
//                    // get the cached target
//                    var target = xhr._cachedTarget[item.id];
//                    // ensure the cached target
//                    target || (target = xhr._cachedTarget[item.id] = cachedDocument.getElementById(item.id)),
//                    // embed the target into the svg
//                    embed(item.svg, target);
//                });
//            }
//        }, // test the ready state change immediately
//        xhr.onreadystatechange();
//    }
//    function svg4everybody(rawopts) {
//        function oninterval() {
//            // while the index exists in the live <use> collection
//            //for (// get the cached <use> index
//            //var index = 0; index < uses.length;) {
//            //    // get the current <use>
//            //    var use = uses[index], svg = use.parentNode;
//            //    if (svg && /svg/i.test(svg.nodeName)) {
//            //        var src = use.getAttribute("xlink:href");
//            //        if (polyfill && (!opts.validate || opts.validate(src, svg, use))) {
//            //            // remove the <use> element
//            //            svg.removeChild(use);
//            //            // parse the src and get the url and id
//            //            var srcSplit = src.split("#"), url = srcSplit.shift(), id = srcSplit.join("#");
//            //            // if the link is external
//            //            if (url.length) {
//            //                // get the cached xhr request
//            //                var xhr = requests[url];
//            //                // ensure the xhr request exists
//            //                xhr || (xhr = requests[url] = new XMLHttpRequest(), xhr.open("GET", url), xhr.send(),
//            //                xhr._embeds = []), // add the svg and id as an item to the xhr embeds list
//            //                xhr._embeds.push({
//            //                    svg: svg,
//            //                    id: id
//            //                }), // prepare the xhr ready state change event
//            //                loadreadystatechange(xhr);
//            //            } else {
//            //                // embed the local id into the svg
//            //                embed(svg, document.getElementById(id));
//            //            }
//            //        }
//            //    } else {
//            //        // increase the index when the previous value was not "valid"
//            //        ++index;
//            //    }
//            //}

//            for (var use; use = uses[0];) {
//                var svg = use.parentNode;
//                if (svg && /svg/i.test(svg.nodeName)) {
//                    var src = use.getAttribute("xlink:href");
//                    if (polyfill && (!validate || validate(src, svg, use))) {
//                        var url = src.split("#"), url_root = url[0], url_hash = url[1];
//                        if (svg.removeChild(use), url_root.length) {
//                            var xhr = svgCache[url_root] = svgCache[url_root] || new XMLHttpRequest();
//                            xhr.s || (xhr.s = [], xhr.open("GET", url_root), xhr.send()), xhr.s.push([svg, url_hash]),
//                            loadreadystatechange(xhr);
//                        } else {
//                            embed(svg, document.getElementById(url_hash));
//                        }
//                    }
//                }
//            }
//            // continue the interval
//            requestAnimationFrame(oninterval, 67);
//        }
//        var polyfill, opts = Object(rawopts), newerIEUA = /\bTrident\/[567]\b|\bMSIE (?:9|10)\.0\b/, webkitUA = /\bAppleWebKit\/(\d+)\b/, olderEdgeUA = /\bEdge\/12\.(\d+)\b/;
//        polyfill = "polyfill" in opts ? opts.polyfill : newerIEUA.test(navigator.userAgent) || (navigator.userAgent.match(olderEdgeUA) || [])[1] < 10547 || (navigator.userAgent.match(webkitUA) || [])[1] < 537;
//        // create xhr requests object
//        var requests = {}, requestAnimationFrame = window.requestAnimationFrame || setTimeout, uses = document.getElementsByTagName("use");
//        // conditionally start the interval if the polyfill is active
//        polyfill && oninterval();
//    }
//    return svg4everybody;
//});
//svg4everybody();

!function (root, factory) {
    "function" == typeof define && define.amd ? define([], function () {
        return root.svg4everybody = factory();
    }) : "object" == typeof exports ? module.exports = factory() : root.svg4everybody = factory();
}(this, function () {
    /*! svg4everybody v2.0.0 | github.com/jonathantneal/svg4everybody */
    function embed(svg, g) {
        if (g) {
            var viewBox = !svg.getAttribute("viewBox") && g.getAttribute("viewBox"), fragment = document.createDocumentFragment(), clone = g.cloneNode(!0);
            for (viewBox && svg.setAttribute("viewBox", viewBox) ; clone.childNodes.length;) {
                fragment.appendChild(clone.firstChild);
            }
            svg.appendChild(fragment);
        }
    }
    function loadreadystatechange(xhr) {
        xhr.onreadystatechange = function () {
            if (4 === xhr.readyState) {
                var x = document.createElement("x");
                x.innerHTML = xhr.responseText, xhr.s.splice(0).map(function (array) {
                    embed(array[0], x.querySelector("#" + array[1].replace(/(\W)/g, "\\$1")));
                });
            }
        }, xhr.onreadystatechange();
    }
    function svg4everybody(opts) {
        function oninterval() {
            for (var use; use = uses[0];) {
                var svg = use.parentNode;
                if (svg && /svg/i.test(svg.nodeName)) {
                    var src = use.getAttribute("xlink:href");
                    if (polyfill && (!validate || validate(src, svg, use))) {
                        var url = src.split("#"), url_root = url[0], url_hash = url[1];
                        if (svg.removeChild(use), url_root.length) {
                            var xhr = svgCache[url_root] = svgCache[url_root] || new XMLHttpRequest();
                            xhr.s || (xhr.s = [], xhr.open("GET", url_root), xhr.send()), xhr.s.push([svg, url_hash]),
                            loadreadystatechange(xhr);
                        } else {
                            embed(svg, document.getElementById(url_hash));
                        }
                    }
                }
            }
            requestAnimationFrame(oninterval, 17);
        }
        opts = opts || {};
        var uses = document.getElementsByTagName("use"), polyfill = "polyfill" in opts ? opts.polyfill : /\bEdge\/12\b|\bTrident\/[567]\b|\bVersion\/7.0 Safari\b/.test(navigator.userAgent) || (navigator.userAgent.match(/AppleWebKit\/(\d+)/) || [])[1] < 537, validate = opts.validate, requestAnimationFrame = window.requestAnimationFrame || setTimeout, svgCache = {};
        polyfill && oninterval();
    }
    return svg4everybody;
});
svg4everybody();