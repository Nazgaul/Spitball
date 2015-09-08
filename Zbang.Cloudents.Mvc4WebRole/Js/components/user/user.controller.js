(function () {
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', '$stateParams'];

    function user(userService, $stateParams) {
        var u = this;
        

        userService.getDetails($stateParams.boxId).then(function (response) {
            console.log(response);
        });

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
       
    }
})();