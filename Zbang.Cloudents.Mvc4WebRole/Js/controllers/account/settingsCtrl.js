﻿var mAccount = angular.module('mAccount', ['angular-plupload']).
    controller('AccountSettingsCtrl',
    ['$scope', '$window', '$timeout', 'sAccount', '$analytics', 'sNotify', '$angularCacheFactory', 'resManager','sUserDetails',

        function ($scope, $window, $timeout, sAccount, $analytics, sNotify, $angularCacheFactory, resManager, sUserDetails) {
            "use strict";

            var emailRegExp = new RegExp(/^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/);

            $scope.formData = {};
            $scope.emailForm = {};
            $scope.passwordForm = {};
            $scope.languageForm = {};

            $scope.params = {
                changeEmailBtnText: 'Change',
                currentTab: 'profile'
            };
            $scope.languageForm = {};

            sAccount.settings.data().then(function (data) {
                $scope.formData.firstName = data.firstName;
                $scope.formData.middleName = data.middleName;
                $scope.formData.lastName = data.lastName;
                $scope.languageForm.selected = data.language;
                $scope.data = data;
                $scope.data.usedSpacePercent = data.usedSpace / data.allocatedSize * 100;
                $scope.data.freeSpacePercent = 100 - $scope.data.usedSpacePercent;
            });

            $timeout(function () {
                $scope.$emit('viewContentLoaded');
            });

            $scope.selectTab = function (tab) {
                if ($scope.params.currentTab === tab) {
                    return;
                }

                $scope.params.currentTab = tab;

                $analytics.eventTrack('Account Settings', {
                    category: 'Select Tab',
                    label: 'User selected ' + tab
                });
            };


            $scope.saveUserInfo = function (isValid) {

                if (!isValid) {
                    return;
                }

                sAccount.changeProfile($scope.formData).then(function () {
                    $scope.submitted = false;
                    sNotify.alert('Your settings are saved');
                    sUserDetails.setImage($scope.formData.image);

                    $analytics.eventTrack('User Information', {
                        category: 'Save Settings',
                        label: 'User saved settings'
                    });
                });
            };

            //#region upload
            $scope.onUploaded = function (response) {
                $scope.params.uploading = false;

                $scope.formData.image = response.urlSmall;
                $scope.formData.largeImage = response.urlLarge;
                $scope.data.image = response.urlLarge;
                
                $analytics.eventTrack('User Information', {
                    category: 'Upload Image',
                    label: 'User uploaded an image'
                });
            };

            $scope.onError = function (error) {

                $scope.params.uploading = false;

                if (error.code === plupload.FILE_EXTENSION_ERROR) {
                    sNotify.tAlert('IncorrectExtension');
                }
                if (error.status === 401) {
                    $window.location.href = '/';
                    return;
                }
                if (error.status === 403) {
                    document.location.href = '/';
                    return;
                }

                sNotify.alert(error.message);
            };

            $scope.onFilesAdded = function () {
                $scope.params.uploading = true;
            };

            //#endregion

            $scope.changeEmail = function () {
                $scope.params.changeEmailError = '';

                if ($scope.params.verifyCode) {
                    var code = $scope.emailForm.verifyCode.trim();


                    if (!code) {
                        $scope.params.changeEmailError = resManager.get('FieldRequired');
                        return;
                    }

                    code = parseInt($scope.emailForm.verifyCode);

                    if (_.isNaN(code)) {
                        if (!code) {
                            $scope.params.changeEmailError = resManager.get('CodeIncorrect');
                            return;
                        }
                    }

                    if (!_.isNumber(code)) {
                        $scope.params.changeEmailError = resManager.get('CodeNumeric');
                        return;
                    }

                    sAccount.submitCode({ code: code }).then(function() {
                        $scope.params.changeEmailBtnText = resManager.get('ChangeEmail');
                        $scope.params.changingEmail = $scope.params.verifyCode = false;
                        $scope.data.email = $scope.emailForm.email;

                        $scope.emailForm = {};

                        sNotify.tAlert('EmailChanged');
                        $analytics.eventTrack('Account settings', {
                            category: 'Email Changed',
                            label: 'User changed email'
                        });


                    }, function(response) {
                        sNotify.alert(response);
                    });
                } else {

                    if (!$scope.params.changingEmail) {
                        $scope.params.changingEmail = true;
                        $scope.params.changeEmailBtnText = resManager.get('Change');
                        return;
                    }

                    var email = $scope.emailForm.email.trim();


                    if (!email) {
                        $scope.params.changeEmailError = resManager.get('FieldRequired');
                        return;
                    }

                    if (!emailRegExp.test(email)) {
                        $scope.params.changeEmailError = resManager.get('InvalidEmail');
                        return;
                    }

                    sAccount.changeEmail({ email: email }).then(function() {
                        $scope.params.verifyCode = true;
                        $scope.params.changeEmailBtnText = resManager.get('Save');
                        $analytics.eventTrack('Account settings', {
                            category: 'Email Change',
                            label: 'User started changing email proccess'
                        });
                    }, function(response) {
                        sNotify.alert(response[0].value[0]);
                    });
                }


            };

            $scope.changePassword = function () {
                if (!$scope.params.changingPassword) {
                    $scope.params.changingPassword = true;
                    return;
                }

                var oldPassword = $scope.passwordForm.oldPassword,
                    newPassword = $scope.passwordForm.newPassword;

                if (!oldPassword) {
                    $scope.params.passwordError = resManager.get('FieldRequired');
                    return;
                }
                if (oldPassword.length < 6) {
                    $scope.params.passwordError = resManager.get('PwdAtLeast6Chars');
                    return;
                }

                if (!newPassword) {
                    $scope.params.passwordError = resManager.get('FieldRequired');
                    return;
                }
                if (newPassword.length < 6) {
                    $scope.params.passwordError = resManager.get('PwdAtLeast6Chars');
                    return;
                }


                sAccount.changePassword({ currentPassword: oldPassword, newPassword: newPassword }).then(function () {
                    $scope.params.changingPassword = false;
                    $scope.passwordForm = {};
                    sNotify.tAlert('PwdChanged');
                    $analytics.eventTrack('Account settings', {
                        category: 'Password Change',
                        label: 'User changed password'
                    });
                }, function (response) {
                    $scope.params.passwordError = response;
                }).finally(function () {
                    $scope.params.passwordError = null;
                });
            };

            $scope.changeLanguage = function () {
                if ($scope.languageForm.selected === $scope.data.language) {
                    return;
                }

                //$angularCacheFactory.get('htmlCache').removeAll();

                $analytics.eventTrack('Account settings', {
                    category: 'Language Change',
                    label: 'User changed language to ' + $scope.languageForm.selected
                });

                sAccount.changeLanguage({ language: $scope.languageForm.selected }).then(function () {


                    $window.location.reload();
                });
            };
        }]);