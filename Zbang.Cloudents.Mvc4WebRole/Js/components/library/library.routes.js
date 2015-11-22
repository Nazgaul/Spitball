(function () {
    angular.module('app.user').run(appRun);

    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());


        function getStates() {
            return [
            {
                state: 'department',
                config: {
                    url: '/library/',
                    templateUrl: function () {
                        return routerHelper.buildUrl('/library/indexpartial/');
                    },
                    controller: 'Library as l'
                }
            },
            {
                state: 'departmentWithNode', config: {
                    url: '/library/:nodeId/:nodeName/',
                    templateUrl: function () {
                        return routerHelper.buildUrl('/library/indexpartial/');
                    },
                    controller: 'Library as l'
                }
            },
            {
                state: 'universityChoose', config: {
                    url: '/library/choose/',
                    templateUrl: function () {
                        return routerHelper.buildUrl('/library/choosepartial/');
                    },
                    controller: 'LibraryChoose as lc'
                }
            }


            ];
        }
    }
})();