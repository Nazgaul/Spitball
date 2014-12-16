app.factory('sGmfcnHandler',
    ['$rootScope', 'sModal', '$angularCacheFactory',
    function ($rootScope, sModal, $angularCacheFactory) {
        "use strict";

        var pointsTable = {            
            answer: 100,
            question: 50,
            itemUpload: 100,
            shareFb: 50,
            quiz: 300,
            register: 500,
            itemComment: 30,
            itemReply: 15
        };

        var popupCallback,
            counterCallbacks = [];


        $rootScope.$on('viewContentLoaded', function () {

            var pointsCache = $angularCacheFactory.get('points') || $angularCacheFactory('points');

            if (!pointsCache) {
                return;
            }

            var totalPoints = 0,
                keys = pointsCache.keys();

            _.forEach(keys, function (key) {
                var points = pointsTable[key];
                if (!_.isUndefined(points)) {
                    totalPoints += points;
                }
            });

            pointsCache.destroy();

            if (!totalPoints) {
                return;
            }
            addPointsPopup(totalPoints);
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
            addPointsPopup: function (type) {
                var points = pointsTable[type];
                addPointsPopup(points);
            },
            registerPopup: function (callback) {
                popupCallback = callback;
            },
            registerCounter: function (callback) {
                counterCallbacks.push(callback);
            }
        };

        function addPointsPopup(points) {
            sModal.open('congrats', {
                data: points
            });            
        }
    }
    ]);
