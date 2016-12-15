(function () {
    'use strict';
    angular.module('app').directive('userImage', generateImage);

    generateImage.$inject = ['$compile'];

    function generateImage($compile) {
        "use strict";
        var imageTemplate = '<img alt="" />',
            letterTemplate = '<span class="userLetter background color-override textColor1"></span>',
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
                var innerEl, className;

                 scope.$watchGroup(["image", "name"],
                     function (newVal, oldVal) {
                         if (scope.noremove) {
                             
                             //if (newVal[0] || newVal[1]) {
                             //    buildTemplate();
                             //}
                             //return;
                         }
                         console.log(elem, newVal, oldVal);
                         if (newVal[0] || newVal[1]) {
                             console.log(elem, newVal, oldVal);
                             buildTemplate();
                         }
                         //console.log(newVal, oldVal);
                     });



                function compile() {
                    var el = $compile(innerEl)(scope);

                    elem.after(el);
                    if (scope.noremove) {
                        return;
                    }
                    elem.remove();
                }


                function buildTemplate() {
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
                        });
                    }
                    compile();
                }

               


                function image() {
                    innerEl = angular.element(imageTemplate);
                    className = elem[0].getAttribute('class');
                    if (className) {
                        innerEl.addClass(className);
                    }

                    if (elem[0].getAttribute('name-title') !== null) {
                        innerEl.attr("title", scope.name);
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

                    if (elem[0].getAttribute('name-title') !== null) {
                        innerEl.attr("title", scope.name);
                    }
                    innerEl.attr('d-color', scope.name.length);
                    //var borderWidthValue = elem.css('border-width');
                    var borderWidth = 0;
                    //if (borderWidthValue) {
                    //borderWidth = parseInt(borderWidthValue, 10) * 2;
                    //}
                    var elementHeight = elem[0].getAttribute('height');

                    innerEl[0].style.width = elem[0].getAttribute('width') + 'px';
                    innerEl[0].style.height = elementHeight + 'px';
                    innerEl[0].style.lineHeight = (elementHeight - borderWidth) + 'px';

                    //$(innerEl).css({
                    //    width: elem[0].getAttribute('width') + 'px',
                    //    height: elementHeight + 'px',
                    //    lineHeight: (elementHeight - borderWidth) + 'px'
                    //});
                    innerEl.text(scope.name.trim()[0].toUpperCase());
                }

                function emptyState() {
                    innerEl = angular.element(emptyTemplate);
                    className = elem[0].getAttribute('class');
                    if (className) {
                        innerEl.addClass(className);
                    }

                    if (elem[0].getAttribute('name-title') !== null) {
                        innerEl.attr("title", scope.name);
                    }

                    innerEl[0].height = elem[0].getAttribute('height');
                    innerEl[0].width = elem[0].getAttribute('width');
                }
            }
        };
    }
})();