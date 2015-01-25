app.directive('appFilereader', ['$routeParams', function ($routeParams) {
    return {
        restrict: 'A',
        scope: {
            onChoose: '&'
        },
        link: function (scope, element, attrs) {
            element.bind('change', function (e) {
                e.preventDefault();
                var element = e.target;

                var files = Array.prototype.slice.call(element.files, 0);
                files.map(uploadFile);

                
                return false;

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
                        }
                    }

                    var formData = new FormData();
                    formData.append(file.name, file);
                    formData.append("boxId", $routeParams.boxId);
                    client.open("POST", "/upload/quizimage/", true);                    
                    client.send(formData);

                }

            });//change

        }//link

    };//return

}])//appFilereader
;