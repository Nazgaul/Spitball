app.factory('sGmfcnHandler',
    ['$rootScope', 'sModal', '$angularCacheFactory',
    function ($rootScope, sModal) {
        "use strict";

        var pointsTable = {
            answer: 10,
            question: 5,
            itemUpload: 10,
            shareFb: 5,
            quiz: 30,
            newUser: 500
        };

        var popupCallback,
            counterCallbacks = [];


        $rootScope.$on('viewContentloaded', function () {
            var pointsCache = $angularCacheFactory.get('points');

            if (!pointsCache) {
                return;
            }

            var totalPoints = 0,
                keys = pointsCache.keys();

            _.forEach(keys, function (key) {
                var points = pointsCache[key];
                if (!_.isUndefined(points)) {
                    totalPoints += points;
                }
            });

            cache.destroy();

            if (!totalPoints) {
                return;
            }

            sModal.open('congrats', {
                data: totalPoints
            });

        });

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
