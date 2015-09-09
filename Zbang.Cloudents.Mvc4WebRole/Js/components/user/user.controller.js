(function () {
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', '$stateParams', '$scope', '$q'];

    function user(userService, $stateParams, $scope, $q) {
        var u = this;


        userService.getDetails($stateParams.userId).then(function (response) {
            u.details = response;
        });

        var deferBoxes = $q.defer();
        userService.getboxes($stateParams.userId).then(function (response) {
            u.boxes = response;
            deferBoxes.resolve();
        });

        var deferFriends = $q.defer();
        userService.getfriends($stateParams.userId).then(function (response) {
            u.friends = response;
            deferFriends.resolve();
        });

        var deferFiles = $q.defer();
        userService.getfiles($stateParams.userId).then(function (response) {
            u.files = response;
            deferFiles.resolve();

        });

        var deferFeed = $q.defer();
        userService.getfeed($stateParams.userId).then(function (response) {
            u.feed = response;
            deferFeed.resolve();
        });
        $q.all([
            deferBoxes, deferFeed, deferFiles, deferFriends
        ]).then(function () {
            $scope.$broadcast("tableScroll");
        });

        //$scope.$on('$viewContentLoaded', function () {
        //    $timeout(function () {

        //    });
        //    //TODO: maybe this is no good.
        //    //Metronic.init(); // init core components

        //});
        //initAjax

    }
})();


(function () {
    angular.module('app.user').service('userService', user);
    user.$inject = ['ajaxService'];

    function user(ajaxservice) {
        var u = this;

        u.getDetails = function (userid) {
            return ajaxservice.get('/user/minprofile/', { id: userid });
        }
        u.getboxes = function (userid) {
            return ajaxservice.get('/user/boxes/', { id: userid });
        }

        u.getfriends = function (userid) {
            return ajaxservice.get('/user/friends/', { id: userid });
        }
        u.getfiles = function (userid) {
            return ajaxservice.get('/user/items/', { id: userid });
        }
        u.getfeed = function (userid) {
            return ajaxservice.get('/user/activity/', { id: userid });
        }

    }
})();