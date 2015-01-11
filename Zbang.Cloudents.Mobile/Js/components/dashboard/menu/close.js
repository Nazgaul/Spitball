angular.module('dashboard').directive('toggleMenu',
   ['$analytics', function ($analytics) {

       return {
           restrict: "A",
           scope: false,
           link: function (scope, element, attr) {
               var $body = angular.element(document.body),
                   $menuBtn = angular.element(document.getElementById('menuBtn')),
                   $closeBtn = angular.element(document.getElementById('closeMenuBtn')),
                   $dashboard = angular.element(document.getElementById('dashboard')),
                   menu = document.getElementById('sidemenu');

               $menuBtn.on('click', toggleMenu);
               $closeBtn.on('click', toggleMenu);

               $body.on('click', function (e) {
                   if (e.target === menu) {
                       return false;
                   }

                   if (isDescendant(menu, e.target)) {
                       return false;
                   }

                   $analytics.eventTrack('Open sidebar', {
                       category: 'Dashboard page'
                   });

                   $dashboard.removeClass('menuOpen');
               });

               scope.$on('$destroy', function () {
                   $body.off('click');
                   $menuBtn.off('click');
                   $closeBtn.off('click');
               });

               function toggleMenu(e) {
                   e.preventDefault();
                   e.stopPropagation();

                   $analytics.eventTrack($dashboard.hasClass('menuOpen') ? 'Open' : 'Closed' + ' sidebar', {
                       category: 'Dashboard page'
                   });

                   $dashboard.toggleClass('menuOpen');
               }

               function isDescendant(parent, child) {
                   var node = child.parentNode;
                   while (node != null) {
                       if (node == parent) {
                           return true;
                       }
                       node = node.parentNode;
                   }
                   return false;
               }


           }
       }
   }]);