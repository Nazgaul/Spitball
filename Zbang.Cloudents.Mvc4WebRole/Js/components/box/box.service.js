(function () {
    angular.module('app.box').service('boxService', box);
    box.$inject = ['ajaxService'];

    function box(ajaxservice) {
        var d = this;
        d.getBox = function (boxid) {
            return ajaxservice.get('/box/data/', { id: boxid });
        };
        d.getFeed = function (boxid) {
            return ajaxservice.get('/qna/', { id: boxid });
        };
        d.leaderBoard = function (boxid) {
            return ajaxservice.get('/box/leaderboard/', { id: boxid });
        }
        d.getRecommended = function (boxid) {
            return ajaxservice.get('/box/recommended/', { id: boxid });
        };
        d.getItems = function (boxid) {
            return ajaxservice.get('/box/items/', { id: boxid });
        };
        d.getMembers = function (boxid) {
            return ajaxservice.get('/box/members/', { boxId: boxid });
        };
        d.getQuizzes = function (boxid) {
            return ajaxservice.get('/box/quizes/', { id: boxid });
        };
        d.getTabs = function (boxid) {
            return ajaxservice.get('/box/tabs/', { id: boxid });
        };
    }
})();