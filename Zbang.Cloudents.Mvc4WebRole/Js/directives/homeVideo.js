app.directive('homeVideo',
    ['$analytics','$animate',
        function ($analytics,$animate) {
            "use strict";

            return {
                restrict: "A",     
                link: function (scope, elem, attrs) {
                    elem.click(function () {

                        mVideo.style.display = 'block';
                        setTimeout(function () {
                            videoWrapper.addClass('open');
                        }, 0);

                        if (homeVideo.readyState) {
                            homeVideo.currentTime = 0;
                        }
                        setTimeout(function () {
                            homeVideo.play();
                        }, 600);

                    });
                }
            };
        }
    ]);
