app.directive('loadSpinner',
    [

    function () {
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
                var spinner;            
                scope.$watch(attrs.loadSpinner, function (newValue) {
                    var loader = attrs.loadSpinnerType ? loaders[attrs.loadSpinnerType] : loaders.init;
                    if (newValue) {
                        spinner = angular.element(Modernizr.cssanimations ? loader.css : loader.img);
                        elem.append(spinner);
                        return;
                    }

                    if (spinner) {
                        spinner.remove();
                    }

                });
            }
        };
    }
    ]);
