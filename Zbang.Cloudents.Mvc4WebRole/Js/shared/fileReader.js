
(function () {
    'use strict';
    //TODO: on quiz directory
    angular.module('app.quiz').directive('appFilereader', appFilereader);
    appFilereader.$inject = ['$stateParams'];
    function appFilereader($stateParams) {
            return {
                restrict: 'A',
                scope: {
                    onChoose: '&'
                },
                link: function(scope, element) {
                    element.on('change', function(e) {
                        e.preventDefault();
                        var input = e.target;

                        var files = Array.prototype.slice.call(input.files, 0);
                        files.map(uploadFile);


                        return false;
                    }); //change

                    scope.$on('$destroy', function() {
                        element.off('change');
                    });

                    scope.$on('filePaste', function(e, data) {
                        uploadFile(data);
                    });

                    function uploadFile(file) {

                        if (file.size > 20971520) {
                            alert('Maximum 20MB');
                            return;
                        }

                        var client = new XMLHttpRequest();
                        client.onreadystatechange = function() {
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
                        formData.append("boxId", $stateParams.boxId);
                        client.open("POST", "/upload/quizimage/", true);
                        client.send(formData);

                    }

                } //link

            }; //return

        }
    
})();

//(function() {

//    angular.module('app.quiz').directive('appFilePaste', appFilePaste);
//    appFilePaste.$inject = ['$routeParams'];
//    function appFilePaste() {
    
   
//        return {
//            restrict: 'A',
//            link: function (scope, element, attrs) {
//                var textarea = element[0].querySelector('[contenteditable]');

//                textarea.onpaste = function (e) {
//                    if (e.clipboardData.types.indexOf('text/plain') > -1) {
//                        return true;
//                    }
//                    var blob;

//                    if (e && e.clipboardData && e.clipboardData.getData) {// Webkit - get data from clipboard, put into editdiv, cleanup, then cancel event                        
//                        blob = e.clipboardData.items[0].getAsFile();
//                    }

//                    setTimeout(function(){
//                        handlepaste(blob);
//                    },150)
//                }

//                function handlepaste(blob) {
//                   // var savedcontent = textarea.innerHTML;
//                    scope.$broadcast('filePaste', blob);                    
//                }

//                scope.$on('$destroy', function () {
//                    textarea.onpaste = null;
//                });

//            }//link

//        };//return

//    };


//})();