(function () {
    'use strict';

    angular.module('app.quiz.stopwatch').directive('stopwatch', stopwatch);

    stopwatch.$inject = ['$interval'];

    function stopwatch($interval) {
        return {
            restrict: 'AE',
            scope: {
                control: '='
            },
            link: link,
            controller: controller,
            controllerAs: 'sw',
            replace: true,
            template: '<div><span>{{sw.getTime() | stopwatch}}</span><button type="button" ng-click="sw.toggle()">Start</button></div>'
        };

        controller.$inject = ['$scope'];

        function controller($scope) {
            var sw = this;

            sw.control = $scope.control || {};

            var totalElapsedMs = 0;
            var elapsedMs = 0;
            //var time;
            var startTime;
            var timerPromise;


            sw.isRunning = false;
            sw.toggle = toggle;
            sw.control.getTime = sw.getTime = getTime;

            function pause() {
                if (timerPromise) {
                    $interval.cancel(timerPromise);
                    timerPromise = undefined;
                    totalElapsedMs += elapsedMs;
                    elapsedMs = 0;
                }
            }

            function start() {
                if (!timerPromise) {
                    startTime = new Date();
                    timerPromise = $interval(function () {
                        var now = new Date();                        
                        elapsedMs = now.getTime() - startTime.getTime();
                        sw.control.currentTime = elapsedMs;
                    }, 900);
                }
            }
            
            function getTime() {                
                return totalElapsedMs + elapsedMs;
            }

            function toggle() {             
                sw.control.isRunning = !sw.control.isRunning;
                sw.control.isRunning ? start() : pause();
            }

        }

        function link(scope, element, attributes, ctrl) {

        }

    }
})();