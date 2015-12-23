(function () {
    angular.module('app').directive('userImage', generateImage);

    generateImage.$inject = ['$compile'];

    function generateImage($compile) {
        "use strict";
        var imageTemplate = '<img alt="" />',
            letterTemplate = '<span class="userLetter background"></span>',
            emptyTemplate = '<img class="userImg" src="/images/user.svg" />';


        return {
            restrict: "E",
            scope: {
                id: '=',
                name: '=',
                image: '=',
                noremove: '='
            },
            //templateUrl: 'usericon.html'
            link: function (scope, elem) {

                function compile() {
                    var el = $compile(innerEl)(scope);

                    elem.after(el);
                    if (scope.noremove) {
                        return;
                    }
                    elem.remove();
                }
               
                //scope.$watch('name', function () {
                //    console.log('z');
                //});

                var innerEl, className;

                if (scope.image) {
                    scope.image === true ? letter() : image();

                } else if (scope.name) {
                    letter();
                } else {
                    emptyState();

                }
                if (scope.noremove) {
                    scope.$watch('image', function () {
                        if (scope.image) {
                            elem.next().remove();
                            image();
                            compile();
                        }

                        //elem.remove();
                    });
                }
                compile();
               

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
                    innerEl.attr('d-color', scope.name.length);
                    var borderWidth = parseInt(elem.css('border-width')) * 2;
                    var elementHeight = elem[0].getAttribute('height');


                    $(innerEl).css({
                        width: elem[0].getAttribute('width') + 'px',
                        height: elementHeight + 'px',
                        lineHeight: (elementHeight - borderWidth) + 'px'
                    });
                    innerEl.text(scope.name.trim()[0]);
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
})();