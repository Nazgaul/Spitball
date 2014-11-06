app.directive('loadSpinner',
    [

    function () {
        "use strict";
        var loaders = {
            init: {
                css: '<div class="smallLoader upLoader"><div class="spinner"></div>',
                img: '<img class="pageLoaderImg upLoader" src="/images/loader2.gif" />',
            },
            page: {
                css: '<div class="pageLoader pageAnim"></div>',
                img: '<img class="pageLoaderImg pageAnim" src="/images/loader1.gif" />'
            }
        };

        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var spinner, timer;
                scope.$watch(attrs.loadSpinner, function (newValue) {

                    var loader = attrs.loadSpinnerType ? loaders[attrs.loadSpinnerType] : loaders.init;
                    if (newValue) {
                        spinner = angular.element(Modernizr.cssanimations ? loader.css : loader.img);
                        var timeout = parseInt(attrs.loadSpinnerTime, 10) || 0;
                        timer = window.setTimeout(function () { elem.append(spinner); }, timeout);
                        return;
                    }

                    if (spinner) {
                        spinner.remove();
                        clearTimeout(timer);
                    }

                });
            }
        };
    }
    ]);
