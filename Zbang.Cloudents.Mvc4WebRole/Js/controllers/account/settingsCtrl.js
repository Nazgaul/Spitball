
var mAccount = angular.module('mAccount', ['angular-plupload']).
    controller('AccountSettingsCtrl',
    ['$scope', '$window', '$timeout', 'sAccount','$analytics',

        function ($scope, $window, $timeout, sAccount,$analytics) {
            "use strict";

            var jsResources = window.JsResources,
                emailRegExp = new RegExp(/^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/);

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


                //TODO: analytics selecttab
                $scope.params.currentTab = tab;
            };


            $scope.saveUserInfo = function (isValid) {

                if (!isValid) {
                    return;
                }

                sAccount.changeProfile($scope.formData).then(function (response) {
                    if (response.success) {
                        $scope.submitted = false;
                        alert('Your settings are saved');

                        //TODO analytics
                    }
                });
            };

            //#region upload
            $scope.onUploaded = function (response) {
                $scope.formData.image = response.urlSmall;
                $scope.formData.largeImage = response.urlLarge;
                $scope.data.image = response.urlLarge;

                //TODO analytics
            };

            $scope.onError = function (error) {
                if (error.code === plupload.FILE_EXTENSION_ERROR) {
                    alert(jsResources.IncorrectExtension);
                }
                if (error.status === 401) {
                    $window.location.href = '/';
                    return;
                }
                if (error.status === 403) {
                    document.location.href = '/';
                    return;
                }

                alert(error.message);
            };

            $scope.onFilesAdded = function (files) {
                $scope.data.image = '/Images/loader2.gif';
            };

            //#endregion

            $scope.changeEmail = function () {
                $scope.params.changeEmailError = '';

                if ($scope.params.verifyCode) {
                    var code = $scope.emailForm.verifyCode.trim();


                    if (!code) {
                        $scope.params.changeEmailError = 'Empty code';
                        return;
                    }

                    code = parseInt($scope.emailForm.verifyCode);

                    if (_.isNaN(code)) {
                        if (!code) {
                            $scope.params.changeEmailError = 'invalid code';
                            return;
                        }
                    }

                    if (!_.isNumber(code)) {
                        $scope.params.changeEmailError = 'Code should be numeric';
                        return;
                    }

                    sAccount.submitCode({ code: code }).then(function (response) {
                        $scope.params.changeEmailBtnText = 'Change email';
                        $scope.params.changingEmail = $scope.params.verifyCode = false;
                        $scope.data.email = $scope.emailForm.email;

                        $scope.emailForm = {};

                        alert(jsResources.EmailChanged);
                        //TODO analytics



                    }, function (response) {
                        alert(response);
                    });
                }

                if (!$scope.params.changingEmail) {
                    $scope.params.changingEmail = true;
                    $scope.params.changeEmailBtnText = 'Change';
                    return;
                }

                var email = $scope.emailForm.email.trim();


                if (!email) {
                    $scope.params.changeEmailError = 'Empty email';
                    return;
                }

                if (!emailRegExp.test(email)) {
                    $scope.params.changeEmailError = 'Invalid email';
                    return;
                }

                sAccount.changeEmail({ email: email }).then(function (response) {
                    $scope.params.verifyCode = true;
                    $scope.params.changeEmailBtnText = 'Save';

                }, function (response) {
                    alert(response[0].value[0]);
                });


            };

            $scope.changePassword = function () {
                if (!$scope.params.changingPassword) {
                    $scope.params.changingPassword = true;
                    return;
                }

                var oldPassword = $scope.passwordForm.oldPassword,
                    newPassword = $scope.passwordForm.newPassword;

                if (!oldPassword) {
                    $scope.params.passwordError = 'Current password cannot be empty';
                    return;
                }
                if (oldPassword.length < 6) {
                    $scope.params.passwordError = 'Minimum password length is 6 characters';
                    return;
                }

                if (!newPassword) {
                    $scope.params.passwordError = 'New password cannot be empty';
                    return;
                }
                if (newPassword.length < 6) {
                    $scope.params.passwordError = 'Minimum password length is 6 characters';
                    return;
                }


                sAccount.changePassword({ currentPassword: oldPassword, newPassword: newPassword }).then(function (response) {                   
                    $scope.params.changingPassword = false;
                    $scope.passwordForm = {};
                    alert(jsResources.PwdChanged);
                    //TODO analytics
                }, function (response) {
                    $scope.params.passwordError = response;
                });
            };

            $scope.changeLanguage = function () {
                if ($scope.languageForm.selected === $scope.data.language) {
                    return;
                }
                //TODO analytics
                sAccount.changeLanguage({ language: $scope.languageForm.selected }).then(function (response) {
                    $window.location.reload();
                });
            };
        }]);