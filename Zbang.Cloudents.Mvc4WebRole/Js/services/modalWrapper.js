app.factory('sModal',
    ['$rootScope', '$modal', '$modalStack', '$route', 'sVerChecker',
        function ($rootScope, $modal, $modalStack, $route, sVerChecker) {
            var modalList = {
                shareEmail: function (params) {
                    return buildObj('invite', { url: '/share/messagepartial/' }, 'ShareCtrl', 'none', true, params.data);
                },
                boxInvite: function (params) {
                    return buildObj('boxInvitePopup', { url: '/box/socialinvitepartial/' }, 'BoxInviteCtrl', 'none', true, params.data);
                },
                cloudentsInvite: function () {
                    return buildObj('boxInvitePopup', { url: '/dashboard/socialinvitepartial' }, 'InviteCloudentsCtrl', 'none', true);
                },
                boxSettings: function (params) {
                    return buildObj('boxSettings', { url: '/box/settingspartial/' }, 'SettingsCtrl', 'none', true, params.data);
                },
                upload: function (params) {
                    return buildObj('uploader', { url: '/box/uploadpartial/' }, 'UploadPopupCtrl', 'none', true, params.data);
                },
                tab: function (params) {
                    return buildObj('createTab', { url: '/box/createtabpartial/' }, 'createTabCtrl', 'none', true, params.data);
                },
                uploadLink: function () {
                    return buildObj('uploadLink', { url: '/box/uploadlinkpartial/' }, 'UploadLinkCtrl', 'none', true);
                },
                createDep: function () {
                    return buildObj('newDpt', { url: '/library/createdepartmentpartial/' }, 'CreateDepartmentCtrl', 'none', true);
                },
                createBoxWizard: function (params) {
                    return buildObj('createWizard', { url: '/dashboard/createbox/' }, 'CreateBoxWizardCtrl', false, false, params.data);
                },
                itemFullscreen: function (params) {
                    return buildObj('fullscreen', { url: '/item/fullscreen/' }, 'ItemFullScreenCtrl', 'static', true, null, params.scope);
                },
                flagItem: function (params) {
                    return buildObj('flagItem', { url: '/item/flag/' }, 'ItemFlagCtrl', 'none', true, params.data);
                },
                itemRename: function (params) {
                    return buildObj('rename', { url: '/item/rename/' }, 'ItemRenameCtrl', 'none', true, params.data);
                },
                connectPopup: function (params) {
                    return buildObj('welcome ' + params.windowClass, { url: '/account/welcomeangularpartial/' }, 'LoginWrapperCtrl', 'none', true, params.data);
                },
                itemReg: function (params) {
                    return buildObj('itemReg', { url: '/item/itemregisterpartial/' }, 'ItemRegCtrl', 'none', true, params.data);
                },
                congrats: function (params) {
                    return buildObj('congrats', { url: '/account/congratspartial/' }, 'CongratsCtrl', 'none', true, params.data);
                },
                uniRestriction: function (params) {
                    return buildObj('libChoosePopUp', { html: params.html }, 'RestrictionPopUpCtrl', 'none', true, params.data);
                },
                depSettings: function (params) {
                    return buildObj('deptSettings', { url: '/library/rename/' }, 'LibraryRenameCtrl', 'none', true, params.data);
                },
                quitQuiz: function () {
                    return buildObj('quitQuiz', { url: 'quizMenuTemplate' }, 'QuizCloseCtrl', 'none', true);
                },
                quizChallenge: function (params) {
                    return buildObj('quizPopup', { url: '/quiz/challengepartial/' }, 'ChallengeCtrl', 'static', true, params.data);
                },
                //coupon: function (params) {
                //    return buildObj('couponPopup', { url: '/store/couponpartial/' }, 'CouponCtrl', 'none', true, params.data);
                //},
                leavePrompt: function (params) {
                    return buildObj('leavePrompt', { url: '/box/leavepromptpartial/' }, 'LeavePromptCtrl', 'static', true, params.data);
                },
                cloudentsIsNowSpitball: function () {
                    return buildObj('leavePrompt', { url: '/home/cloudentsIsNowSpitball/' }, 'SpitballCtrl', 'static', true);
                }
            };

            var opened = {};


            var service = {
                open: function (modalId, params) {

                    if (opened[modalId]) {
                        return;
                    }

                    params = params || {};
                    params.callback = params.callback || {};

                    var modalParams = modalList[modalId](params),
                        modalInstance = $modal.open(modalParams);

                    opened[modalId] = true;

                    modalInstance.result.then(function (response) {
                        if (_.isFunction(params.callback.close)) {
                            params.callback.close(response);
                        }
                    }, function (response) {
                        if (_.isFunction(params.callback.dismiss)) {
                            params.callback.dismiss(response);
                        }
                    })['finally'](function (response) {
                        //var index = modalsOpened.indexOf(modalInstance);
                        //modalsOpened.splice(index,1);
                        modalInstance = undefined;
                        opened[modalId] = false;
                        if (_.isFunction(params.callback.always)) {
                            params.callback.always(response);
                        }

                    });

                    //modalsOpened.push(modalInstance);
                }
            };


            $rootScope.$on('$routeChangeStart', function () {
                $modalStack.dismissAll();
                //modalsOpened=[];

            });


            function buildObj(windowClass, template, controller, backDrop, keyboard, resolveData, scope) {
                resolveData = resolveData || {};

                var params = {
                    windowClass: windowClass,
                    controller: controller,
                    backdrop: _.isUndefined(backDrop) ? 'static' : backDrop,
                    keyboard: _.isUndefined(keyboard) ? true : keyboard,
                    resolve: {
                        data: function () {
                            return resolveData;
                        }
                    }
                };

                var routeName = $route.current.$$route.params.type;


                if (routeName === 'item' || routeName === 'quiz') {
                    params.windowClass += ' popupOffset'
                }

                if (!_.isEmpty(scope)) {
                    params.scope = scope;
                }

                if (template.url) {                    
                    params.templateUrl = template.url;
                    if (params.templateUrl.indexOf('/') > -1) {
                        params.templateUrl += '?lang=' + getCookie('l2') + '&version=' + sVerChecker.currentVersion();
                    }
                     
                    if (resolveData.quizId) {
                        var queryString = 'quizId=' + resolveData.quizId;
                        if (params.templateUrl.indexOf('?')) {
                            params.templateUrl += '&' + queryString;
                        } else {
                            params.templateUrl += '?' + queryString;
                        }
                    }
                }

                if (template.html) {
                    params.template = template.html;
                }

                return params;

            }

            return service;
        }]
);