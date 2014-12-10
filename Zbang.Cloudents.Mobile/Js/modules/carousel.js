"use strict";
angular.module('Carousel', []).
    controller('CarouselController', ['$scope', '$timeout', function ($scope, $timeout) {
        var self = this,
        slides = self.slides = $scope.slides = [],
        currentIndex = -1,
        currentTimeout, isPlaying;

        self.currentSlide = null;
        var destroyed = false;

        $self.select = $scope.select = function (nextSlide, direction) {
            var nextIndex = slides.indexOf(nextSlide);
            //Decide direction if it's not given
            if (direction === undefined) {
                direction = nextIndex > currentIndex ? 'next' : 'prev';
            }

            if (nextSlide && nextSlide !== self.currentSlide) {
                //if ($scope.$currentTransition) {
                //    $scope.$currentTransition.cancel();
                //    //Timeout so ng-class in template has time to fix classes for finished slide
                //    $timeout(goNext);
                //} else {
                goNext();
                //}
            }

            function goNext() {
                if (destroyed) {
                    return false;
                }


            }
        }

        $scope.$on('$destroy', function () {
            destroyed = true;
        });


        self.addSlide = function (scope, element) {
            slide.$element = element;
            slides.push(slide);
            //if this is the first slide or the slide is set to active, select it
            if (slides.length === 1 || slide.active) {
                self.select(slides[slides.length - 1]);
                if (slides.length == 1) {
                    $scope.play();
                }
            } else {
                slide.active = false;
            }
        };


        self.removeSlide = function (scope) {
            var index = slides.indexOf(slide);
            slides.splice(index, 1);
            if (slides.length > 0 && slide.active) {
                if (index >= slides.length) {
                    self.select(slides[index - 1]);
                } else {
                    self.select(slides[index]);
                }
            } else if (currentIndex > index) {
                currentIndex--;
            }
        };

        $scope.next = function () {
            var newIndex = (currentIndex + 1) % slides.length;
            return self.select(slides[newIndex], 'next');

        };

        $scope.prev = function () {
            var newIndex = currentIndex - 1 < 0 ? slides.length - 1 : currentIndex - 1;

            //Prevent this user-triggered transition from occurring if there is already one in progress
            //if (!$scope.$currentTransition) {
            return self.select(slides[newIndex], 'prev');
            //}
        };

        $scope.isActive = function (slide) {
            return self.currentSlide === slide;
        };

        self.indexOfSlide = function (slide) {
            return slides.indexOf(slide);
        };

        $scope.$watch('interval', restartTimer);
        $scope.$on('$destroy', resetTimer);

        function restartTimer() {
            resetTimer();
            var interval = +$scope.interval;
            if (!isNaN(interval) && interval >= 0) {
                currentTimeout = $timeout(timerFn, interval);
            }
        }

        function resetTimer() {
            if (currentTimeout) {
                $timeout.cancel(currentTimeout);
                currentTimeout = null;
            }
        }

        function timerFn() {
            if (isPlaying) {
                $scope.next();
                restartTimer();
            } else {
                $scope.pause();
            }
        }

        $scope.play = function () {
            if (!isPlaying) {
                isPlaying = true;
                restartTimer();
            }
        };
        $scope.pause = function () {
            if (!$scope.noPause) {
                isPlaying = false;
                resetTimer();
            }
        };

    }]).
    directive('carousel', [function () {
        return {
            restrict: 'EA',
            transclude: true,
            replace: true,
            controller: 'CarouselController',
            templateUrl: 'carousel.html',
            require: 'carousel',
            scope: {
                interval: '=',
                noTransition: '=',
                noPause: '='
            }
        };
    }]).
    directive('slide', [function () {
        return {
            require: '^carousel',
            restrict: 'EA',
            transclude: true,
            replace: true,
            link: function (scope, element, attrs, carouselCtrl) {
                carouselCtrl.addSlide(scope, element);

                scope.$on('$destroy', function () {
                    carouselCtrl.removeSlide(scope);
                });

                scope.$watch('active', function (active) {
                    if (active) {
                        carouselCtrl.select(scope);
                    }
                });

            }
        };
    }]).run(['$templateCache', function ($templateCache) {

        $templateCache.put("carousel.html",
      //"<div ng-mouseenter=\"pause()\" ng-mouseleave=\"play()\" class=\"carousel\" ng-swipe-right=\"prev()\" ng-swipe-left=\"next()\">\n" +
      "<div class=\"carousel\">\n" +
      "    <ol class=\"carousel-indicators\" ng-show=\"slides.length > 1\">\n" +
      "        <li ng-repeat=\"slide in slides track by $index\" ng-class=\"{active: isActive(slide)}\" ng-click=\"select(slide)\"></li>\n" +
      "    </ol>\n" +
      "    <div class=\"carousel-inner\" ng-transclude></div>\n" +
      //"    <a class=\"left carousel-control\" ng-click=\"prev()\" ng-show=\"slides.length > 1\"><span class=\"glyphicon glyphicon-chevron-left\"></span></a>\n" +
      //"    <a class=\"right carousel-control\" ng-click=\"next()\" ng-show=\"slides.length > 1\"><span class=\"glyphicon glyphicon-chevron-right\"></span></a>\n" +
      "</div>\n" +
      "");

        $templateCache.put("template/carousel/slide.html",
      "<div ng-class=\"{\n" +
      "    'active': leaving || (active && !entering),\n" +
      "    'prev': (next || active) && direction=='prev',\n" +
      "    'next': (next || active) && direction=='next',\n" +
      "    'right': direction=='prev',\n" +
      "    'left': direction=='next'\n" +
      //"  }\" class=\"item text-center\" ng-transclude></div>\n" +
      "  }\" ng-transclude></div>\n" +
      "");
    }]);
