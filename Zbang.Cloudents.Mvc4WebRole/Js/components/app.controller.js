(function () {
    angular.module('app').controller('AppController', appController);
    appController.$inject = ['$rootScope', '$window', '$location', 'history', '$state',
        'userDetailsFactory', '$mdToast', '$document', '$mdMenu', 'resManager', 'CacheFactory'];

    function appController($rootScope, $window, $location, h, $state, userDetails, $mdToast,
        $document, $mdMenu, resManager, cacheFactory) {
        var self = this;
        $rootScope.$on('$viewContentLoaded', function () {
            var path = $location.path(),
                absUrl = $location.absUrl(),
                virtualUrl = absUrl.substring(absUrl.indexOf(path));
            $window.dataLayer.push({ event: 'virtualPageView', virtualUrl: virtualUrl });
            $window.Intercom('update');
            svg4everybody();
           
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

        self.showMenu = false;

        self.toggleMenu = toggleMenu;
        function logOut() {
            cacheFactory.clearAll();
        }
        function setTheme() {
            self.theme = 'theme-' + userDetails.get().theme;
        }
        function toggleMenu() {
            $rootScope.$broadcast('open-menu');
        }

        function showToaster(text, parentId) {
            var element = $document.find('body');
            if (parentId) {
                element = $document[0].querySelector('#' + parentId);
            }
            $mdToast.show(
                  $mdToast.simple()
                  .textContent(text)
                  .position('top')
                  .parent(element)
                  .hideDelay(2000));
        }

        // var originatorEv;
        function openMenu($mdOpenMenu, ev) {
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

        $rootScope.$on('$stateChangeSuccess', function (event, toState) {
            self.showMenu = true; // if user comes from university choose need to remove this.
        });




        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            if (!fromState.name) {
                return;
            }
            $mdMenu.hide(); //closes menu
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
                return;
            }
            var userWithNoUniversityState = 'universityChoose';
            if (toStateName !== userWithNoUniversityState) {
                //showToaster(resManager.get('universityChooseContinue'), 'lib-choose');
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