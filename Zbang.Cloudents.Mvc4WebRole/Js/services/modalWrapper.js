app.factory('sModal',
    ['$rootScope','$modal',' $modalStack',
        function ($rootScope,$modal, $modalStack) {
            //var modalsOpened = [];

            var modalList = {
                shareEmail: function(data){
                    var params = {
                        windowClass: 'invite',
                        templateUrl: '/Share/MessagePartial/',
                        controller: 'ShareCtrl',
                        backdrop: 'static',
                        resolve: {
                            data: function () {
                                return data;
                            }
                        }
                    };

                    return params;
                }
            };


            var service = {
                open: function(modalId,data,closeCb,dismissCb) {

                    var params = modalList[modalId](data),
                        modalInstance = $modal.open(params);

                    modalInstance.result.then(function(response){
                        if (angular.isFunction(closeCb)) {
                            closeCb(response);
                        }
                    },function(response) {
                        if (angular.isFunction(dismissCb)) {
                            dismissCb(response);
                        }
                    })['finally'](function () {
                        //var index = modalsOpened.indexOf(modalInstance);
                        //modalsOpened.splice(index,1);
                        modalInstance = undefined;

                    });

                    //modalsOpened.push(modalInstance);
                }
            };


            $rootScope.$on('$locationChangeStart', function(){
                $modalStack.dismissAll();
                //modalsOpened=[];

            });
        }]
);