(function (cd, pubsub, ko, dataContext, $, analytics) {
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
            shareFb: 5
        };


    pubsub.subscribe('addPoints', function (type) {
        var points = pointsTable[type];
        if (!points) {
            return;
        }

        appendDigits(points);
        applyAnimation();
        
        
    });

    
    function appendDigits(points) {

        var strPoints = points.toString();


        function getDigit(value) {
            return '<span class="ptDigit boldFont">' + value + '</span>';
        }
    }

    function applyAnimation(points) {
        pointBox.classList.add('ptsAnim');

        setTimeout(function () {
            pointBox.classList.remove('ptsAnim');
        }, 3500);

        setTimeout(changeScore, 4500);

        function changeScore() {
            var currentScore = parseInt(usrPoints.textContent,10);
            var numAnim = new countUp(currentScore, currentScore+points, 1);

            numAnim.start(function (score) {
                if (!score) {
                    return;
                }
                usrPoints.textContent = score;
            });
        }
    }
    

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);