(function () {
    'use strict';

    angular.module('app')
        //.run([
        //    '$templateCache', function($templateCache) {
        //        $templateCache.put('partials/menu-link.tmpl.html',
        //            '<md-button ng-class="{\'{{section.icon}}\' : true}" \n' +
        //            ' ng-click="focusSection()">\n' +
        //            '  {{section | humanizeDoc}}\n' +
        //            '  <span  class="md-visually-hidden "\n' +
        //            '    ng-if="isSelected()">\n' +
        //            '    current page\n' +
        //            '  </span>\n' +
        //            '</md-button>\n' +
        //            '');
        //    }
        //])
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