"use strict";
(function () {
    //angular.module('textAngular');
    angular.module('app.quiz.create', ['textAngular', 'app.ajaxservice']);
})();
(function () {
    "use strict";
    angular.module('textAngular').config(config);
    config.$inject = ['$provide'];
    function config($provide) {
        $provide.decorator('taOptions', ['taRegisterTool', '$delegate', '$q', '$stateParams',
            function (taRegisterTool, taOptions, $q, $stateParams) {
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
                taOptions.defaultFileDropHandler = function (file, insertAction) {
                    var dfd = $q.defer();
                    var client = new XMLHttpRequest();
                    client.onreadystatechange = function () {
                        if (client.readyState === 4 && client.status === 200) {
                            var response = JSON.parse(client.response);
                            if (!response.success) {
                                alert('Error');
                                return;
                            }
                            insertAction('insertImage', response.payload, true);
                            dfd.resolve();
                        }
                    };
                    var formData = new FormData();
                    formData.append(file.name, file);
                    formData.append("boxId", $stateParams.boxId);
                    client.open("POST", "/upload/quizimage/", true);
                    client.send(formData);
                    return dfd.promise;
                };
                return taOptions;
            }]);
    }
})();
//# sourceMappingURL=quizCreate.module.js.map