angular.module('app').directive('innerScroll',
   [function () {

       return {
           restrict: "A",

           link: function (scope, element, attr) {

               var loader = angular.element('<svg id="loading" viewBox="0 14 32 18" width="32" height="4" fill="#f20" preserveAspectRatio="none"><path opacity="0.8" transform="translate(21.4331 0)" d="M2 14 V18 H6 V14z"><animateTransform attributeName="transform" type="translate" values="0 0; 24 0; 0 0" dur="2s" begin="0" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" calcMode="spline" /></path><path opacity="0.5" transform="translate(19.8224 0)" d="M0 14 V18 H8 V14z"><animateTransform attributeName="transform" type="translate" values="0 0; 24 0; 0 0" dur="2s" begin="0.1s" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" calcMode="spline" /></path><path opacity="0.25" transform="translate(17.8416 0)" d="M0 14 V18 H8 V14z"><animateTransform attributeName="transform" type="translate" values="0 0; 24 0; 0 0" dur="2s" begin="0.2s" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" calcMode="spline" /></path></svg>');
               element.append(loader);

               attr.$observe('innerScroll', function () {
                   if (attr.innerScroll === 'true') {
                       loader.css({ display: 'block' });
                       if (window.pageYOffset > 0) {
                           loader.css({ position: 'fixed', top: 0 });
                       }
                       return;
                   }
                   //loader.css({ display: 'none' });
               });

               scope.$on('$destroy', function () {
               });
           }
       }
   }]);