module app {
    export interface IChatTimeAgo extends ng.IScope {
        fromTime: any;
        format: any;
    }
    class ChatTimeAgo implements angular.IDirective {
        scope = {
            fromTime: '@',
            format: '@'
        };
        restrict = 'EA';

        constructor(private timeAgo, private nowTime) {
        }
        link = (scope: IChatTimeAgo, element: ng.IAugmentedJQuery) => {
            /*handle all your linking requirements here*/

            var fromTime;

            // Track changes to fromTime
            scope.$watch('fromTime', () => {
                fromTime = this.timeAgo.parse(scope.fromTime);
            });

            // Track changes to time difference
            scope.$watch(() => {
                return this.nowTime() - fromTime;
            }, (value) => {
                const threeDaysInMilliseconds = 2.592e+8;
                if (value > threeDaysInMilliseconds) {
                    angular.element(element).text('');
                    return;
                }
                angular.element(element).text(this.timeAgo.inWords(value, fromTime, scope.format));
            });
        };
        public static factory(): angular.IDirectiveFactory {
            var directive = (timeAgo, nowTime) => {
                return new ChatTimeAgo(timeAgo, nowTime);
            };

            directive['$inject'] = ['timeAgo', 'nowTime'];

            return directive;
        }
    }

    angular
        .module("app.chat")
        .directive("chatTimeAgo", ChatTimeAgo.factory());
};


//(function () {
//    angular.module('app.chat').directive('chatTimeAgo2', timeAgo);

//    timeAgo.$inject = ['timeAgo', 'nowTime'];
//    function timeAgo(timeAgo, nowTime) {
//        return {
//            scope: {
//                fromTime: '@',
//                format: '@'
//            },
//            restrict: 'EA',
//            link: function (scope, elem) {
//                var fromTime;

//                // Track changes to fromTime
//                scope.$watch('fromTime', function () {
//                    fromTime = timeAgo.parse(scope.fromTime);
//                });

//                // Track changes to time difference
//                scope.$watch(function () {
//                    return nowTime() - fromTime;
//                }, function (value) {
//                    var threeDaysInMilliseconds = 2.592e+8;
//                    if (value > threeDaysInMilliseconds) {
//                        angular.element(elem).text('');
//                        return;
//                    }
//                    angular.element(elem).text(timeAgo.inWords(value, fromTime, scope.format));
//                });
//            }
//        };
//    }
//})();