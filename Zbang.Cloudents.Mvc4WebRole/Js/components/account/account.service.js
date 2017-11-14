"use strict";
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
        return AaccountService;
    }());
    AaccountService.$inject = ["ajaxService2"];
    angular.module("app.account").service("accountService", AaccountService);
})(app || (app = {}));
//  # OLD account.service.js
//(function () {
//    'use strict';
//    angular.module('app.account').service('accountService', accountService);
//    accountService.$inject = ['ajaxService2'];
//    function accountService(ajaxservice) {
//        var self = this;
//        self.changeEmail = function (email) {
//            return ajaxservice.post('/account/changeemail/', {
//                email: email
//            }, 'accountDetail');
//        }
//        self.submitCode = function (code) {
//            return ajaxservice.post('/account/entercode/', {
//                code: code
//            });
//        }
//        self.getAccountDetails = function () {
//            return ajaxservice.get('/account/settingsdata/');
//        }
//        self.setAccountDetails = function (firstName, lastName) {
//            return ajaxservice.post('/account/changeprofile/', { firstName: firstName, lastName: lastName }, 'accountDetail');
//        }
//        self.searchUniversity = function (term) {
//            return ajaxservice.get('/university/searchuniversity/', {
//                term: term
//            });
//        }
//        self.updatePassword = function (oldPassword, newPassword) {
//            return ajaxservice.post('/account/changepassword/', {
//                CurrentPassword: oldPassword,
//                NewPassword: newPassword
//            });
//        }
//        self.getNotification = function () {
//            return ajaxservice.get('/user/notification/');
//        }
//        self.setNotification = function (boxId, notification) {
//            return ajaxservice.post('/box/changenotification/', {
//                boxId: boxId,
//                notification: notification
//            });
//        }
//        self.setPersonalNotification = function (value) {
//            return ajaxservice.post('/account/changenotification/', {
//                subscribe: value
//            });
//        }
//        self.changeLocale = function (lang) {
//            return ajaxservice.post('/account/changelocale/', {
//                language: lang
//            }, 'accountDetail');
//        }
//        self.facebookLogIn = function (token, boxId) {
//            return ajaxservice.post('/account/facebooklogin/', {
//                token: token,
//                boxId: boxId
//            });
//        }
//        self.googleLogIn = function (token, boxId) {
//            return ajaxservice.post('/account/googlelogin/', {
//                token: token,
//                boxId: boxId
//            });
//        }
//        self.closeddepartment = function () {
//            return ajaxservice.get('/university/closeddepartment/');
//        }
//        self.closedMembers = function (id) {
//            return ajaxservice.get('/university/closeddepartmentmembers/',
//                {
//                    id: id
//                });
//        }
//        self.approveRequest = function (id, userId) {
//            return ajaxservice.post('/university/approverequest/', {
//                id: id,
//                userId: userId
//            });
//        }
//    }
//})(); 
//# sourceMappingURL=account.service.js.map