(function () {
    angular.module('app').config(config);

    config.$inject = ['$ocLazyLoadProvider'];

    function config($ocLazyLoadProvider) {
        $ocLazyLoadProvider.config({
            modules: [{
                name: 'box',
                serie: true,
                files: [
                   '/bower_components/plupload/js/moxie.min.js','/bower_components/plupload/js/plupload.dev.js','/bower_components/angular-plupload/src/angular-plupload.js','/scripts/draganddrop.js','/js/components/box/box.module.js','/js/components/box/box.controller.js','/js/components/box/tab.controller.js','/js/components/box/shareBox.directive.js','/js/components/box/feed.controller.js','/js/components/box/feed.likes.controller.js','/js/components/box/item.controller.js','/js/components/box/quizzes.controller.js','/js/components/box/members.controller.js','/js/components/box/recommended.controller.js','/js/components/box/slideit.directive.js','/js/components/item/upload.controller.js','/js/components/item/externalProviderUpload.service.js'
                ]
            }]
        });
    }
})();
