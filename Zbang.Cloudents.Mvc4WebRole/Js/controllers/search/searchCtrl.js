angular.module('mSearch', [])
    .controller('SearchCtrl',
['$scope',
'$location',
'$analytics',
'$timeout',
'sSearch',
'$rootScope',
'searchHistory',
'sFocus',
function ($scope, $location, $analytics, $timeout, sSearch, $rootScope, searchHistory, sFocus) {
    "use strict";

    var analyticsCategory = 'Search';

    $scope.formData = {};
    $scope.params = {
        currentPage: 0,
        lastPage: false
    };


    resetDisplaySettings();
    resetData();
    sFocus('search:open');

    

    $scope.search = function (isAppend) {
        if (isAppend) {
            search(appendMore);
            return;
        }

        $scope.params.currentPage = 0;

        resetDisplaySettings();

        resetData();

        searchHistory.clearData();

        search(appendFirstPage);
    };
    if (searchHistory.checkData()) {
        $scope.formData.query = searchHistory.getQuery();
        $timeout(function() { $scope.$broadcast('search:select'); });
        $scope.data = searchHistory.getData();
        $scope.params.currentPage = searchHistory.getPage();
    } else {
        $scope.search();
    }

    function search(parser) {

        //if ($scope.params.loading) {
        //    return;
        //}

        var query = $scope.formData.query;

        if (query && query.length) {
            searchHistory.setQuery(query);
            $analytics.eventTrack('Search', {
                category: analyticsCategory,
                label: 'User searched for ' + query
            });

            if ($scope.params.currentPage === 0) {
                $analytics.searchTrack($location.$$path.replace(/\//g, ''), query, 'search page');
            } else {

                $analytics.eventTrack('Show more', {
                    category: 'Search',
                    label: 'User scrolled to page ' + $scope.params.currentPage

                });
            }

        }
          
        
        sSearch.searchByPage({ q: query, page: $scope.params.currentPage }).then(function (data) {
            data.boxes = data.boxes || [];
            data.quizzes = data.quizzes || [];
            data.items = data.items || [];
            parser(data);
            $scope.params.currentPage++;
            searchHistory.setPage($scope.params.currentPage);
        }).finally(function () {
            $scope.params.loading = false;
        });
    }
    function appendFirstPage(data) {
        if (checkEmptyResult(data)) {
            $scope.displaySettings = {
                noResults: true
            };
            return;
        }
        searchHistory.setData($scope.data);
        appendData(data);
    }

    function appendMore(data) {
        if (checkEmptyResult(data)) {
            $scope.params.lastPage = true;
        }

        appendData(data);
    }

    function appendData(data) {
        $scope.data.boxes = _.union($scope.data.boxes, data.boxes);
        $scope.data.quizzes = _.union($scope.data.quizzes, data.quizzes);
        $scope.data.items = _.union($scope.data.items, data.items);

        $scope.displaySettings = {
            boxes: $scope.data.boxes.length > 0,
            quizzes: $scope.data.quizzes.length > 0,
            items: $scope.data.items.length > 0
        };

        searchHistory.setData($scope.data);
    }

    function checkEmptyResult(data) {
        if (data.boxes.length + data.quizzes.length + data.items.length === 0) {
            return true;
        }

        return false;
    }

    function resetData() {
        $scope.data = {
            boxes: [],
            quizzes: [],
            items: []
        };
    }

    function resetDisplaySettings() {
        $scope.displaySettings = {
            boxes: true,
            quizzes: true,
            items: true
        };
    }

}]
).directive('showMore',
   ['$window', function ($window) {

       return {
           restrict: "A",
           scope: {
               onScroll: '&'
           },
           link: function (scope, element, attr) {
               var $win = angular.element($window);

               $win.on('scroll', isTriggerFunc);

               scope.$on('$destroy', function () {
                   $win.off('scroll', isTriggerFunc);
               });

               function isTriggerFunc() {
                   var scrollTop = $window.pageYOffset,
                       scrollHeight = document.body.scrollHeight,
                       windowHeight = window.innerHeight;

                   if (scrollTop + windowHeight >= scrollHeight * 0.8) {
                       scope.$apply(scope.onScroll);
                   }
               }
           }
       }
   }]).service('searchHistory', function () {
       var service = this,
           mData, mPage, mQuery;


       service.setData = function (data) {
           mData = data;
       };

       service.setPage = function (page) {
           mPage = page;
       };

       service.setQuery = function (query) {
           mQuery = query;
       };

       service.getData = function () {
           return mData;
       };

       service.getPage = function () {
           return mPage;
       };

       service.getQuery = function () {
           return mQuery;
       };


       service.clearData = function () {
           mData = mPage = mQuery = null;
       };

       service.checkData = function () {
           return !(_.isEmpty(mData)) && mPage != 0;
       };

   });