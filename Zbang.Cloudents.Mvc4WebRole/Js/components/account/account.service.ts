
module app {
    "use strict";

    export interface IAaccountService {
        changeEmail(email: string): angular.IPromise<any>;
        submitCode(code: number): angular.IPromise<any>;
        getAccountDetails(): angular.IPromise<any>;
        setAccountDetails(firstName: string, lastName: string): angular.IPromise<any>;
        searchUniversity(term: string): angular.IPromise<any>;
        updatePassword(oldPassword: string, newPassword: string): angular.IPromise<any>;
        getNotification(): angular.IPromise<any>;
        setNotification(boxId: number, notification: string): angular.IPromise<any>;
        setPersonalNotification(value: string): angular.IPromise<any>;
        changeLocale(lang: string): angular.IPromise<any>;
        facebookLogIn(token: string, boxId: number): angular.IPromise<any>;
        googleLogIn(token: string, boxId: number): angular.IPromise<any>;
        closeddepartment(): angular.IPromise<any>;
        closedMembers(id: Guid): angular.IPromise<any>;
        approveRequest(id: Guid, userId: number): angular.IPromise<any>;
    }

    class AaccountService implements IAaccountService {
        static $inject = ["ajaxService2"];
        constructor(private ajaxService: IAjaxService2) {

        }
        changeEmail(email: string) {
            return this.ajaxService.post('/account/changeemail/', {
                email: email
            }, 'accountDetail');
        }
        submitCode(code: number) {
            return this.ajaxService.post('/account/entercode/', {
                code: code
            });
        }
        getAccountDetails() {
            return this.ajaxService.get('/account/settingsdata/');
        }



        setAccountDetails(firstName: string, lastName: string) {
            return this.ajaxService.post('/account/changeprofile/', { firstName: firstName, lastName: lastName }, 'accountDetail');

        }

        searchUniversity(term: string) {
            return this.ajaxService.get('/university/searchuniversity/', {
                term: term
            });
        }

        updatePassword(oldPassword: string, newPassword: string) {
            return this.ajaxService.post('/account/changepassword/', {
                CurrentPassword: oldPassword,
                NewPassword: newPassword
            });
        }

        getNotification() {
            return this.ajaxService.get('/user/notification/');
        }


        setNotification(boxId: number, notification: string) {
            return this.ajaxService.post('/box/changenotification/', {
                boxId: boxId,
                notification: notification
            });
        }
        setPersonalNotification(value: string) {
            return this.ajaxService.post('/account/changenotification/', {
                subscribe: value
            });
        }

        changeLocale(lang: string) {
            return this.ajaxService.post('/account/changelocale/', {
                language: lang
            }, 'accountDetail');
        }



        facebookLogIn(token: string, boxId: number) {
            return this.ajaxService.post('/account/facebooklogin/', {
                token: token,
                boxId: boxId
            });
        }
        googleLogIn(token: string, boxId: number) {
            return this.ajaxService.post('/account/googlelogin/', {
                token: token,
                boxId: boxId
            });
        }

        closeddepartment() {
            return this.ajaxService.get('/university/closeddepartment/');
        }
        closedMembers(id: Guid) {
            return this.ajaxService.get('/university/closeddepartmentmembers/',
                {
                    id: id
                });
        }
        approveRequest(id: Guid, userId: number) {
            return this.ajaxService.post('/university/approverequest/', {
                id: id,
                userId: userId
            });
        }
    }
    angular.module("app.account").service("accountService", AaccountService);
}



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