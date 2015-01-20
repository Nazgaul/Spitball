app.directive('itemScroll',
   ['$window', '$analytics', 'sUserDetails', 'sLogin', 'sModal',
  function ($window, $analytics, sUserDetails, sLogin, sModal) {
      "use strict";
      return {
          restrict: "A",
          link: function () {
              var scrolled, scrolledBottom, $win = angular.element($window), $doc = angular.element(document);

              if (sUserDetails.isAuthenticated()) {
                  return;
              }

              $win.on('scroll', scroll);

              scope.$on('$destroy', function () {
                  $win.off('scroll',scroll);
              });

              function scroll(e) {
                  if (!scrolled) {

                      if ($win.scrollTop() < ($win.height() * 1.5) / 2) {
                          return;

                      };

                      $analytics.eventTrack('Blur register', {
                          category: 'Item'
                      });

                      sModal.open('itemReg', {
                          callback: {
                              close: function () {
                                  sLogin.register();
                              }
                          }
                      });                      
                      scrolled = true;

                  }

              });
          }
      };
  }
   ]);