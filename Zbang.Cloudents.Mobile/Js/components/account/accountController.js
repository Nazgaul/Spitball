angular.module('account', ['ajax']).
    controller('AccountController',
    ['accountService', function (accountService) {
        "use strict";
        var account = this;

        account.facebookLogin = function () {            
            accountService.facebookLogin();
        };

        account.changeLanguage = function () {
            accountService.changeLanguage(account.language);
        };

        accountService.doneLoad();
    }]
);