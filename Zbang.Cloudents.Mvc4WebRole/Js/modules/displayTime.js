(function (angular) {
    if (!String.prototype.format) {
        String.prototype.format = function () {
            var args = arguments;
            return this.replace(/{(\d+)}/g, function (match, number) {
                return typeof args[number] != 'undefined'
                  ? args[number]
                  : match
                ;
            });
        };
    }

    angular.module('displayTime', []).

        factory('displayTimeService', ['$interval', '$rootScope',
            function ($interval, $rootScope) {

                var updateTime = 'updateTime';


                var timeAgoTick = function () {
                    $rootScope.$broadcast(updateTime);
                };

                $interval(function () {
                    timeAgoTick();
                }, 60000);

                return {
                    timeAgoTick: timeAgoTick,

                    onTimeAgo: function ($scope, handler) {
                        $scope.$on(updateTime, function () {
                            handler();
                        });
                    }
                }
            }
        ]).
        directive('displayTime',
        ['$log', '$filter', 'displayTimeService',
        function ($log, $filter, displayTimeService) {
            return {
                restrict: "A",
                link: function (scope, elem, attrs) {

                    var date = new Date(attrs.displayTime);

                    if (!isDate(date)) {
                        $log.error('invalid date' + elem[0].className);
                        return;
                    }

                    var updateTime = function () {
                        elem[0].textContent = $filter('displayTimeFilter')(attrs.displayTime);
                    };

                    displayTimeService.onTimeAgo(scope, updateTime);
                    updateTime();

                    function isDate(date) {
                        return date instanceof Date && !isNaN(date.valueOf());
                    }
                }

            };
        }
        ]).filter('displayTimeFilter',[
        function () {
            return function (date) {

                var date = new Date(date),
                   oneDay = 86400000,
                   today = new Date(),
                   jsResources = window.JsResources,
                   dateDifference = calculateDayDifference(),
                   months = [jsResources.January, jsResources.February, jsResources.March, jsResources.April,
                       jsResources.May, jsResources.June, jsResources.July, jsResources.August,
                       jsResources.September, jsResources.October, jsResources.November, jsResources.December];


                switch (dateDifference) {
                    case 0:
                        var timeObj = calculateSecondsDifferece();
                        if (timeObj.hours >= 1) {
                            return jsResources.HoursAgo.format(Math.round(timeObj.hours));
                        }
                        if (timeObj.minutes >= 1) {
                            return jsResources.MinAgo.format(Math.round(timeObj.minutes));

                        }

                        return jsResources.JustNow;
                    case 1:
                        return jsResources.Yesterday;

                    default:
                        var dateMonth = date.getMonth() + 1,
                            todayMonth = today.getMonth() + 1;

                        if (dateMonth < todayMonth) {
                            return date.getDate() + ' ' + months[dateMonth - 1];
                        } else if (dateMonth > todayMonth) {
                            return date.getDate() + ' ' + months[dateMonth - 1] + ', ' + date.getFullYear();
                        } else if (today.getYear() > date.getYear()) {
                            return date.getDate() + ' ' + months[dateMonth - 1] + ', ' + date.getFullYear();
                        } else {
                            return dateDifference + ' days ago';
                        }
                }

                function calculateDayDifference() {
                    return Math.round(Math.abs((date.getTime() - today.getTime()) / (oneDay)));
                }
                function calculateSecondsDifferece() {
                    var time1 = date.getTime(),
                        time2 = today.getTime();

                    var timeDifference = time2 - time1;
                    return {
                        seconds: (timeDifference / 1000) % 60,
                        minutes: (timeDifference / (1000 * 60)) % 60,
                        hours: (timeDifference / (1000 * 60 * 60)) % 24
                    }
                }

            };
        }
        ]);
})(window.angular);