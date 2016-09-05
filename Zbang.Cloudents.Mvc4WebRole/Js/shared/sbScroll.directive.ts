module app {
    "use strict";
    class SbScroll implements angular.IDirective {
        constructor(private $compile: angular.ICompileService,
            private $mdMedia: angular.material.IMedia
            ) {
        }
       
        restrict = 'A';
        terminal = true;
        priority = 1000;
        link = (scope, element: JQuery, attribute: ng.IAttributes) => {
            element.removeAttr('sb-scroll');
            
            if (this.$mdMedia('xs')) {
                element.removeAttr('container');
            }
            if (Modernizr.cssscrollbar) {
                element.removeAttr("ng-scrollbars-config ng-scrollbars-paging-function ng-scrollbars");
                this.$compile(element)(scope);

                return;
            }
            element.removeAttr("srph-infinite-scroll");
            this.$compile(element)(scope);
        }

        static factory(): angular.IDirectiveFactory {
            const directive = ($compile, $mdMedia) => {
                return new SbScroll($compile, $mdMedia);
            };
            directive["$inject"] = ["$compile", "$mdMedia"];
            return directive;
        }
    }
    angular
        .module("app")
        .directive("sbScroll", SbScroll.factory());
}

module app {
    "use strict";
    class ChatScrollButtom implements angular.IDirective {
        constructor(private $mdMedia: angular.material.IMedia
        ) {
        }
        link = (scope: angular.IScope, element: JQuery, attribute: ng.IAttributes) => {
            scope.$on('chat-scroll',
                () => {
                    if (this.$mdMedia('xs')) {
                        window.scrollTo(0, document.body.scrollHeight);
                    }
                    if (Modernizr.cssscrollbar) {
                        element[0].scrollTop = element[0].scrollHeight;
                        return;
                    }
                    scope["c"].updateScrollbar('scrollTo', 'bottom', { scrollInertia: 0, timeout: 100 });        
            });
            
            
        }
        static factory(): angular.IDirectiveFactory {
            const directive = ($mdMedia) => {
                return new ChatScrollButtom( $mdMedia);
            };
            directive["$inject"] = ["$mdMedia"];
            return directive;
        }
    }
    angular
        .module("app")
        .directive("chatScrollButtom", ChatScrollButtom.factory());

}