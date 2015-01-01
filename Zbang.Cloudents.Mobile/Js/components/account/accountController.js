angular.module('account', ['ajax', 'social']).
    controller('AccountController',
    ['accountService', function (accountService) {
        "use strict";
        var account = this;

        account.facebookLogin = function () {
            alert('asd');
            accountService.facebookLogin();
        };

        account.changeLanguage = function (language) {
            accountService.changeLanguage(language);
        };
    }]
);