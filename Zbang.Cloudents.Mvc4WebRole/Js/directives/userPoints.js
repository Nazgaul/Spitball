app.directive('userPoints',
    ['sGmfcnHandler',
    function (sGmfcnHandler) {
        "use strict";
        return {
            restrict: "A",
            scope:false,
            link: function (scope, elem, attrs) {
               


                sGmfcnHandler.registerCounter(count);

                function count(points) {
                    if (isNaN(elem[0].textContent)) {
                        return;
                    }
                    var currentScore = parseInt(elem[0].textContent, 10),
                        numAnim = new countUp(currentScore, currentScore + points, 1);

                    numAnim.start(function (score) {
                        if (!score) {
                            return;
                        }
                        elem[0].textContent = score;
                    });
                }
            }
        };
    }
    ]).
    directive('userPointsPopup',
    ['$q', 'sGmfcnHandler',
    function ($q, sGmfcnHandler) {
        "use strict";
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                sGmfcnHandler.registerPopup(startAnimation);
                var defer;

                function startAnimation(points) {
                    defer = $q.defer();
                    appendDigits(points);
                    applyAnimation();

                    return defer.promise;
                }

                function appendDigits(points) {

                    var strPoints = points.toString(),
                     resultHtml = '';

                    for (var i = 0, l = strPoints.length; i < l; i++) {
                        resultHtml += getDigit(strPoints[i]);
                    }

                    var child = elem[0].firstElementChild;
                    child.innerHTML = '';
                    child.insertAdjacentHTML('beforeend', resultHtml);

                    function getDigit(value) {
                        return '<span class="ptDigit boldFont">' + value + '</span>';
                    }
                }

                function applyAnimation() {
                    elem.addClass('ptsAnim');

                    setTimeout(function () {
                        elem.removeClass('ptsAnim');
                        defer.resolve();
                    }, 3500);

                }
            }
        };
    }
    ]);
