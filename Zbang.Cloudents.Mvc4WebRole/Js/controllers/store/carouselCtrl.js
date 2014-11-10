
app.controller('CarouselCtrl',
    ['$scope', '$timeout','$analytics',
    function ($scope, $timeout, $analytics) {
        "use strict";
        var currentTimeout,
            transition,
            interval = 3500;

        $scope.params = {
            currentIndex: 0
        };

        $scope.select = function (index) {
            if (transition) {
                return;
            }
            transition = true;
            if (index < $scope.params.currentIndex) {
                $scope.params.reverse = true;
            } else {
                $scope.params.reverse = false;
            }
            $scope.params.currentIndex = index;

            restartTimer();

            $timeout(function () {
                transition = false;
            }, 500);
            $analytics.eventTrack('Store Carousel', {
                category: 'Select'                
            });
        };

        $scope.next = function () {
            var newIndex = ($scope.params.currentIndex + 1) % $scope.params.slidesLength;
            $scope.select(newIndex);
            $scope.params.reverse = false;
            $analytics.eventTrack('Store Carousel', {
                category: 'Next'
            });

        };

        $scope.prev = function () {
            var newIndex = $scope.params.currentIndex - 1 < 0 ? $scope.params.slidesLength - 1 : $scope.params.currentIndex - 1;
            $scope.select(newIndex);
            $analytics.eventTrack('Store Carousel', {
                category: 'Previous'
            });
        };

        $scope.isActive = function (index) {
            return $scope.params.currentIndex === index;
        }

        restartTimer();

        function restartTimer() {
            resetTimer();
            currentTimeout = $timeout(timerFn, interval);
        }

        function resetTimer() {
            if (currentTimeout) {
                $timeout.cancel(currentTimeout);
                currentTimeout = null;
            }
        }

        function timerFn() {
            //if (isPlaying) {
            $scope.params.reverse = false;

            $scope.next();
            restartTimer();
            //} else {
            //    $scope.pause();
            //}
        }
    }]
);
