(function () {
    angular.module('app.user').run(appRun);

    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());


        function getStates() {
            var partial = '/library/indexpartial/';
            return [
            {
                state: 'department',
                config: {
                    url: '/library/',
                    controller: 'Library as l',
                    resolve: {
                        nodeData: [
                           'libraryService', function (libraryService) {
                               return libraryService.getDepartments();
                           }
                        ]
                    }
                },

                templateUrl: partial
            },
            {
                state: 'departmentWithNode', config: {
                    url: '/library/:nodeId/:nodeName/',
                    controller: 'Library as l',
                    resolve: {
                        nodeData: [
                           'libraryService', '$stateParams', function (libraryService, $stateParams) {
                               return libraryService.getDepartments($stateParams.nodeId);
                           }
                        ]
                    }
                },
                templateUrl: partial
            },
            {
                state: 'universityChoose', config: {
                    url: '/library/choose/',
                    controller: 'LibraryChoose as lc',
                    data: { animateClass: 'library-choose' }
                },
                templateUrl: '/library/choosepartial/'
            }


            ];
        }
    }
})();