var app;
(function (app) {
    "use strict";
    var AaccountService = (function () {
        function AaccountService(ajaxService) {
            this.ajaxService = ajaxService;
        }
        AaccountService.prototype.changeEmail = function (email) {
            return this.ajaxService.post('/account/changeemail/', {
                email: email
            }, 'accountDetail');
        };
        AaccountService.prototype.submitCode = function (code) {
            return this.ajaxService.post('/account/entercode/', {
                code: code
            });
        };
        AaccountService.prototype.getAccountDetails = function () {
            return this.ajaxService.get('/account/settingsdata/');
        };
        AaccountService.prototype.setAccountDetails = function (firstName, lastName) {
            return this.ajaxService.post('/account/changeprofile/', { firstName: firstName, lastName: lastName }, 'accountDetail');
        };
        AaccountService.prototype.searchUniversity = function (term) {
            return this.ajaxService.get('/university/searchuniversity/', {
                term: term
            });
        };
        AaccountService.prototype.updatePassword = function (oldPassword, newPassword) {
            return this.ajaxService.post('/account/changepassword/', {
                CurrentPassword: oldPassword,
                NewPassword: newPassword
            });
        };
        AaccountService.prototype.getNotification = function () {
            return this.ajaxService.get('/user/notification/');
        };
        AaccountService.prototype.setNotification = function (boxId, notification) {
            return this.ajaxService.post('/box/changenotification/', {
                boxId: boxId,
                notification: notification
            });
        };
        AaccountService.prototype.setPersonalNotification = function (value) {
            return this.ajaxService.post('/account/changenotification/', {
                subscribe: value
            });
        };
        AaccountService.prototype.changeLocale = function (lang) {
            return this.ajaxService.post('/account/changelocale/', {
                language: lang
            }, 'accountDetail');
        };
        AaccountService.prototype.facebookLogIn = function (token, boxId) {
            return this.ajaxService.post('/account/facebooklogin/', {
                token: token,
                boxId: boxId
            });
        };
        AaccountService.prototype.googleLogIn = function (token, boxId) {
            return this.ajaxService.post('/account/googlelogin/', {
                token: token,
                boxId: boxId
            });
        };
        AaccountService.prototype.closeddepartment = function () {
            return this.ajaxService.get('/university/closeddepartment/');
        };
        AaccountService.prototype.closedMembers = function (id) {
            return this.ajaxService.get('/university/closeddepartmentmembers/', {
                id: id
            });
        };
        AaccountService.prototype.approveRequest = function (id, userId) {
            return this.ajaxService.post('/university/approverequest/', {
                id: id,
                userId: userId
            });
        };
        AaccountService.$inject = ["ajaxService2"];
        return AaccountService;
    }());
    angular.module("app.account").service("accountService", AaccountService);
})(app || (app = {}));
//# sourceMappingURL=account.service.js.map