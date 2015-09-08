(function () {
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', '$stateParams', '$scope', '$timeout'];

    function user(userService, $stateParams, $scope, $timeout) {
        var u = this;
        

        userService.getDetails($stateParams.userId).then(function (response) {
            u.details = response;
        });

        userService.getboxes($stateParams.userId).then(function (response) {
            u.boxes = response;
        });

        userService.getfriends($stateParams.userId).then(function (response) {
            u.friends = response;
        });

        userService.getfiles($stateParams.userId).then(function (response) {
            console.log(response);
            u.files = response;
        });
        userService.getfeed($stateParams.userId).then(function (response) {
            console.log(response);
            u.feed = response;
        });


        $scope.$on('$viewContentLoaded', function () {
            $timeout(function () {
               
            });
            //TODO: maybe this is no good.
            //Metronic.init(); // init core components

        });
        //initAjax

    }
})();


(function () {
    angular.module('app.user').service('userService', user);
    user.$inject = [ 'ajaxService'];

    function user(ajaxservice) {
        var u = this;

        u.getDetails = function (userid) {
            return ajaxservice.get('/user/minprofile/', { id: userid });
        }
        u.getboxes = function(userid) {
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