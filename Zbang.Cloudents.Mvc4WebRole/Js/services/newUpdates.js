app.factory('sNewUpdates', [
    '$http', '$q',
    'sBox',
    'sUserDetails',
    '$interval',
    '$timeout',
function ($http, $q, sBox, sUserDetails, $interval, $timeout) {
    "use strict";
    var updates = {},
        updatesLoaded = false;
    var response = {
        getBoxUpdates: function (boxId, callback) {
            boxId = parseInt(boxId, 10);


            if (updatesLoaded) {
                boxUpdates();
                return;
            }

            var interval = $interval(function () {
                if (!updatesLoaded) {
                    return;
                }
                $interval.cancel(interval);
                boxUpdates();

            }, 20);

            function boxUpdates() {
                var count = 0;

                if (!updates[boxId]) {
                    callback('');
                }

                for (var key in updates[boxId]) {
                    count += updates[boxId][key].length;
                }

                return callback(count > 0 ? count : '');
            }
        },
        isNew: function (boxId, type, id, callback) {
            id = parseInt(id, 10);


            if (updatesLoaded) {
                isNew();
                return;
            }
            var interval = $interval(function () {
                if (!updatesLoaded) {
                    return;
                }
                $interval.cancel(interval);
                isNew();
            }, 20);

            function isNew() {
                if (updates[boxId] && updates[boxId][type]) {
                    callback(updates[boxId][type].indexOf(id) > -1 ? true : false);
                }
            }

        },
        getUpdatesCount: function (boxId, callback) {
            if (updatesLoaded) {
                updatesCount();
                return;
            }

            var interval = $interval(function () {
                if (!updatesLoaded) {
                    return;
                }
                $interval.cancel(interval);
                updatesCount();
            }, 20);

            function updatesCount() {
                var updateCount = {};

                if (!updates[boxId]) {
                    updateCount = {
                        feed: 0,
                        items: 0,
                        quizzes: 0
                    }
                    callback(updateCount);
                    return;
                }
                updateCount = {
                    feed: updates[boxId].questions.length + updates[boxId].answers.length,
                    items: updates[boxId].items.length,
                    quizzes: updates[boxId].quizzes.length
                };

                callback(updateCount);
            }

        },
        setOld: function (boxId, type, id) {
            id = parseInt(id, 10);

            if (updates[boxId] && updates[boxId][type]) {
                var index = updates[boxId][type].indexOf(id);
                if (index > -1) {
                    updates[boxId][type].splice(index, 1);
                }
            }
        },
        removeUpdates: function (boxId) {
            boxId = parseInt(boxId, 10);
            sBox.deleteUpdates({
                boxId: boxId
            }).then(function () { });
        },
        loadUpdates: loadUpdates
    };

    return response;

    function loadUpdates() {
        var defer = $q.defer();
        if (!sUserDetails.isAuthenticated()) {
            $timeout(function () {
                defer.resolve();
            });

            return defer.promise;
        }

        var update;
        $http.get('/User/Updates/').success(function (response2) {
            var data = response2.payload;
            if (!data) {
                defer.resolve();
                return;
            }
            for (var i = 0, l = data.length ; i < l; i++) {
                update = data[i];
                var boxId = parseInt(update.boxId);
                if (!updates[update.boxId]) {
                    updates[boxId] = {};
                    updates[boxId].items = [];
                    updates[boxId].quizzes = [];
                    updates[boxId].questions = [];
                    updates[boxId].answers = [];

                }

                if (update.itemId) {
                    addUpdate(updates[boxId].items, update.itemId);
                    continue;
                }
                if (update.quizId) {
                    addUpdate(updates[boxId].quizzes, update.quizId);
                    continue;
                }
                if (update.questionId) {
                    addUpdate(updates[boxId].questions, update.questionId);
                    continue;
                }
                if (update.answerId) {
                    addUpdate(updates[boxId].answers, update.questionId);
                }
            }
            updatesLoaded = true;

            defer.resolve();
        });

        return defer.promise;

        function addUpdate(array, id) {
            id = parseInt(id, 10);
            if (array.indexOf(id) === -1) {
                array.push(id);
            }
        }
    }
}
]);
