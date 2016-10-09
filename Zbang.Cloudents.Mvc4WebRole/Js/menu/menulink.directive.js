(function () {
    'use strict';

    angular.module('app')
        .directive('menuLink', function () {
            return {
                scope: {
                    section: '='
                },
                templateUrl: 'partials/menu-link.tmpl.html',
                link: function ($scope, $element) {
                    var controller = $element.parent().controller();

                    $scope.focusSection = function () {
                        // set flag to be used later when
                        // $locationChangeSuccess calls openPage()
                        controller.autoFocusContent = true;
                    };
                    $scope.isSectionSelected = function (section) {
                        return controller.isSectionSelected(section);
                    };
                }
            };
        });
})();
