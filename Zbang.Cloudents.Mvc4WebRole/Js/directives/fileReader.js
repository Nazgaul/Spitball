app.directive('appFilereader', ['$routeParams', function ($routeParams) {
    return {
        restrict: 'A',
        scope: {
            onChoose: '&'
        },
        link: function (scope, element, attrs) {
            element.on('change', function (e) {
                e.preventDefault();
                var input = e.target;

                var files = Array.prototype.slice.call(input.files, 0);
                files.map(uploadFile);


                return false;               
            });//change

            scope.$on('$destroy', function () {
                element.off('change');
            });

            scope.$on('filePaste', function (e, data) {
                uploadFile(data);
            });

            function uploadFile(file) {

                var client = new XMLHttpRequest();
                client.onreadystatechange = function () {
                    if (client.readyState == 4 && client.status == 200) {
                        var response = JSON.parse(client.response);
                        if (!response.success) {
                            alert('Error');
                            return;
                        }

                        scope.onChoose({ data: response.payload });
                        element.val('');
                    }
                }

                var formData = new FormData();
                formData.append(file.name || 'filename', file);
                formData.append("boxId", $routeParams.boxId);
                client.open("POST", "/upload/quizimage/", true);
                client.send(formData);

            }

        }//link

    };//return

}]).
    directive('appFilePaste', [function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var textarea = element[0].querySelector('[contenteditable]');
                if (window.b) {
                    return;
                }
                window.b = true;
                textarea.onpaste = function (e) {
                    if (e.clipboardData.types.indexOf('text/plain') > -1) {
                        return true;
                    }
                    handlepaste(this, e);
                }

                function handlepaste(elem, e) {
                    var savedcontent = elem.innerHTML;
                    if (e && e.clipboardData && e.clipboardData.getData) {// Webkit - get data from clipboard, put into editdiv, cleanup, then cancel event                        
                        var file = e.clipboardData.items[0].getAsFile();                                                
                        scope.$broadcast('filePaste', file);
                    }
                }

                scope.$on('$destroy', function () {
                    textarea.onpaste = null;
                });

            }//link

        };//return

    }]);



