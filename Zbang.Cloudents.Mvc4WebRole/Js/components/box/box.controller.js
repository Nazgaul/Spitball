(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', '$stateParams', '$location','$scope' ,'$timeout'];

    function box(boxService, $stateParams, $location, $scope, $timeout) {

        if ($location.path().endsWith($stateParams.boxName + '/')) {
                $location.path($location.path() + 'feed/');
        }

        var b = this;
        var boxData;
        boxService.getBox($stateParams.boxId).then(function (response) {
            boxData = response;
            b.name = response.name;
            b.professorName = response.professorName;
            b.courseId = response.courseId;
        });

        $scope.$on('$viewContentLoaded', function () {
            $timeout(function () {
                Metronic.initAjax();
            });
            //TODO: maybe this is no good.
            //Metronic.initAjax(); // init core components

        });
    }
})();


(function () {
    angular.module('app.box').service('boxService', box);
    box.$inject = ['ajaxService'];

    function box(ajaxservice) {
        var d = this;
        d.getBox = function (boxid) {
            return ajaxservice.get('/box/data/', { id: boxid });
        }
        d.getFeed = function (boxid) {
            return ajaxservice.get('/qna/', { id: boxid });
        }
        d.getLeaderBoard = function(boxid) {
            return ajaxservice.get('/box/leaderboard/', { id: boxid });
        }
        d.getRecommended = function(boxid) {
            return ajaxservice.get('/box/recommended/', { id: boxid });
        }
        d.getItems = function (boxid) {
            return ajaxservice.get('/box/items/', { id: boxid });
        }
        d.getMembers = function (boxid) {
            return ajaxservice.get('/box/members/', { boxId: boxid });
        }
        d.getTabs = function(boxid) {
            return ajaxservice.get('/box/tabs/', { id: boxid });
        }
        

    }
})();