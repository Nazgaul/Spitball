angular.module('dashboard').directive('toggleMenu',
   ['$analytics', function ($analytics) {
       return {
           restrict: "A",
           scope: false,
           link: function (scope, element, attr) {
               var $menuBtn = angular.element(document.getElementById('menuBtn')),                   
                   $dashboard = angular.element(document.getElementById('dashboard')),
                   menu = document.getElementById('sidemenu'),
                   menuOpened= false;

               $menuBtn.on('click', toggleMenu);               

               $dashboard.on('click', function (e) {
                   if (e.target === menu) {
                       return false;
                   }

                   if (isDescendant(menu, e.target)) {
                       return false;
                   }

                   if (menuOpened) {
                       return;
                   }

                   $analytics.eventTrack('Open sidebar', {
                       category: 'Dashboard page'
                   });

                   $dashboard.removeClass('menuOpen');
               });

               scope.$on('$destroy', function () {
                   $dashboard.off('click');
                   $menuBtn.off('click');
                   menuOpened = false;
               });

               function toggleMenu(e) {
                   e.preventDefault();
                   e.stopPropagation();

                   $analytics.eventTrack($dashboard.hasClass('menuOpen') ? 'Open' : 'Closed' + ' sidebar', {
                       category: 'Dashboard page'
                   });

                   $dashboard.toggleClass('menuOpen');
                   menuOpened = !menuOpened;
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