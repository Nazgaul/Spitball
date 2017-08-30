angular.module('mSearch', [])
    .controller('SearchCtrl',
['$scope',
'$location',
'$analytics',
'$timeout',
'sSearch',
'$rootScope',
function ($scope, $location, $analytics, $timeout, sSearch, $rootScope) {
    "use strict";

    var analyticsCategory = 'Search',
        firstTime = false,
        isSearchOpen;

    $scope.formData = {};
    $scope.params = {
        currentPage: 0,
        lastPage: false
    };
   
    resetData();
    resetDisplaySettings();


    $scope.search = function (isAppend) {
        if (!isSearchOpen) {
            return;
        }

        if (isAppend) {
            search(appendMore);
            return;
        }

        $scope.params.currentPage = 0;

        resetDisplaySettings();

        resetData();
       

        search(appendFirstPage);
    };    

    $scope.$on('search:toggle', function (e, isOpen) {
        isSearchOpen = isOpen;
        if (!isOpen) {            
            return;
        }

        if (firstTime) {
            return;
        }

        $scope.search();

    });

    function search(parser) {

        //if ($scope.params.loading) {
        //    return;
        //}

        var query = $scope.formData.query;

        if (query && query.length) {
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
            var regex = /(<([^>]+)>)/ig;
            data.boxes = _.map(data.boxes || [],function(box) {

                box.nameWithoutHighLight = box.name.replace(regex, "");
                return box;
            });
            data.quizzes = _.map(data.quizzes || [], function (quiz) {
                quiz.nameWithoutHighLight = quiz.name.replace(regex, "");
                return quiz;
            });
            data.items = _.map(data.items || [], function (item) {
                if (item.blobName) {
                    item.image = 'https://az779114.vo.msecnd.net/preview/' + item.blobName + '.jpg?width=63&height=88&mode=crop'; 
                }
                return item;
            });
            //image
            parser(data);
            $scope.params.currentPage++;
            
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
   }]);