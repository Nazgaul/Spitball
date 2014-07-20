mLibrary.directive('facebookFeed',
   ['$rootScope','$window','$timeout',

   function ($rootScope,$window,$timeout) {
       return {
           restrict: "A",
           link: function (scope, elem, attrs) {

               
                       var href = attrs.href, link = attrs.link,
                        height = Math.floor($window.outerHeight -  352),
                        src = link.replace(/href=/i, 'href=' + href).replace(/height=/i, 'height=' + height);
                       height = Math.ceil(height / 10) * 10;
                       elem[0].src = src;
                       elem.height(height);
                       
               
               
               
           }
       };
   }
   ]);
