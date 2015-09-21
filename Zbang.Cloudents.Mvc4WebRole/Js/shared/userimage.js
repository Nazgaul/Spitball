(function() {
    angular.module('app').directive('userImage',
    ['$compile',
    function ($compile) {
        "use strict";
        var imageTemplate = '<img alt=""  />',
            letterTemplate = '<span class="userLetter"></span>',
            emptyTemplate = '<img class="userImg" src="/images/user.svg" />';
        return {
            restrict: "E",
            scope: {
                id: '=',
                name: '=',
                image: '=',
                noremove : '='
            },
            link: function (scope, elem, attrs) {
                var innerEl, className;

                if (scope.image) {
                    scope.image === true ? letter() : image();                    

                } else if (scope.name) {
                    letter();
                } else {
                    emptyState();
                 
                }

                //if (scope.id && sUserDetails.getDetails().id != scope.id) {
                //    innerEl.attr('user-tooltip-popup', scope.id);
                //}

                //if (attrs.tooltip) {
                //    innerEl.attr('tooltip', attrs.tooltip);
                //}

                var el = $compile(innerEl)(scope);

                elem.after(el);
                //scope.$destroy();

                if (scope.noremove) {
                    return;
                }

                elem.remove();

                function image() {
                    innerEl = angular.element(imageTemplate);
                    className = elem[0].getAttribute('class');
                    if (className) {
                        innerEl.addClass(className);
                    }
                    innerEl[0].height = elem[0].getAttribute('height');
                    innerEl[0].width = elem[0].getAttribute('width');

                    innerEl[0].src = scope.image;
                }

                function letter() {
                    innerEl = angular.element(letterTemplate);
                    className = elem[0].getAttribute('class');
                    if (className) {
                        innerEl.addClass(className);
                    }
                    innerEl.css({ width: elem[0].getAttribute('width'), height: elem[0].getAttribute('height'), lineHeight: elem[0].getAttribute('height') + 'px' });
                    innerEl.text(scope.name[0]);
                    //innerEl[0].setAttribute('data-letter', scope.name[0]);
                }

                function emptyState() {
                    innerEl = angular.element(emptyTemplate);
                    className = elem[0].getAttribute('class');
                    if (className) {
                        innerEl.addClass(className);
                    }

                    innerEl[0].height = elem[0].getAttribute('height');
                    innerEl[0].width = elem[0].getAttribute('width');
                }
            }
        };
    }
    ]);
})();