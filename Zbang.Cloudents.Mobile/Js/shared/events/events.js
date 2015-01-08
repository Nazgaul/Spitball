﻿angular.module('app.events', []).
    directive('cursorEnd',
        function () {
            return function (scope, element, attr) {
                element.on('focus', function () {
                    setTimeout(function () {
                        element[0].setSelectionRange(element[0].value.length, element[0].value.length);
                    }, 10);
                });

                scope.$on('destroy', function () {
                    element.off('focus');
                });
            }
        }).
    directive('dragView', function () {
        return {
            restrict: "A",
            link: function (scope, element, attr) {
                var posX;

                element.on('touchstart', function (e) {
                    posX = e.touches[0].clientX;
                });
                element.on('touchmove', function (e) {

                    var curPosX = e.touches[0].clientX;

                    if (curPosX > posX) {
                        return;
                    }

                    if (Math.abs(curPosX - posX) >= 135) {
                        return;
                    }

                    element.css({ marginLeft: curPosX - posX + 'px' });

                });
                element.on('touchend', function (e) {
                    element.css({ marginLeft: 0 });
                });

                scope.$on('$destroy', function () {
                    element.off('touchstart');
                    element.off('tocuhmove');
                    element.off('touchend');
                });

            }
        }
    }).
    directive('focusInvalid', function () {
        return {
            restrict: "A",
            link: function (scope, elem) {

                // set up event handler on the form element
                elem.on('submit', function () {

                    // find the first invalid element
                    var firstInvalid = elem[0].querySelector('.ng-invalid');


                    // if we find one, set focus
                    if (firstInvalid) {
                        firstInvalid.focus();
                    }
                });

                scope.$on('$destroy', function () {
                    elem.off('submit');
                });

            }
        };
    }).directive('closeKeyboard', function () {
        return {
            restrict: "A",
            link: function (scope, elem) {

                // set up event handler on the form element
                elem.on('submit', function () {

                    elem[0].querySelector('input[type="search"]').blur(); //hack to close keyboard

                });

                scope.$on('$destroy', function () {
                    elem.off('submit');
                })
            }
        };
    }).
    directive('stickyForm', ['$window', function ($window) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var $win = angular.element($window);

                $win.on('scroll', function () {
                    var rectObj = element[0].getBoundingClientRect();
                    if (rectObj.top <= 0) {
                        element.addClass('sticky')
                        return;
                    }

                    element.removeClass('sticky')

                });


                scope.$on('destroy', function () {
                    $win.off('scroll');
                });
            }
        };
    }]).
    directive('focusComment', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.on('click',function(){                    
                    var parentElement = element[0].parentElement;
                    while (parentElement.nodeName !== 'LI') {
                        parentElement = parentElement.parentElement;
                    }

                    var textarea = parentElement.querySelector('textarea');
                    textarea.focus();
                });

                scope.$on('destroy', function () {
                    element.off('click');
                });
            }
        };
    });
