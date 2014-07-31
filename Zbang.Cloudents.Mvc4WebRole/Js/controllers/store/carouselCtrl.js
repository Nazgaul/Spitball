app.controller('CarouselCtrl',
    ['$scope', '$timeout',
    function ($scope, $timeout) {
        var currentTimeout,
            interval = 7000;

        $scope.params= {
            currentIndex : 0
        };

        $scope.select = function (index) {
            $scope.params.currentIndex = index;
            restartTimer();
        };

        $scope.next = function () {
            var newIndex = ($scope.params.currentIndex + 1) % $scope.params.slidesLength;
            $scope.select(newIndex);
        };

        $scope.prev = function () {
            var newIndex = $scope.params.currentIndex - 1 < 0 ? $scope.params.slidesLength - 1 : $scope.params.currentIndex - 1;
            $scope.select(newIndex);
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
                $scope.next();
                restartTimer();
            //} else {
            //    $scope.pause();
            //}
        }
    }]
);
