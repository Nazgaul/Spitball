module app {
    "use strict";

    class FlipClock implements angular.IDirective {
        restrict = "E";

        link = (scope: angular.IScope, element: JQuery) => {
            var d = new Date(2016, 11, 16);
            var now = new Date();
            element["FlipClock"]((d.getTime() - new Date().getTime())/1000, {
                clockFace: 'DailyCounter',
                countdown: true
            });
        };

        static factory(): angular.IDirectiveFactory {

            const directive = () => {
                return new FlipClock();
            };
            // directive['$inject'] = ['$mdMedia', "$anchorScroll", "$timeout"];
            return directive;
        }
    }

    angular
        .module("app.dashboard")
        .directive("flipClock", FlipClock.factory());
}