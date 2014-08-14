app.directive('loadSpinner',
    [

    function () {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var cssSpinner = '<div class="smallLoader upLoader"><div class="spinner"></div>',
                    imgSpinner = '<img class="pageLoaderImg upLoader" src="/images/loader2.gif" />',
                    spinner;

                scope.$watch(attrs.loadSpinner, function (newValue) {
                    if (newValue) {
                        spinner = angular.element(Modernizr.cssanimations ? cssSpinner : imgSpinner);
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
