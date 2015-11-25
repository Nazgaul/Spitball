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
                    controller: 'Library as l'
                },
                templateUrl: partial
            },
            {
                state: 'departmentWithNode', config: {
                    url: '/library/:nodeId/:nodeName/',
                    controller: 'Library as l'
                },
                templateUrl: partial
            },
            {
                state: 'universityChoose', config: {
                    url: '/library/choose/',
                    controller: 'LibraryChoose as lc',
                    resolve: {
                        universitySuggest: [
                            'facebookService', 'libraryService', function (facebookService, libraryService) {
                                return facebookService.getToken().then(function (token) {
                                    return libraryService.getUniversityByFriends(token);
                                });

                            }
                        ],
                        universityInit: [
                            'libraryService',function(libraryService) {
                                return libraryService.getUniversity();
                            }
                        ]
                    }
                },
                templateUrl: '/library/choosepartial/'
            }


            ];
        }
    }
})();