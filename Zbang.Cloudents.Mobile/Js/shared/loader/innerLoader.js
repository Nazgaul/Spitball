angular.module('app').directive('innerScroll',
   [function () {

       return {
           restrict: "A",

           link: function (scope, element, attr) {

               var loader = angular.element('<svg class="svg-loading" viewBox="0 14 32 4" height="5px" preserveAspectRatio="none">\
                            <path opacity="0.8" transform="translate(21.4331 0)" d="M2 14 V18 H6 V14z">\
                                <animateTransform attributeName="transform" type="translate" values="0 0; 24 0; 0 0" dur="2s" begin="0" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" calcMode="spline" />\
                            </path>\
                            <path opacity="0.5" transform="translate(19.8224 0)" d="M0 14 V18 H8 V14z">\
                                <animateTransform attributeName="transform" type="translate" values="0 0; 24 0; 0 0" dur="2s" begin="0.1s" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" calcMode="spline" />\
                            </path>\
                            <path opacity="0.25" transform="translate(17.8416 0)" d="M0 14 V18 H8 V14z">\
                                <animateTransform attributeName="transform" type="translate" values="0 0; 24 0; 0 0" dur="2s" begin="0.2s" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" calcMode="spline" />\
                            </path>\
                       </svg>');

               element.append(loader);
               element.css({ zIndex: 10, width: '100%' });
               attr.$observe('innerScroll', function () {
                   if (attr.innerScroll === 'true') {
                       setTimeout(function () {
                           var scrollHeight = document.body.scrollHeight,
                                windowHeight = window.innerHeight;

                           if (scrollHeight === windowHeight) {
                               element.css({ display: 'block' });
                               return;

                           }

                           element.css({ display: 'block', position: 'fixed', bottom: 0 });
                       }, 50);
                       return;
                   }
                   setTimeout(function () {
                       element.css({ display: 'none', position: 'static', bottom: 'auto' });
                   }, 50);

               });

           }
       }
   }]);