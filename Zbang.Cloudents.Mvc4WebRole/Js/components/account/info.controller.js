﻿(function () {
    angular.module('app.account').controller('AccountSettingsInfoController', info);
    info.$inject = ['accountService', '$timeout', 'userData'];
    function info(accountService, $timeout, userData) {
        var self = this;
        self.data = angular.copy(userData);

        self.submit = function () {

            var firstName = self.data.firstName,
            lastName = self.data.lastName;
            if (firstName == userData.firstName && lastName && userData.lastName) {
                return;
            }
            accountService.setAccountDetails(firstName, lastName).then(function () {
                self.done = true;
            });
        }
        self.changeLanguage = changeLanguage;


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
                    //$scope.loading = true;
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                //uploadProgress: function (uploader, file) {
                //    $scope.loading = file.percent / 100.0;
                //},
                fileUploaded: function (uploader, file, response) {
                    // $scope.loading = false;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        self.data.image = obj.payload;
                        accountService.changeImage(obj.payload);
                    }
                },
                error: function (uploader, error) {
                    //$scope.loading = false;
                    alert(error.message);
                }
            }
        }

        function changeLanguage() {
            if (self.data.language == userData.language) {
                return;
            }

            accountService.changeLocale(self.data.language).then(function() {
                location.reload(true);
            });
        }
    }
})();