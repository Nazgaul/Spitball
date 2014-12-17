app.directive('homeScroll',
   ['$window', '$analytics',
  function ($window, $analytics) {
      "use strict";
      return {
          restrict: "A",
          link: function () {
              var scrolled, scrolledBottom, $win = angular.element($window), $doc = angular.element(document);
              $win.on('scroll', function (e) {
                  if (!scrolled) {
                      $analytics.eventTrack('Scroll', {
                          category: 'Home page'
                      });
                      scrolled = true;

                      document.getElementById('unions').className = 'unions';
                  }

                  if (!scrolledBottom) {
                      if ($win.scrollTop() + $win.height() == $doc.height()) {
                          $analytics.eventTrack('Scroll bottom', {
                              category: 'Home page'
                          });
                          scrolledBottom = true;
                      }
                  }
              });
          }
      };
  }
   ]);