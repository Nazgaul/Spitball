declare var a2a: any;

interface IAddToAnyScope extends ng.IScope {
    url: string
}

(() => {
    angular.module('app.box').directive('addToAny', addToAny);
    addToAny.$inject = ['$templateCache', '$timeout','$compile','$location'];

    function addToAny($templateCache: ng.ITemplateCacheService,
        $timeout: ng.ITimeoutService,
        $compile: ng.ICompileService,
        $location: ng.ILocationService): ng.IDirective {
        return {
            restrict: 'A',
            //template: ''
            link: (scope: IAddToAnyScope, element: JQuery) => {
                var template = $templateCache.get<string>('addToAnyTemplate.html');
                scope.url = $location.absUrl();
                element.append(template);
                $compile(element.contents())(scope);
                
                $timeout(() => {
                    a2a.init('page');
                });
                
                scope.$on('$stateChangeSuccess', () => {
                    element.empty();
                    
                });
              
            }
        }
    }
})();

