"use strict";
/**
 * angular-timer - v1.1.6 - 2014-07-01 7:37 AM
 * https://github.com/siddii/angular-timer
 *
 * Copyright (c) 2014 Siddique Hameed
 * Licensed MIT <https://github.com/siddii/angular-timer/blob/master/LICENSE.txt>
 */
var timerModule = angular.module('timer', [])
  .directive('timer', ['$compile', function ($compile) {
      var oneHour = 3600000,
          thirty = 30,
          twelve = 12,
          year = 365,
          sixtyK = 60000,
          second = 1000,
          twentyFour = 24,
          sixty = 60;

      return {
          restrict: 'EAC',
          replace: false,
          scope: {
              interval: '=interval',
              startTimeAttr: '=startTime',
              endTimeAttr: '=endTime',
              countdownattr: '=countdown',
              finishCallback: '&finishCallback',
              autoStart: '&autoStart',
              maxTimeUnit: '='
          },
          controller: ['$scope', '$element', '$attrs', '$timeout', function ($scope, $element, $attrs, $timeout) {

              // Checking for trim function since IE8 doesn't have it
              // If not a function, create tirm with RegEx to mimic native trim
              if (typeof String.prototype.trim !== 'function') {
                  String.prototype.trim = function () {
                      return this.replace(/^\s+|\s+$/g, '');
                  };
              }

              //angular 1.2 doesn't support attributes ending in "-start", so we're
              //supporting both "autostart" and "auto-start" as a solution for
              //backward and forward compatibility.
              $scope.autoStart = $attrs.autoStart || $attrs.autostart;

              if ($element.html().trim().length === 0) {
                  $element.append($compile('<span>{{millis}}</span>')($scope));
              } else {
                  $element.append($compile($element.contents())($scope));
              }

              $scope.startTime = null;
              $scope.endTime = null;
              $scope.timeoutId = null;
              $scope.countdown = $scope.countdownattr && parseInt($scope.countdownattr, 10) >= 0 ? parseInt($scope.countdownattr, 10) : undefined;
              $scope.isRunning = false;

              $scope.$on('timer-start', function () {
                  $scope.start();
              });

              $scope.$on('timer-resume', function () {
                  $scope.resume();
              });

              $scope.$on('timer-stop', function () {
                  $scope.stop();
              });

              $scope.$on('timer-clear', function () {
                  $scope.clear();
                  $scope.millis = 0;
                  calculateTimeUnits();
              });

              $scope.$on('timer-set-countdown', function (e, countdown) {
                  $scope.countdown = countdown;
              });

              function resetTimeout() {
                  if ($scope.timeoutId) {
                      $timeout.cancel($scope.timeoutId);
                  }
              }

              $scope.start = $element[0].start = function () {
                  $scope.startTime = $scope.startTimeAttr ? new Date($scope.startTimeAttr) : new Date();
                  $scope.endTime = $scope.endTimeAttr ? new Date($scope.endTimeAttr) : null;
                  if (!$scope.countdown) {
                      $scope.countdown = $scope.countdownattr && parseInt($scope.countdownattr, 10) > 0 ? parseInt($scope.countdownattr, 10) : undefined;
                  }
                  resetTimeout();
                  tick();
                  $scope.isRunning = true;
              };

              $scope.resume = $element[0].resume = function () {
                  resetTimeout();
                  if ($scope.countdownattr) {
                      $scope.countdown += 1;
                  }
                  $scope.startTime = new Date() - ($scope.stoppedTime - $scope.startTime);
                  tick();
                  $scope.isRunning = true;
              };

              $scope.stop = $scope.pause = $element[0].stop = $element[0].pause = function () {
                  var timeoutId = $scope.timeoutId;
                  $scope.clear();
                  $scope.$emit('timer-stopped', { timeoutId: timeoutId, millis: $scope.millis, seconds: $scope.sseconds, minutes: $scope.mminutes, hours: $scope.hours, days: $scope.days,startTime:$scope.startTime, endTime: $scope.stoppedTime });
              };

              $scope.clear = $element[0].clear = function () {
                  // same as stop but without the event being triggered
                  $scope.stoppedTime = new Date();
                  resetTimeout();
                  $scope.timeoutId = null;
                  $scope.isRunning = false;                
              };

              $element.bind('$destroy', function () {
                  resetTimeout();
                  $scope.isRunning = false;
              });

              function calculateTimeUnits() {

                  // compute time values based on maxTimeUnit specification
                  if (!$scope.maxTimeUnit || $scope.maxTimeUnit === 'day') {
                      $scope.seconds = Math.floor(($scope.millis / second) % sixty);
                      $scope.minutes = Math.floor((($scope.millis / (sixtyK)) % sixty));
                      $scope.hours = Math.floor((($scope.millis / (oneHour)) % twentyFour));
                      $scope.days = Math.floor((($scope.millis / (oneHour)) / twentyFour));
                      $scope.months = 0;
                      $scope.years = 0;
                  } else if ($scope.maxTimeUnit === 'second') {
                      $scope.seconds = Math.floor($scope.millis / second);
                      $scope.minutes = 0;
                      $scope.hours = 0;
                      $scope.days = 0;
                      $scope.months = 0;
                      $scope.years = 0;
                  } else if ($scope.maxTimeUnit === 'minute') {
                      $scope.seconds = Math.floor(($scope.millis / second) % sixty);
                      $scope.minutes = Math.floor($scope.millis / sixtyK);
                      $scope.hours = 0;
                      $scope.days = 0;
                      $scope.months = 0;
                      $scope.years = 0;
                  } else if ($scope.maxTimeUnit === 'hour') {
                      $scope.seconds = Math.floor(($scope.millis / second) % sixty);
                      $scope.minutes = Math.floor((($scope.millis / (sixtyK)) % sixty));
                      $scope.hours = Math.floor($scope.millis / oneHour);
                      $scope.days = 0;
                      $scope.months = 0;
                      $scope.years = 0;
                  } else if ($scope.maxTimeUnit === 'month') {
                      $scope.seconds = Math.floor(($scope.millis / second) % sixty);
                      $scope.minutes = Math.floor((($scope.millis / (sixtyK)) % sixty));
                      $scope.hours = Math.floor((($scope.millis / (oneHour)) % twentyFour));
                      $scope.days = Math.floor((($scope.millis / (oneHour)) / twentyFour) % thirty);
                      $scope.months = Math.floor((($scope.millis / (oneHour)) / twentyFour) / thirty);
                      $scope.years = 0;
                  } else if ($scope.maxTimeUnit === 'year') {
                      $scope.seconds = Math.floor(($scope.millis / second) % sixty);
                      $scope.minutes = Math.floor((($scope.millis / (sixtyK)) % sixty));
                      $scope.hours = Math.floor((($scope.millis / (oneHour)) % twentyFour));
                      $scope.days = Math.floor((($scope.millis / (oneHour)) / twentyFour) % thirty);
                      $scope.months = Math.floor((($scope.millis / (oneHour)) / twentyFour / thirty) % twelve);
                      $scope.years = Math.floor(($scope.millis / (oneHour)) / twentyFour / year);
                  }

                  // plural - singular unit decision
                  $scope.secondsS = $scope.seconds == 1 ? '' : 's';
                  $scope.minutesS = $scope.minutes == 1 ? '' : 's';
                  $scope.hoursS = $scope.hours == 1 ? '' : 's';
                  $scope.daysS = $scope.days == 1 ? '' : 's';
                  $scope.monthsS = $scope.months == 1 ? '' : 's';
                  $scope.yearsS = $scope.years == 1 ? '' : 's';
                  //add leading zero if number is smaller than 10
                  $scope.sseconds = $scope.seconds < 10 ? '0' + $scope.seconds : $scope.seconds;
                  $scope.mminutes = $scope.minutes < 10 ? '0' + $scope.minutes : $scope.minutes;
                  $scope.hhours = $scope.hours < 10 ? '0' + $scope.hours : $scope.hours;
                  $scope.ddays = $scope.days < 10 ? '0' + $scope.days : $scope.days;
                  $scope.mmonths = $scope.months < 10 ? '0' + $scope.months : $scope.months;
                  $scope.yyears = $scope.years < 10 ? '0' + $scope.years : $scope.years;

              }

              //determine initial values of time units and add AddSeconds functionality
              if ($scope.countdownattr) {
                  $scope.millis = $scope.countdownattr * second;

                  $scope.addCDSeconds = $element[0].addCDSeconds = function (extraSeconds) {
                      $scope.countdown += extraSeconds;
                      $scope.$digest();
                      if (!$scope.isRunning) {
                          $scope.start();
                      }
                  };

                  $scope.$on('timer-add-cd-seconds', function (e, extraSeconds) {
                      $timeout(function () {
                          $scope.addCDSeconds(extraSeconds);
                      });
                  });

                  $scope.$on('timer-set-countdown-seconds', function (e, countdownSeconds) {
                      if (!$scope.isRunning) {
                          $scope.clear();
                      }

                      $scope.countdown = countdownSeconds;
                      $scope.millis = countdownSeconds * second;
                      calculateTimeUnits();
                  });
              } else {
                  $scope.millis = 0;
              }
              calculateTimeUnits();

              var tick = function () {

                  $scope.millis = new Date() - $scope.startTime;
                  var adjustment = $scope.millis % 1000;

                  if ($scope.endTimeAttr) {
                      $scope.millis = $scope.endTime - new Date();
                      adjustment = $scope.interval - $scope.millis % second;
                  }


                  if ($scope.countdownattr) {
                      $scope.millis = $scope.countdown * second;
                  }

                  if ($scope.millis < 0) {
                      $scope.stop();
                      $scope.millis = 0;
                      calculateTimeUnits();
                      if ($scope.finishCallback) {
                          $scope.$eval($scope.finishCallback);
                      }
                      return;
                  }
                  calculateTimeUnits();

                  //We are not using $timeout for a reason. Please read here - https://github.com/siddii/angular-timer/pull/5
                  $scope.timeoutId = $timeout(function () {
                      tick();
                  }, $scope.interval - adjustment);

                  $scope.$emit('timer-tick', { timeoutId: $scope.timeoutId, millis: $scope.millis });

                  if ($scope.countdown > 0) {
                      $scope.countdown--;
                  }
                  else if ($scope.countdown <= 0) {
                      $scope.stop();
                      if ($scope.finishCallback) {
                          $scope.$eval($scope.finishCallback);
                      }
                  }
              };

              if ($scope.autoStart === undefined || $scope.autoStart === true) {
                  $scope.start();
              }
          }]
      };
  }]);