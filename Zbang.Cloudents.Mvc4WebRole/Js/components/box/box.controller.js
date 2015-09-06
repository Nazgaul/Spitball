(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', '$stateParams', '$location'];

    function box(boxService, $stateParams, $location) {

        if ($location.path().endsWith($stateParams.boxName + '/')) {
                $location.path($location.path() + 'feed/');
        }

        var b = this;
        var boxData;
        boxService.getBox($stateParams.boxId).then(function (response) {
            console.log(response);
            boxData = response;
            b.name = response.name;
            b.professorName = response.professorName;
            b.courseId = response.courseId;
        });


        b.active = function() {
            console.log('here');
        }
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

    }
})();