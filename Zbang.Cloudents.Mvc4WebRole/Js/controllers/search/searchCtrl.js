angular.module('mSearch', [])
    .controller('SearchCtrl',
['$scope',
'$location',
'$analytics',
'sSearch',
'$rootScope',
'searchHistory',
function ($scope, $location, $analytics, sSearch, $rootScope, searchHistory) {
    "use strict";

    var analyticsCategory = 'Search';

    $scope.params = {
        currentPage: 0,
        loading: false,
        lastPage: false
    };

    $scope.data = {
        boxes: [],
        quizzes: [],
        items: []
    };
  
    if (searchHistory.checkData()) {
        $scope.data = searchHistory.getData();
        $scope.params.currentPage = searchHistory.getPage();
    }

    $scope.search = function (isAppend) {
        if (isAppend) {
            search(appendMore);
            return;
        }

        $scope.params.noResults = false;
        $scope.params.currentPage = 0;
        $scope.data = {
            boxes: [],
            quizzes: [],
            items: []
        };
        searchHistory.clearData();

        search(appendFirstPage);
    };

    function search(parser) {

        if ($scope.params.loading) {
            return;
        }

        $scope.params.loading = true;

        var query = $scope.formData.query;

        $analytics.eventTrack('Search', {
            category: analyticsCategory,
            label: 'User searched for ' + query
        });


        if ($scope.params.currentPage === 0) {
            $analytics.searchTrack($location.$$path.replace(/\//g, ''), query, 'search page');
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
            $scope.params.noResults = true;
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

        searchHistory.setData($scope.data);
    }

    function checkEmptyResult(data) {
        if (data.boxes.length + /*data.quizzes.length +*/ data.items.length === 0) {
            return true;
        }

        return false;
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
           mData, mPage;


       service.setData = function (data) {
           mData = data;
           
       };

       service.setPage = function (page) {
           mPage = page;
       };
      
       service.getData = function () {
           return mData;
       };

       service.getPage = function () {
           return mPage;
       };

       service.clearData = function () {
           mData = mPage = null
       };

       service.checkData = function () {
           return _.isEmpty(mData) || mPage != 0;
       };

   });