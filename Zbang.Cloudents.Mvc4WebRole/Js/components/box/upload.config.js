(function () {
    angular.module('app').config(config);

    config.$inject = ['$ocLazyLoadProvider'];

    function config($ocLazyLoadProvider) {
        $ocLazyLoadProvider.config({
            modules: [{
                name: 'upload',//angular-plupload',//, 'ang-drag-drop'',
                serie: true,
                files: [
                   {0}
                ]
            }]
        });
    }
})();
