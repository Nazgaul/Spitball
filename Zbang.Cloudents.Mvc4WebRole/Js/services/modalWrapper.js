app.factory('sModal',
    ['$rootScope', '$modal', '$modalStack', '$log',
        function ($rootScope, $modal, $modalStack, $log) {
            var modalList = {
                shareEmail: function (params) {
                    return buildObj('invite', { url: '/Share/MessagePartial/' }, 'ShareCtrl', 'static', true, params.data);
                },
                boxInvite: function (params) {
                    return buildObj('boxInvitePopup', { url: '/Box/SocialInvitePartial/' }, 'BoxInviteCtrl', 'static', true, params.data);
                },
                cloudentsInvite: function () {
                    return buildObj('boxInvitePopup', { url: '/Dashboard/SocialInvitePartial' }, 'InviteCloudentsCtrl', 'static', true);
                },
                boxSettings: function (params) {
                    return buildObj('boxSettings', { url: '/Box/SettingsPartial/' }, 'SettingsCtrl', 'static', true, params.data);
                },
                upload: function (params) {
                    return buildObj('uploader', { url: '/Box/UploadPartial/' }, 'UploadPopupCtrl', 'static', true, params.data);
                },
                tab: function (params) {
                    return buildObj('createTab', { url: '/Box/CreateTabPartial/' }, 'createTabCtrl', 'static', true, params.data);
                },
                uploadLink: function () {
                    return buildObj('uploadLink', { url: '/Box/UploadLinkPartial/' }, 'UploadLinkCtrl', 'static', true);
                },
                createDep: function () {
                    return buildObj('newDpt', { url: '/Library/CreateDepartmentPartial/' }, 'CreateDepartmentCtrl', 'static', true);
                },
                createBoxWizard: function (params) {
                    return buildObj(null, { url: '/Dashboard/CreateBox/' }, 'CreateBoxWizardCtrl', false, false, params.data);
                },
                itemFullscreen: function (params) {
                    return buildObj('fullscreen', { url: '/Item/fullscreen/' }, 'itemFullScreenCtrl', 'static', true, null, params.scope);
                },
                flagItem: function (params) {
                    return buildObj('flagItem', { url: '/Item/Flag/' }, 'itemFlagCtrl', 'static', true, params.data);
                },
                itemRename: function (params) {
                    return buildObj('rename', { url: '/Item/Rename/' }, 'itemRenameCtrl', 'static', true, params.data);
                },
                uniRestriction: function (params) {
                    return buildObj('libChoosePopUp', { html: params.html }, 'restrictionPopUpCtrl', 'static', true, params.data);
                },
                depSettings: function (params) {
                    return buildObj('deptSettings', { url: '/Library/Rename/' }, 'libraryRenameCtrl', 'static', true, params.data);
                },
                quitQuiz: function () {
                    return buildObj('quitQuiz', { url: 'quizMenuTemplate' }, 'QuizCloseCtrl', 'static', true);
                },
                quizChallenge: function (params) {
                    return buildObj('quizPopup', { url: '/Quiz/ChallengePartial/?quizid=' + params.data.quizId }, 'ChallengeCtrl', 'static', true, params.data);
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


            $rootScope.$on('$locationChangeStart', function () {
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
                            return resolveData
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