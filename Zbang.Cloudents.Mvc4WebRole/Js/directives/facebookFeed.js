mLibrary.directive('facebookFeed',
   ['$window',

   function ($window) {
       return {
           restrict: "A",
           link: function (scope, elem, attrs) {

               setIFrame();

               $window.onresize = setIFrame;

               function setIFrame() {
                   var href = attrs.href, link = attrs.link,
                   height = Math.floor($window.innerHeight - 352 - 15),
                   src = link.replace(/href=/i, 'href=' + href).replace(/height=/i, 'height=' + height);

                   height = Math.ceil(height / 10) * 10;
                   elem[0].src = src;

                   elem.height(height);
               }
           }
       };
   }
   ]);