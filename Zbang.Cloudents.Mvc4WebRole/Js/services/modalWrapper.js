app.factory('sModal',
    ['$rootScope', '$modal', '$modalStack',
        function ($rootScope, $modal, $modalStack) {
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
                    return buildObj('fullscreen', { url: '/item/fullscreen/' }, 'itemFullScreenCtrl', 'static', true, null, params.scope);
                },
                flagItem: function (params) {
                    return buildObj('flagItem', { url: '/item/flag/' }, 'itemFlagCtrl', 'none', true, params.data);
                },
                itemRename: function (params) {
                    return buildObj('rename', { url: '/item/rename/' }, 'itemRenameCtrl', 'none', true, params.data);
                },
                uniRestriction: function (params) {
                    return buildObj('libChoosePopUp', { html: params.html }, 'restrictionPopUpCtrl', 'none', true, params.data);
                },
                depSettings: function (params) {
                    return buildObj('deptSettings', { url: '/library/rename/' }, 'libraryRenameCtrl', 'none', true, params.data);
                },
                quitQuiz: function () {
                    return buildObj('quitQuiz', { url: 'quizMenuTemplate' }, 'QuizCloseCtrl', 'none', true);
                },
                quizChallenge: function (params) {
                    return buildObj('quizPopup', { url: '/quiz/challengepartial/?quizid=' + params.data.quizId }, 'ChallengeCtrl', 'static', true, params.data);
                }
            };


            var service = {
                open: function (modalId, params) {
                    params = params || {};
                    params.callback = params.callback || {};

                    var modalParams = modalList[modalId](params),
                        modalInstance = $modal.open(modalParams);

                    modalInstance.result.then(function (response) {

                        if (angular.isFunction(params.callback.close)) {
                            params.callback.close(response);
                        }
                    }, function (response) {
                        if (angular.isFunction(params.callback.dismiss)) {
                            params.callback.dismiss(response);
                        }
                    })['finally'](function (response) {
                        //var index = modalsOpened.indexOf(modalInstance);
                        //modalsOpened.splice(index,1);
                        modalInstance = undefined;
                        if (angular.isFunction(params.callback.always)) {
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

                if (!_.isEmpty(scope)) {
                    params.scope = scope;
                }

                if (template.url) {
                    params.templateUrl = template.url;
                }

                if (template.html) {
                    params.template = template.html;
                }

                return params;

            }

            return service;
        }]
);