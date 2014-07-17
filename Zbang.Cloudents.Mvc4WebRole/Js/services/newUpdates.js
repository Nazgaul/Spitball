
app.factory('sNewUpdates', [
    '$http',
    'sUserDetails',
function ($http, sUserDetails) {

    var updates = {};
    var response = {
        getBoxUpdates: function (boxId) {
            boxId = parseInt(boxId, 10);
            var count = 0;

            if (!updates[boxId]) {
                return '';
            }

            for (var key in updates[boxId]) {
                count += updates[boxId][key].length;
            }

            return count > 0 ? count : '';

        },
        isNew: function (boxId, type, id) {
            id = parseInt(id, 10);

            if (updates[boxId] && updates[boxId][type]) {
                return updates[boxId][type].indexOf(id) > -1 ? true : false;
            }

            return false;

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
            $http.post('/Box/DeleteUpdates/', { boxId: boxId }).success(function () {                
            });
        },
        loadUpdates: loadUpdates
    };

    return response;

    function loadUpdates() {
        var update;
        $http.get('/User/Updates/').success(function (response2) {
            var data = response2.payload;
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
        });

        function addUpdate(array, id) {
            id = parseInt(id, 10);
            if (array.indexOf(id) === -1) {
                array.push(id);
            }
        }
    }
}
]);
