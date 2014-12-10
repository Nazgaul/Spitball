
app.directive('counter', function () {
    "use strict";
    var lastTime = 0, startVal = 0;
    var vendors = ['webkit', 'moz', 'ms'];
    for (var x = 0; x < vendors.length && !window.requestAnimationFrame; ++x) {
        window.requestAnimationFrame = window[vendors[x] + 'RequestAnimationFrame'];
        window.cancelAnimationFrame =
          window[vendors[x] + 'CancelAnimationFrame'] || window[vendors[x] + 'CancelRequestAnimationFrame'];
    }
    if (!window.requestAnimationFrame) {
        window.requestAnimationFrame = function (callback, element) {
            var currTime = new Date().getTime();
            var timeToCall = Math.max(0, 16 - (currTime - lastTime));
            var id = window.setTimeout(function () { callback(currTime + timeToCall); },
              timeToCall);
            lastTime = currTime + timeToCall;
            return id;
        }
    }
    if (!window.cancelAnimationFrame) {
        window.cancelAnimationFrame = function (id) {
            clearTimeout(id);
        }
    }

    return {
        link: function (scope, elem, attrs) {
            // make sure requestAnimationFrame and cancelAnimationFrame are defined
            // polyfill for browsers without native support
            // by Opera engineer Erik Möller
         
            var options = {
                useEasing: (attrs.counterEasing === "true"),
                duration: angular.isNumber(attrs.counterDuration) || null,
            };

            attrs.$observe('counterTarget', function (newValue) {                
                endVal = attrs.counterTarget ? Number(attrs.counterTarget) : 0;
                if (angular.isNumber(endVal) && endVal > startVal) {
                    start();
                }
                
            });

            var d = elem[0],
             endVal = angular.isNumber(attrs.counterTarget) ? Number(attrs.counterTarget) : 0,
             startTime = null,
             timestamp = null,
             remaining = null,
             frameVal = startVal,
             rAF = null,                         
             duration = angular.isNumber(attrs.counterDuration) ? Number(attrs.counterDuration) * 1000 : 2000

            function easeOutExpo(t, b, c, d) {
                return c * (-Math.pow(2, -10 * t / d) + 1) * 1024 / 1023 + b;
            }

            function count(timestamp) {                
                if (startTime === null) startTime = timestamp;

                timestamp = timestamp;

                var progress = timestamp - startTime;
                remaining = duration - progress;

                // to ease or not to ease
                if (options.useEasing) {
                    frameVal = easeOutExpo(progress, startVal, endVal - startVal, duration);
                } else {
                    frameVal = startVal + (endVal - startVal) * (progress / duration);
                }
                                
                // don't go past endVal since progress can exceed duration in the last frame

                frameVal = (frameVal > endVal) ? endVal : frameVal;


                // format and print value
                print(frameVal);

                // whether to continue
                if (progress < duration) {
                    rAF = requestAnimationFrame(count);
                }
            }

            function start() {
                // make sure values are valid
                if (!isNaN(endVal) && !isNaN(startVal)) {
                    rAF = requestAnimationFrame(count);
                } else {
                    console.log('countUp error: startVal or endVal is not a number');
                    d.innerHTML = '--';
                }
            }

            function print(nStr) {                
                nStr = nStr.toFixed(0).toString();
                var children = elem[0].children;
                for (var i = 0, c = 0, l = children.length; i < l ; i++) {
                    if (l - i > nStr.length) {
                        continue;
                    }
                    children[i].textContent = nStr[c];
                    c++;
                }
            }

            // format startVal on initialization
            print(startVal);

        }
    };
});