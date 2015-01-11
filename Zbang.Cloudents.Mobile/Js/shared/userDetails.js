angular.module('app').service('userDetails', ['account', function (account) {
    var service = this;

    var isAuthenticated,
        userData;

    service.init = function (data) {
        return account.details().then(function (data) {
            userData = data || {};

            ga('create', 'UA-9850006-3', {
                'userId': userData.id,
                'siteSpeedSampleRate': 70,
                'cookieDomain': 'cloudents.com',
                'alwaysSendReferrer': true
            });

            if (!userData.id) {
                isAuthenticated = false;
                return;
            }

            isAuthenticated = true;
            userData.image = userData.image || '/images/emptystate/user.svg';

            ga('set', 'dimension1', userData.universityName);
            ga('set', 'dimension2', userData.universityCountry);
            ga('set', 'dimension3', userData.id);
        
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