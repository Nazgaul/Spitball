app.directive('userImage',
    ['$compile','sUserDetails',
    function ($compile,sUserDetails) {
        "use strict";
        var imageTemplate = '<img alt=""  />',
            letterTemplate = '<span></span>';
        return {
            restrict: "E",
            scope: {
                id: '=',
                name: '=',
                image: '='
            },
            link: function (scope, elem, attrs) {
                var innerEl, className, width, height, src;

                if (scope.image) {
                    innerEl = angular.element(imageTemplate);
                    className = elem[0].getAttribute('class-image');
                    if (className) {
                        innerEl.addClass(className);
                    }                    
                    innerEl[0].height= elem[0].getAttribute('height');                    
                    innerEl[0].width = elem[0].getAttribute('width');
                    
                    innerEl[0].src = scope.image;

                } else {
                    innerEl = angular.element(letterTemplate);
                    className = elem[0].getAttribute('class-letter');
                    if (className) {
                        innerEl.addClass(className);
                    }

                    innerEl[0].setAttribute('data-letter', scope.name[0]);
                }               

                if (scope.id && sUserDetails.getDetails().id != scope.id) {
                    innerEl.attr('user-tooltip-popup', scope.id);
                }

                var el = $compile(innerEl)(scope);

                elem.after(el);

                elem.remove();
            }
        };
    }
    ]);
