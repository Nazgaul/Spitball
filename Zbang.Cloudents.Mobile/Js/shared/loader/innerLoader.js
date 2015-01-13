﻿angular.module('app').directive('innerScroll',
   [function () {

       return {
           restrict: "A",

           link: function (scope, element, attr) {

               var loader = angular.element('<svg class="svg-loading" id="loading" viewBox="0 14 32 18" preserveAspectRatio="none"> \
                   <path opacity="0.8" transform="translate(21.4331 0)" d="M2 14 V18 H6 V14z"><animateTransform attributeName="transform" \
                    type="translate" values="0 0; 24 0; 0 0" dur="2s" begin="0" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" \
                    calcMode="spline" /></path><path opacity="0.5" transform="translate(19.8224 0)" d="M0 14 V18 H8 V14z"><animateTransform attributeName="transform" \
                    type="translate" values="0 0; 24 0; 0 0" dur="2s" begin="0.1s" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" calcMode="spline" /> \
                    </path><path opacity="0.25" transform="translate(17.8416 0)" d="M0 14 V18 H8 V14z"><animateTransform attributeName="transform" type="translate" \
                    values="0 0; 24 0; 0 0" dur="2s" begin="0.2s" repeatCount="indefinite" keySplines="0.2 0.2 0.4 0.8;0.2 0.2 0.4 0.8" calcMode="spline" /></path></svg>');
               element.append(loader);
               element.css({ zIndex: 10 });
               attr.$observe('innerScroll', function () {
                   if (attr.innerScroll === 'true') {
                       setTimeout(function () {
                           element.css({ display: 'block' });
                           if (window.pageYOffset > 0) {
                               element.css({ position: 'fixed', bottom: '-28px' });
                           }
                       }, 50);

                       
                       return;
                   }
                   setTimeout(function () {
                       element.empty().css({ display: 'none' });
                   }, 50);
                   
               });

           }
       }
   }]);