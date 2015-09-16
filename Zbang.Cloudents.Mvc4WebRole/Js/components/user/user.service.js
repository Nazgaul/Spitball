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