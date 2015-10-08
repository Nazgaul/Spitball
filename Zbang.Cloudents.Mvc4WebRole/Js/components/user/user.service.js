﻿(function () {
    angular.module('app.user').service('userService', user);
    user.$inject = ['ajaxService'];

    function user(ajaxservice) {
        var u = this;

        u.getDetails = function (userid) {
            return ajaxservice.get('/user/minprofile/', { id: userid });
        }
        u.boxes = function (userid, page) {
            return ajaxservice.get('/user/boxes/', { id: userid, page: page });
        }

        u.friends = function (userid, page) {
            return ajaxservice.get('/user/friends/', { id: userid, page: page });
        }
        u.getfiles = function (userid) {
            return ajaxservice.get('/user/items/', { id: userid });
        }
        u.getfeed = function (userid) {
            return ajaxservice.get('/user/comment/', { id: userid });
        }

    }
})();