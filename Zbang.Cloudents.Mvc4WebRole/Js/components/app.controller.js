(function () {
    angular.module('app').controller('AppController', appController);
    appController.$inject = ['$rootScope', '$window', '$location', 'history', '$state',
        'userDetailsFactory', '$mdToast', '$document', '$mdMenu', 'resManager', 'CacheFactory'
    ];

    function appController($rootScope, $window, $location, h, $state, userDetails, $mdToast,
        $document, $mdMenu, resManager, cacheFactory) {
        var self = this;
        $rootScope.$on('$viewContentLoaded', function () {
            var path = $location.path(),
                absUrl = $location.absUrl(),
                virtualUrl = absUrl.substring(absUrl.indexOf(path));
            // ReSharper disable UseOfImplicitGlobalInFunctionScope
            dataLayer.push({ event: 'virtualPageView', virtualUrl: virtualUrl }); // google tag manger
            Intercom('update'); //intercom
            __insp.push(["virtualPage"]); //inspectlet
            // svg4everybody();
            // ReSharper restore UseOfImplicitGlobalInFunctionScope

        });
        userDetails.init().then(function () {
            setTheme();
        });

        self.setTheme = setTheme;

        self.back = function (defaultUrl) {
            var element = h.popElement();
            if (!element) {
                $location.url(defaultUrl);
                return;
            }
            if (typeof element === 'string') {
                $location.url(element);
            }
            $rootScope.$broadcast('from-back');
            $state.go(element.name, element.params);
        }

        self.logOut = logOut;
        self.openMenu = openMenu;
        self.menuOpened = false;
        self.expandSearch = false;
        self.resetForm = resetForm;
        self.showToaster = showToaster;
        
        self.toggleMenu = toggleMenu;
        function logOut() {
            cacheFactory.clearAll();
            Intercom('shutdown');
        }
        function setTheme() {
            self.theme = 'theme-' + userDetails.get().theme;
        }
        function toggleMenu() {
            $rootScope.$broadcast('open-menu');
        }

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
        };


        function resetForm(myform) {
            myform.$setPristine();
            myform.$setUntouched();
        }

        $rootScope.$on('$mdMenuClose', function () {
            self.menuOpened = false;
        });

        self.showMenu = true;
        self.showSearch = true;
        self.fixedBgColor = false;

        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            self.showBoxAd = toState.parent === 'box';

            self.showMenu = true;
            self.showSearch = true;
            self.fixedBgColor = false;

            if (toState.name === 'item' || toState.name === 'quiz') {
                self.fixedBgColor = true;
                self.showMenu = false;
            }
            if (toState.name === 'universityChoose') {
                self.showMenu = false;
                self.showSearch = false;
            }

            if (!fromState.name) {
                return;
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

        //$rootScope.$on('$stateNotFound',
        //    function (event, unfoundState, fromState, fromParams) {
        //        console.log(unfoundState.to); // "lazy.state"
        //        console.log(unfoundState.toParams); // {a:1, b:2}
        //        console.log(unfoundState.options); // {inherit:false} + default options
        //    });

        //$rootScope.$on('$stateChangeError',
        //   function (event, toState, toParams, fromState, fromParams, error) {
        //       console.log(event, toState, toParams, fromState, fromParams, error);
        //   });
    }
})();