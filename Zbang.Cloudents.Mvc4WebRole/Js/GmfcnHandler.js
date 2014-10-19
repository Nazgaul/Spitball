(function (cd, pubsub, $) {
    "use strict";
    if (window.scriptLoaded.isLoaded('gmfcn')) {
        return;
    }

    var eById = document.getElementById.bind(document),
        pointBox = eById('ptsPopup'),
        usrPoints = eById('userPts'),
        pointsTable = {
            answer: 10,
            question: 5,
            itemUpload: 10,
            shareFb: 5,
            quiz: 30
        };



    pubsub.subscribe('addPoints', function (data) {
        var points = pointsTable[data.type];

        if (!points) {
            return;
        }

        if (data.type === 'itemUpload') {
            points *= data.amount;
        }

        startAnimation(points);
    });

    function startAnimation(points) {
        appendDigits(points);
        applyAnimation(points);
    }

    function appendDigits(points) {

        var strPoints = points.toString(),
            resultHtml = '';

        for (var i = 0, l = strPoints.length; i < l; i++) {
            resultHtml += getDigit(strPoints[i]);
        }

        var child = pointBox.firstElementChild;
        child.innerHTML = '';
        child.insertAdjacentHTML('beforeend', resultHtml);

        function getDigit(value) {
            return '<span class="ptDigit boldFont">' + value + '</span>';
        }
    }

    function applyAnimation(points) {
        $(pointBox).addClass('ptsAnim');

        setTimeout(function () {
            $(pointBox).removeClass('ptsAnim');
            changeScore();
        }, 3500);

        function changeScore() {
            var currentScore = parseInt(usrPoints.textContent, 10);
            var numAnim = new countUp(currentScore, currentScore + points, 1);

            numAnim.start(function (score) {
                if (!score) {
                    return;
                }
                usrPoints.textContent = score;
            });


        }
    }
})(cd, cd.pubsub, jQuery);