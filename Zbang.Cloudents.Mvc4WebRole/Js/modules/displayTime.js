﻿angular.module('displayTime', []).
    factory('displayTimeService', ['$timeout', '$rootScope',
        function ($timeout, $rootScope) {
            $timeout(function () {
                $rootScope.$broadcast('updateTime');
            }, 60000);                      
        }
    ]).
    directive('displayTime',
    ['$log', '$filter', '$rootScope',
    function ($log, $filter, $rootScope) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {

                var date = new Date(attrs.displayTime);

                if (!_.isDate(date)) {
                    $log.error('invalid date' + elem[0].className);
                    return;
                }

                scope.$on('updateTime', function () {
                    elem[0].textContent = $filter('displayTimeFilter')(attrs.displayTime);
                });

                elem[0].textContent = $filter('displayTimeFilter')(attrs.displayTime);



            }

        };
    }
    ]).filter('displayTimeFilter',
    ['$filter',
    function ($filter) {
        return function (date) {

            var date = new Date(date),
               oneDay = 86400000,
               today = new Date(),
               jsResources = JsResources,
               dateDifference = calculateDayDifference(),
               months = [jsResources.January, jsResources.February, jsResources.March, jsResources.April,
                   jsResources.May, jsResources.June, jsResources.July, jsResources.August,
                   jsResources.September, jsResources.October, jsResources.November, jsResources.December];


            switch (dateDifference) {
                case 0:
                    var timeObj = calculateSecondsDifferece();
                    if (timeObj.hours >= 1) {
                        return $filter('stringFormat')(jsResources.HoursAgo, [Math.round(timeObj.hours)]);
                    }
                    if (timeObj.minutes >= 1) {
                        return $filter('stringFormat')(jsResources.MinAgo, [Math.round(timeObj.minutes)]);
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
                    } else {
                        return $filter('stringFormat')(jsResources.DaysAgo, [dateDifference]);
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