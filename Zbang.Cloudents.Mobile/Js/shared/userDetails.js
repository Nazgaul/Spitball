angular.module('app').service('userDetails', ['account', function (account) {
    var service = this;

    var isAuthenticated,
        userData;

    service.init = function (data) {
        return account.details().then(function (data) {            
            userData = data || {};

            //TODO: remove name
            if (userData.id) {
                isAuthenticated = true;
            }
        });        
    };

    service.getId = function () {
        return userData.id;
    };

    service.isAuthenticated = function () {
        return isAuthenticated;
    };

    service.getUniversityId = function () {
        return userData.universityId;
    };

    service.isAdmin = function () {
        return userData.isAdmin
    };

    service.getName = function () {
        return userData.name;
    };

    service.getImage = function () {
        return userData.image;
    };

    service.isFirstTimeDashboard = function () {
        return userData.firstTimeDashboard;
    };
}]
);