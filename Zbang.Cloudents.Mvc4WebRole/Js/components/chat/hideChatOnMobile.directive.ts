module app {
    class HideChatOnMobile implements angular.IDirective {
        restrict = 'A';

        constructor(private $mdMedia) {
        }
        link = (scope: IChatTimeAgo, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
            if (this.$mdMedia('xs')) {
                angular.element(element).on(attrs['hideChatOnMobile'], () => {
                    $('.page-body').removeClass('expanded-chat');
                });
            }
        };


        public static factory(): angular.IDirectiveFactory {
            var directive = ($mdMedia) => {
                return new HideChatOnMobile($mdMedia);
            };

            directive['$inject'] = ['$mdMedia'];

            return directive;
        }
    }

    angular
        .module("app.chat")
        .directive("hideChatOnMobile", HideChatOnMobile.factory());
}