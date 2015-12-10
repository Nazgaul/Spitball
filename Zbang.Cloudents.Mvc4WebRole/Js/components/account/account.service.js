(function () {
    angular.module('app.account').service('accountService', accountService);
    accountService.$inject = ['ajaxService', '$q', '$rootScope'];

    function accountService(ajaxservice, $q, $rootScope) {
        var self = this;

        self.changeImage = function (src) {
            self.details.image = src;
            $rootScope.$broadcast('userDetailsChange');
        }

        /*culture: "en-US"
firstTimeBox: false
firstTimeDashboard: false
firstTimeItem: false
firstTimeLibrary: false
id: 1
image: "http://127.0.0.1:10000/devstoreaccount1/zboxprofilepic/S100X100/d35f6930-4e44-469d-adf8-ecac2a1b934f.jpg"
isAdmin: true
name: "ram y"
score: 51550
universityCountry: "IL"
universityId: 920
universityName: "האוניברסיטה הפתוחה"
url: "/user/1/ram-y/"
*/

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
            return ajaxservice.get('/user/notification');
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
    }
})();