angular.module('mSearch', [])
    .controller('SearchCtrl',
['$scope',
'$location',
'$analytics',
'sSearch',
function ($scope, $location, $analytics, sSearch) {
    "use strict";
    $scope.params = {
        currentPage: 0,
        loading: false
    };

    var analyticsCategory = 'Search';

    $scope.data = {
        boxes: [],
        quizzes: [],
        items: []
    };
  
    $scope.search = function (isAppend) {
        if (isAppend) {
            search();
            return;
        }

        $scope.params.noResults = false;
        $scope.params.currentPage = 0;
        $scope.data = {
            boxes: [],
            quizzes: [],
            items: []
        };

        search();
    };

    function search() {

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
            appendData(data);
            $scope.params.currentPage++;
        }).finally(function () {
            $scope.params.loading = false;
        });
    }
    
    function appendData(data) {
        data.boxes = data.boxes || [];
        data.quizzes = data.quizzes || [];
        data.items = data.items || [];

        if (data.boxes.length + /*data.quizzes.length +*/ data.items.length === 0) {
            $scope.data.empty = true;
        }

        $scope.data.boxes = _.union($scope.data.boxes, data.boxes);
        $scope.data.quizzes = _.union($scope.data.quizzes, data.quizzes);
        $scope.data.items = _.union($scope.data.boxes, data.items);
    }

    }]
).directive('showMore',
   [function () {

       return {
           restrict: "A",
           scope: {
               onScroll: '&'
           },
           link: function (scope, element, attr) {
               var $body = angular.element(document.body);

               $body.on('scroll', isTriggerFunc);

               scope.$on('$destroy', function () {
                   $body.off('scroll', isTriggerFunc);
               });

               function isTriggerFunc() {
                   var scrollTop = document.body.scrollTop,
                       scrollHeight = document.body.scrollHeight,
                       windowHeight = window.innerHeight;

                   if (scrollTop + windowHeight >= scrollHeight * 0.8) {
                       scope.$apply(scope.onScroll);
                   }
               }
           }
       }
   }]);