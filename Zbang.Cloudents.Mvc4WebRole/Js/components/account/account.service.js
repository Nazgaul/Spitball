(function () {
    angular.module('app.account').service('accountService', accountService);
    accountService.$inject = ['ajaxService', '$q', '$rootScope'];

    function accountService(ajaxservice, $q, $rootScope) {
        var self = this;

        //self.changeImage = function (src) {
        //    //$rootScope.$broadcast('userDetailsChange');
        //}

        self.changeEmail = function(email) {
            return ajaxservice.post('/account/changeemail/', {
                email: email
            });
        }
        self.submitCode = function(code) {
            return ajaxservice.post('/account/entercode/' , {
                code: code
            });
        }
        self.getAccountDetails = function () {
            return ajaxservice.get('/account/settingsdata/', null, 1800000);
        }

        

        self.setAccountDetails = function (firstName, lastName) {
            //self.details.name = firstName + " " + lastName;
            //self.details.universityName = universityName;
            //self.details.universityId = universityId;
            //$rootScope.$broadcast('userDetailsChange');
            return ajaxservice.post('/account/changeprofile/', { firstName: firstName, lastName: lastName });

        }

        self.searchUniversity = function (term) {
            return ajaxservice.get('/library/searchuniversity/', {
                term: term
            });
        }

        self.updatePassword = function (oldPassword, newPassword) {
            return ajaxservice.post('/account/changepassword/', {
                CurrentPassword: oldPassword,
                NewPassword: newPassword
            });
        }

        self.getNotification = function() {
            return ajaxservice.get('/user/notification/');
        }

        self.setNotification = function(boxId, notification) {
            return ajaxservice.post('/box/changenotification/', {
                boxId: boxId,
                notification: notification
            });
        }

        self.changeLocale = function(lang) {
            return ajaxservice.post('/account/changelocale/', {
                language: lang
            });
        }

        self.changeTheme = function (theme) {
            return ajaxservice.post('/account/ChangeTheme/', {
                theme: theme
            });
        }

        self.facebookLogIn = function (token, boxId) {
            return ajaxservice.post('/account/facebooklogin/', {
                token: token,
                boxId: boxId
            });
        }
        self.googleLogIn = function (token, boxId) {
            return ajaxservice.post('/account/googlelogin/', {
                token: token,
                boxId: boxId
            });
        }
    }
})();