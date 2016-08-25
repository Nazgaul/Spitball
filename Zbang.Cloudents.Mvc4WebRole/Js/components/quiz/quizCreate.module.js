(function () {
    //angular.module('textAngular');
    angular.module('app.quiz.create', ['textAngular', 'app.ajaxservice']);

})();


(function () {
    angular.module('textAngular').config(config);
    config.$inject = ['$provide'];
    function config($provide) {
        $provide.decorator('taOptions', ['taRegisterTool', '$delegate', '$q', '$stateParams',
            (taRegisterTool, taOptions, $q, $stateParams) => {
                taOptions.forceTextAngularSanitize = false;

                var buttons = [['fontUp', 'fontDown'],
                    ['bold', 'italics', 'underline'],
                    ['justifyLeft', 'justifyCenter', 'justifyRight'],
                    ['ol', 'ul'],
                    ['insertImage'],
                    ['redo', 'undo']];

                if (Modernizr.inputtypes.color) {
                    buttons[1].push('color');
                }
                taOptions.toolbar = buttons;


                taOptions.defaultFileDropHandler = (file, insertAction) => {
                    var dfd = $q.defer();
                    var client = new XMLHttpRequest();
                    client.onreadystatechange = () => {
                        if (client.readyState === 4 && client.status === 200) {
                            var response = JSON.parse(client.response);
                            if (!response.success) {
                                alert('Error');
                                return;
                            }
                            insertAction('insertImage', response.payload, true);
                            dfd.resolve();
                        }
                    }

                    var formData = new FormData();
                    formData.append(file.name, file);
                    formData.append("boxId", $stateParams.boxId);
                    client.open("POST", "/upload/quizimage/", true);
                    client.send(formData);

                    return dfd.promise;
                };
                return taOptions;
            }]);

        //function createSvgDisplay(svgNamge) {
        //    return '<button type="button" class="btn btn-default"> \
        //<svg><use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/textEditor.svg?26.0.0#' + svgNamge + '"></use></svg> \
        //</button>';
        //}

        //$provide.decorator('taTools', ['$delegate', function (taTools) {
        //    delete taTools.bold.iconclass;
        //    taTools.bold.display = createSvgDisplay('bold');
        //    return taTools;
        //}]);

    }
})();