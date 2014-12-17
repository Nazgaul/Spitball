app.directive('homeVideo',
    ['$analytics', '$animate',
        function ($analytics, $animate) {
            "use strict";
            return {
                restrict: "A",
                link: function (scope, elem, attrs) {
                    var $playButton = elem,
                        $videoWrapper = angular.element('#videoWpr'),
                        $homeVideo = angular.element('#homeVideo');

                    $playButton.click(function () {

                        $videoWrapper.show();

                        setTimeout(function () {
                            $playButton.addClass('open');
                        }, 0);
                        
                        setTimeout(function () {
                            homeVideo.play();
                        }, 600);

                    });

                    $videoWrapper.on('click', function (e) {
                        if (e.target.id === 'homeVideo') {
                            return;
                        }
                        $videoWrapper.hide();

                        $homeVideo[0].pause();
                        $homeVideo[0].currentTime = 0;

                        setTimeout(function () {
                            $playButton.removeClass('open');
                        }, 0);
                    });
                }
            };
        }
    ]);
