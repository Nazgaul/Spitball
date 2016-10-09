(function () {
    'use strict';
    angular.module('app.account').controller('AccountSettingsInfoController', info);
    info.$inject = ['accountService', '$timeout', 'userData', 'userDetailsFactory',
        '$mdDialog', '$mdMedia', '$scope', 'resManager', 'ajaxService2'];
    function info(accountService, $timeout, userData, userDetailsFactory,
        $mdDialog, $mdMedia, $scope, resManager, ajaxService2) {
        var self = this;


        if (!userData.universityImage) {
            userData.universityImage = 'https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png';
        }
        self.original = userData;
        self.data = angular.copy(userData);

        self.submit = submitChangeName;
        self.changeLanguage = changeLanguage;
        //self.changeTheme = changeTheme;

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
                showToast(resManager.get('settingsChangeName'), 'section-2');
            });
        }


        function changeEmail(ev) {
            accountService.changeEmail(self.data.email).then(function () {
                $mdDialog.show({
                    controller: 'InsertCodeController',
                    controllerAs: 'ic',
                    templateUrl: 'change-email-template.html',
                    parent: angular.element(document.body),
                    targetEvent: ev,
                    clickOutsideToClose: true,
                    fullscreen: $mdMedia('xs')
                }).then(function () {
                    self.original.email = self.data.email;
                    showToast(resManager.get('changeEmailSuccess'),'section-3');
                }, function () {
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
                        showToast(resManager.get('toasterUploadPic'),'section-1');
                        self.data.image = obj.payload;
                        ajaxService2.deleteCacheCategory('accountDetail');
                        //cacheFactory.clearAll();
                        userDetailsFactory.setImage(obj.payload);

                    }
                },
                error: function (uploader, error) {
                    showToast(error.message,'section-1');
                }
            }
        }

        function changeLanguage() {
            if (self.data.language === userData.language) {
                return;
            }

            accountService.changeLocale(self.data.language).then(function () {
                location.reload(true);
            });
        }

        //function changeTheme(theme) {
        //    if (theme === self.data.theme) {
        //        return;
        //    }
        //    accountService.changeTheme(theme).then(function () {
        //        userDetailsFactory.setTheme(theme);
        //        $scope.app.setTheme();
        //        self.data.theme = theme;
        //        //location.reload(true);
        //    });
        //}

        function showToast(message,location) {
            $scope.app.showToaster(message, location);

        }
    }
})();


(function () {
    angular.module('app.account').controller('InsertCodeController', code);
    code.$inject = ['$mdDialog', 'accountService'];

    function code($mdDialog, accountService) {
        var ic = this;
        ic.cancel = cancel;
        ic.submitCode = submitCode;

        function submitCode(myform) {
            accountService.submitCode(ic.code).then(function () {
                $mdDialog.hide();
            }, function(response) {
                myform.code.$setValidity('server', false);
                ic.error = response;
            });

        }

        function cancel() {
            $mdDialog.cancel();
        }


    }
})()