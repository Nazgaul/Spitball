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
                if ($().tabdrop) {
                    $('.tabbable-tabdrop .nav-pills, .tabbable-tabdrop .nav-tabs').tabdrop({
                        text: '<i class="fa fa-ellipsis-v"></i>&nbsp;<i class="fa fa-angle-down"></i>'
                    });
                }
            });
            //TODO: maybe this is no good.
            //Metronic.init(); // init core components

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
        d.getRecommended = function(boxid) {
            return ajaxservice.get('/box/recommended/', { id: boxid });
        }
        d.getItems = function (boxid) {
            return ajaxservice.get('/box/items/', { id: boxid });
        }

    }
})();