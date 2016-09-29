(function () {
    angular.module('app').config(config);

    config.$inject = ['$ocLazyLoadProvider'];

    function config($ocLazyLoadProvider) {
        $ocLazyLoadProvider.config({
            modules: [{
                name: 'upload',//angular-plupload',//, 'ang-drag-drop'',
                serie: true,
                files: [
                   '/bower_components/plupload/js/moxie.min.js','/bower_components/plupload/js/plupload.dev.js','/bower_components/angular-plupload/src/angular-plupload.js','/scripts/draganddrop.js','/js/components/item/upload.controller.js'
                ]
            }]
        });
    }
})();
