'use strict';
(function () {
    angular.module('app').controller('AppController', appController);
    appController.$inject = ['$rootScope', '$window', '$location',  '$state',
        'userDetailsFactory', '$mdToast', '$document', '$mdMenu', 'resManager', 'CacheFactory', '$scope', '$mdSidenav'
    ];

    function appController($rootScope, $window, $location, $state, userDetails, $mdToast,
        $document, $mdMenu, resManager, cacheFactory, $scope, $mdSidenav) {
        var self = this;
        $rootScope.$on('$viewContentLoaded', function () {
            var path = $location.path(),
                absUrl = $location.absUrl(),
                virtualUrl = absUrl.substring(absUrl.indexOf(path));
            // ReSharper disable UndeclaredGlobalVariableUsing
            dataLayer.push({ event: 'virtualPageView', virtualUrl: virtualUrl }); // google tag manger
            __insp.push(["virtualPage"]); //inspectlet
            // ReSharper restore UndeclaredGlobalVariableUsing

        });
        userDetails.init().then(function () {
            setTheme();
        });

        self.setTheme = setTheme;

        //self.back = function (defaultUrl) {
        //    var element = h.popElement();
        //    if (!element) {
        //        $location.url(defaultUrl);
        //        return;
        //    }
        //    if (typeof element === 'string') {
        //        $location.url(element);
        //    }
        //    $rootScope.$broadcast('from-back');
        //    $state.go(element.name, element.params);
        //};

        self.logOut = logOut;
        self.openMenu = openMenu;
        self.menuOpened = false;
        //self.chatOpened = false;
        self.expandSearch = false;
        self.resetForm = resetForm;
        self.showToaster = showToaster;
        self.loadChat = false;
        self.toggleMenu = toggleMenu;
        self.chatDisplayState = 1;//collapsed

        //self.toggleChat = toggleChat;
        function logOut() {
            cacheFactory.clearAll();
            Intercom('shutdown');
        }
        function initChat() {
            var details = userDetails.get();
            self.loadChat = details.university.id;
        }

        function setTheme() {
            self.theme = 'theme-' + userDetails.get().theme;
        }
        function toggleMenu() {
            $rootScope.$broadcast('open-menu');
        }

        //function toggleChat() {
        //    $rootScope.$broadcast('toggle-chat');
        //}

        function showToaster(text, parentId, theme) {
            var element = $document.find('header')[0];
            if (parentId) {
                element = $document[0].querySelector('#' + parentId);
            }

            $mdToast.show(
                 $mdToast.simple()
                 .textContent(text)
                 .capsule(true)
                 .position('top right')
                 .parent(element)
                 .theme(theme)
                 .hideDelay(2000));
        }

        // var originatorEv;
        function openMenu($mdOpenMenu, ev) {
            self.menuOpened = true;
            //originatorEv = ev;
            if (!userDetails.isAuthenticated()) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            $mdOpenMenu(ev);
        }

        function resetForm(myform) {
            myform.$setPristine();
            myform.$setUntouched();
        }

        $rootScope.$on('$mdMenuClose', function () {
            self.menuOpened = false;
        });

        self.showMenu = self.showSearch = self.showChat = true;
        self.showBoxAd = false;

        $rootScope.$on('$stateChangeSuccess', function (event, toState) {
            self.showBoxAd = toState.parent === 'box';
            self.showChat = self.showSearch = !(toState.name === 'universityChoose');
            self.showMenu = !(toState.name === 'item' || toState.name === 'quiz' || toState.name === 'universityChoose');
            if (toState.name === 'universityChoose') {
                $mdSidenav('chat').close();
            }
        });

        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            if (!fromState.name) {
                return;
            }
            //can't access anonymous user
            if (toState.name === 'user' && toParams.userId === "22886") {
                event.preventDefault();
                $rootScope.$broadcast('state-change-start-prevent');
            }
            $mdMenu.hide(); //closes menu
            $mdToast.hide(); // hide toasters
            $rootScope.$broadcast('close-menu');

            var toStateName = toState.name;
            if (toStateName !== 'searchinfo') {
                $rootScope.$broadcast('search-close');
            }
            if (fromParams.boxId && toParams.boxId) {
                if (fromParams.boxId === toParams.boxId && toStateName === 'box' && fromState.name.startsWith('box')) {
                    event.preventDefault();
                    $rootScope.$broadcast('state-change-start-prevent');
                }
            }
            if (toStateName === 'settings' && fromState.name.startsWith('settings')) {
                event.preventDefault();
                $rootScope.$broadcast('state-change-start-prevent');
            }
            $rootScope.$broadcast('close-collapse');
            if (!userDetails.isAuthenticated()) {
                return;
            }
            var details = userDetails.get();
            if (details.university.id) {
                document.title = resManager.get('siteName');
                return;
            }
            var userWithNoUniversityState = 'universityChoose';
            if (toStateName !== userWithNoUniversityState) {
                $rootScope.$broadcast('state-change-start-prevent');
                event.preventDefault();
            }
        });

        $scope.$watch(
        userDetails.getUniversity,
        function (val) {
            if (val) {
                initChat();
                return;
            }
        });

    }
})();