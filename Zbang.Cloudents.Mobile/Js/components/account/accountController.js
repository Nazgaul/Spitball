angular.module('account', ['ajax']).
    controller('AccountController',
    ['accountService', function (accountService) {
        "use strict";
        var account = this;

        account.firstTime = accountService.firstTime;

        account.facebookLogin = function () {            
            accountService.facebookLogin();
        };

        account.changeLanguage = function () {
            accountService.changeLanguage(account.language);
        };

        accountService.doneLoad();

        account.closePopup = function () {
            account.isPopupClosed = true;
        };

    }]
);