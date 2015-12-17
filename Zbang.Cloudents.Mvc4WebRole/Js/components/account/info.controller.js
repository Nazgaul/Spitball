(function () {
    angular.module('app.account').controller('AccountSettingsInfoController', info);
    info.$inject = ['accountService', '$timeout', 'userData', 'userDetailsFactory', '$mdDialog', '$mdMedia', '$scope'];
    function info(accountService, $timeout, userData, userDetailsFactory, $mdDialog, $mdMedia, $scope) {
        var self = this;
        self.original = userData;
        self.data = angular.copy(userData);

        self.submit = submitChangeName;
        self.changeLanguage = changeLanguage;

        self.changeEmail = changeEmail;
        self.cancelChangeEmail = cancelChangeEmail;
        self.cancelChangeName = cancelChangeName;

        function cancelChangeEmail() {
            self.data.email = self.original.email;
        }
        function cancelChangeName() {
            self.data.firstName = self.original.firstName;
            self.data.lastName = self.original.lastName;
        }

        function submitChangeName() {
                var firstName = self.data.firstName,
                lastName = self.data.lastName;
                if (firstName === userData.firstName && lastName === userData.lastName) {
                    return;
                }

                accountService.setAccountDetails(firstName, lastName).then(function () {
                    userDetailsFactory.setName(firstName, lastName);
                    showToast('update complete');
                });
        }
       

        function changeEmail(ev) {
            accountService.changeEmail(self.data.email).then(function() {
                $mdDialog.show({
                    controller: 'InsertCodeController',
                    controllerAs: 'ic',
                    templateUrl: 'change-email-template.html',
                    parent: angular.element(document.body),
                    targetEvent: ev,
                    clickOutsideToClose: true,
                    fullscreen: $mdMedia('xs')
                }).then(function() {
                    self.original.email = self.data.email;
                }, function() {
                    cancelChangeEmail();
                });
            });
        }


        self.fileUpload = {
            url: '/upload/profilepicture/',
            options: {
                multi_selection: false,
                max_file_size: '10mb',
                mime_types: [
                    { title: "Image files", extensions: "jpg,gif,png" }
                ]
            },
            callbacks: {
                filesAdded: function (uploader) {
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                fileUploaded: function (uploader, file, response) {
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        self.data.image = obj.payload;
                        userDetailsFactory.setImage(obj.payload);
                        accountService.changeImage(obj.payload);
                    }
                },
                error: function (uploader, error) {
                    showToast(error.message);
                }
            }
        }

        function changeLanguage() {
            if (self.data.language == userData.language) {
                return;
            }

            accountService.changeLocale(self.data.language).then(function () {
                location.reload(true);
            });
        }

        function showToast(messae) {
            $scope.app.showToaster(messae, 'accountPage');
           
        }
    }
})();


(function () {
    angular.module('app.account').controller('InsertCodeController', code);
    code.$inject = ['$mdDialog',  'accountService'];

    function code($mdDialog, accountService) {
        var ic = this;
        ic.cancel = cancel;
        ic.submitCode = submitCode;
        function submitCode() {
            console.log(ic.code);
            accountService.submitCode(ic.code).then(function() {
                $mdDialog.hide();
            });

        }

        function cancel() {
            $mdDialog.cancel();
        }


    }
})()