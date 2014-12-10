app.factory('sGmfcnHandler',
    [
    function () {
        "use strict";

        var pointsTable = {
            answer: 10,
            question: 5,
            itemUpload: 10,
            shareFb: 5,
            quiz: 30
        };

        var popupCallback,
            counterCallbacks = [];

        return {
            addPoints: function (data) {
                var points = pointsTable[data.type];

                if (!points) {
                    return;
                }

                if (data.type === 'itemUpload') {
                    points *= data.amount;
                }

                popupCallback(points).then(function () {
                    _.forEach(counterCallbacks, function (callback) {
                        callback(points);
                    });
                });
            },
            registerPopup: function (callback) {
                popupCallback = callback;
            },
            registerCounter: function (callback) {          
                counterCallbacks.push(callback);
            }
        };
    }
    ]);
