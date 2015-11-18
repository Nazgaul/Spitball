(function () {
    'use strict';

    angular.module('app.quiz.stopwatch').directive('stopwatch', stopwatch);

    stopwatch.$inject = ['$interval'];

    function stopwatch($interval) {
        var ctrl = controller;

        ctrl.$inject = ['$scope'];

        return {
            restrict: 'AE',
            scope: {
                control: '='
            },
            link: link,
            controller: ctrl,
            controllerAs: 'sw',
            replace: true,
            template: '<span class="stopwatch">{{sw.getTime() | stopwatch}}</span>'
        };

        

        function controller($scope) {
            var sw = this;

            sw.control = $scope.control || {};

            var totalElapsedMs = 0;
            var elapsedMs = 0;
            var startTime;
            var timerPromise;
   

            sw.isRunning = false;            
            sw.control.getTime = sw.getTime = getTime;
            sw.control.start = start;
            sw.control.pause = pause;
            sw.control.reset = reset;
            sw.control.toggle = toggle;
            sw.control.setTime = setTime;
            
            function pause() {
                if (timerPromise) {
                    sw.control.isRunning = false;
                    $interval.cancel(timerPromise);
                    timerPromise = undefined;
                    totalElapsedMs += elapsedMs;
                    elapsedMs = 0;
                }
            }

            function start() {
                if (!timerPromise) {
                    startTime = new Date();
                    sw.control.isRunning = true;
                    timerPromise = $interval(function () {
                        var now = new Date();                        
                        elapsedMs = now.getTime() - startTime.getTime();
                        sw.control.currentTime = getTime();
                    }, 1000);
                }
            }
            
            function setTime(time) {
                var arr = time.split(':'),
                    millis = 0;

                millis = parseInt(arr[2], 10) * 1000;
                millis += parseInt(arr[1], 10) * 60000;
                millis += parseInt(arr[0], 10) * 360000;


                totalElapsedMs = millis;

            }

            function getTime() {                
                return totalElapsedMs + elapsedMs;
            }

            function toggle() {             
                sw.control.isRunning = !sw.control.isRunning;
                sw.control.isRunning ? start() : pause();
            }

            function reset() {
                totalElapsedMs = 0;
                elapsedMs = 0;
            }

        }

        function link(scope, element, attributes, ctrl) {

        }

    }
})();