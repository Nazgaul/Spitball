(function () {
    'use strict';
    angular.module('angular-google-adsense', []).
    service('Adsense', [function () {
        this.url = 'https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js';
        this.isAlreadyLoaded = false;
    }]).
    directive('adsense', function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                adClient: '@',
                adSlot: '@',
                inlineStyle: '@',
                adFormat: '@',
                adClass: '@'
            },
            template: '<div class="ads"><ins class="adsbygoogle {{adClass}}" data-ad-client="{{adClient}}" data-ad-slot="{{adSlot}}" style="{{inlineStyle}}" data-ad-format="{{adFormat}}"></ins></div>',
            controller: ['Adsense', '$timeout', '$scope', function (Adsense, $timeout, $scope) {
                if (!Adsense.isAlreadyLoaded) {
                    var s = document.createElement('script');
                    s.type = 'text/javascript';
                    s.src = Adsense.url;
                    s.async = true;
                    document.body.appendChild(s);

                    Adsense.isAlreadyLoaded = true;
                }


                var selector = ".mainAd";
                var statesFullPage = [];
                var statesToHide = ["flashcard","chat", "item", "quiz", "quizCreate", "universityChoose", "settings", "flashcardCreate"];
                $scope.$on("$stateChangeSuccess",
                function (event, toState) {
                    stateToggle(statesToHide, "hide-Ad");
                    stateToggle(statesFullPage, "fullPage");
                    function stateToggle(arr, classToToggle) {
                        if (arr.indexOf(toState.name) > -1 || arr.indexOf(toState.parent) > -1) {
                            $(selector).addClass(classToToggle);
                        } else {
                            $(selector).removeClass(classToToggle);
                        }
                    }
                });


                /**
                 * We need to wrap the call the AdSense in a $apply to update the bindings.
                 * Otherwise, we get a 400 error because AdSense gets literal strings from the directive
                 */
                $timeout(function () {
                    (window.adsbygoogle = window.adsbygoogle || []).push({});
                });
            }]
        };
    });
}());