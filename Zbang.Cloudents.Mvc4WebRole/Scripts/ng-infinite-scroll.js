/* ng-infinite-scroll - v1.0.0 - 2013-02-23 */
var mod;

mod = angular.module('infinite-scroll', []);

mod.directive('infiniteScroll', [
  '$rootScope', '$window', '$timeout', function ($rootScope, $window, $timeout) {
      return {
          link: function (scope, elem, attrs) {
              var checkWhenEnabled, handler, scrollDistance, scrollEnabled;
              var $scrollElem = attrs.infiniteScrollElement ? angular.element(elem) : angular.element($window);
              scrollDistance = 0;
              if (attrs.infiniteScrollDistance != null) {
                  scope.$watch(attrs.infiniteScrollDistance, function (value) {
                      return scrollDistance = parseInt(value, 10);
                  });
              }
              scrollEnabled = true;
              checkWhenEnabled = false;
              if (attrs.infiniteScrollDisabled != null) {
                  scope.$watch(attrs.infiniteScrollDisabled, function (value) {
                      scrollEnabled = !value;
                      if (scrollEnabled && checkWhenEnabled) {
                          checkWhenEnabled = false;
                          return handler();
                      }
                  });
              }
              handler = function () {
                  var elementBottom, remaining, shouldScroll, scrollElemBottom;
                  //scrollElemBottom = $scrollElem.height() + $scrollElem.scrollTop();
                  //elementBottom = elem.offset().top + elem.height();
                  if (attrs.infiniteScrollElement) {

                      remaining = $scrollElem[0].scrollHeight - $scrollElem.scrollTop();
                  } else {
                      remaining = $(document).height() - ($scrollElem.scrollTop() +  $scrollElem.height());
                  }

                  //if (($(window).innerHeight() + $(window).scrollTop()) >= $("body").height()) {



                  shouldScroll = remaining <= $scrollElem.height() * scrollDistance;
                  if (shouldScroll && scrollEnabled) {
                      if ($rootScope.$$phase) {
                          return scope.$eval(attrs.infiniteScroll);
                      } else {
                          return scope.$apply(attrs.infiniteScroll);
                      }
                  } else if (shouldScroll) {
                      return checkWhenEnabled = true;
                  }
              };
              $scrollElem.on('scroll', handler);
              scope.$on('$destroy', function () {
                  return $scrollElem.off('scroll', handler);
              });
              return $timeout((function () {
                  if (attrs.infiniteScrollImmediateCheck) {
                      if (scope.$eval(attrs.infiniteScrollImmediateCheck)) {
                          return handler();
                      }
                  } else {
                      return handler();
                  }
              }), 0);
          }
      };
  }
]);
