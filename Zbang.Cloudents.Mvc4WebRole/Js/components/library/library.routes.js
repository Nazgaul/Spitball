'use strict';
(function () {
    angular.module('app.user').run(appRun);

    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());


        function getStates() {
            var partial = '/university/indexpartial/';
            return [
            {
                state: 'department',
                config: {
                    url: '/university/:universityId/:universityName/',
                    controller: 'Library as l',
                    resolve: {
                        nodeData: [
                           'libraryService','$stateParams', function (libraryService,$stateParams) {
                               return libraryService.getDepartments(null, $stateParams.universityId);
                           }
                        ],
                        universityData: [
                            'dashboardService', '$stateParams', function(dashboardService, $stateParams) {
                                return dashboardService.getUniversityMeta($stateParams.universityId);
                            }
                        ]
                    },
                    data: { animateClass: 'library' }
                },

                templateUrl: partial
            },
            {
                state: 'departmentWithNode', config: {
                    url: '/university/:universityId/:universityName/:nodeName/?id',
                    controller: 'Library as l',
                    resolve: {
                        nodeData: [
                           'libraryService', '$stateParams', function (libraryService, $stateParams) {
                               return libraryService.getDepartments($stateParams.id, $stateParams.universityId);
                           }
                        ],
                        universityData: [
                            'dashboardService', '$stateParams', function (dashboardService, $stateParams) {
                                return dashboardService.getUniversityMeta($stateParams.universityId);
                            }
                        ]
                    },
                    data: { animateClass: 'library' }
                },
                templateUrl: partial
            },
            {
                state: 'universityChoose', config: {
                    url: '/university/choose/',
                    controller: 'LibraryChoose as lc',
                    data: { animateClass: 'library-choose' }
                },
                templateUrl: '/university/choosepartial/'
            }


            ];
        }
    }
})();