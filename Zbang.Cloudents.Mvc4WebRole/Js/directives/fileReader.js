app.directive('appFilereader', ['$routeParams', function ($routeParams) {    
    return {
        restrict: 'A',
        scope: {
            onChoose: '&'
        },
        link: function (scope, element, attrs) {
            element.bind('change', function (e) {
                var element = e.target;

                var files = Array.prototype.slice.call(element.files, 0);
                files.map(uploadFile);


                function uploadFile(file) {

                    var client = new XMLHttpRequest();                    
                    /* Check the response status */
                    client.onreadystatechange = function () {
                        if (client.readyState == 4 && client.status == 200) {
                            console.log(client);
                        }
                    }
                    upload();

                    function upload() {
                        /* Create a FormData instance */
                        var formData = new FormData();
                        /* Add the file */
                        formData.append("upload", file);
                        formData.append("boxId", $routeParams.boxId);
                        client.open("post", "/upload/quizimage/", true);
                        client.setRequestHeader("Content-Type", "multipart/form-data");
                        client.send(formData);  /* Send to server */
                    }                   
                }

            });//change

        }//link

    };//return

}])//appFilereader
;