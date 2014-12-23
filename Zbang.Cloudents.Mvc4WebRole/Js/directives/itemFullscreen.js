app.directive('fullScreen', [
  function () {
      return {
          restrict: 'A',
          link: {
              pre: function (scope, element, attrs) {
                  element.bind('click', function (e) {
                      var video = angular.element('video')[0];
                      if (!video) {
                          return;
                      }

                      e.stopImmediatePropagation();
                      e.preventDefault();
                      
                      if (video.requestFullscreen) {
                          video.requestFullscreen();
                      } else if (video.msRequestFullscreen) {
                          video.msRequestFullscreen();
                      } else if (video.mozRequestFullScreen) {
                          video.mozRequestFullScreen();
                      } else if (video.webkitRequestFullscreen) {
                          video.webkitRequestFullscreen();
                      }

                  });
              }
          }
      }
  }
]);