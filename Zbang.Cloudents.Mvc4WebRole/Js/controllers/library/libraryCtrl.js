var mLibrary = angular.module('mLibrary', []);
mLibrary.controller('LibraryCtrl',
    ['$scope', '$rootScope', '$routeParams', '$timeout', '$modal', 'sUserDetails', 'sLibrary', 'sBox',
        function ($scope, $rootScope, $routeParams, $timeout, $modal, sUserDetails, sLibrary, sBox) {
            //cd.pubsub.publish('initLibrary');
            //cd.pubsub.publish('lib_nodes');//statistics
            //todo proper return;

            var jsResources = window.JsResources;

            var types = {
                box: 'box',
                department: 'department'
            }

            //#region data
            $scope.info = {
                libraryId: $routeParams.libraryId,
                libraryName: $routeParams.libraryName,
                items: [],
                currentPage: 0, pageSize: 50, paggingnNeeded: false
            }

            addItems();

            function addItems() {
                sLibrary.items({ section: $scope.info.libraryId, page: $scope.info.currentPage }).then(function (response) {
                    processData(response.payload);
                });
            }

            function processData(data) {
                var pageData;
                if (data.nodes.elem.length) {
                    pageData = data.nodes.elem;
                    $scope.info.type = types.department;
                }
                else if (data.boxes.elem.length) {
                    pageData = data.boxes.elem;
                    $scope.info.type = types.box;
                }

                if (pageData) {
                    $scope.info.items.push.apply($scope.info.items, pageData);
                }

                $timeout(function () {
                    $rootScope.$broadcast('viewContentLoaded');
                });

                if (pageData.length === $scope.info.pageSize) {
                    $scope.info.paggingnNeeded = true
                    return;
                }

                $scope.info.paggingnNeeded = false;
            }

            function pageUp() {
                $scope.info.currentPage++;
            }

            //#endregion

            //#region actions

            $scope.createBox = function () {
                var modalInstance = $modal.open({
                    //windowClass: "boxSettings dashMembers",
                    templateUrl: $scope.partials.createBox,
                    controller: 'CreateBoxLibCtrl',
                    backdrop: 'static',
                });

                modalInstance.result.then(function (result) {

                    result.parentId = $scope.info.libraryId;

                    sLibrary.box.create(result).then(function (response) {
                        //TODO: navigate to box
                    });
                }, function () {
                    //dismiss
                });
            };

            $scope.createDepartment = function () {
                var modalInstance = $modal.open({
                    //windowClass: "boxSettings dashMembers",
                    templateUrl: $scope.partials.createDepartment,
                    controller: 'CreateDepartmentCtrl',
                    backdrop: 'static',
                });

                modalInstance.result.then(function (result) {

                    result.parentId = $scope.info.libraryId;

                    sLibrary.department.create({}).then(function (response) {
                        $scope.info.items.push(response.payload);
                    });
                }, function () {
                    //dismiss
                });


            };

            $scope.deleteDepartment = function () {
                sLibrary.department.delete({ id: $scope.info.libraryId }).then(function (response) {
                    //TODO: nav to parent
                });
            };

            $scope.subscribe = function (box) {
                box.userType = 'subscribe';
                sBox.follow({ boxUid: box.id }).then(function (response) { }); //uid
                
                cd.postFb(box.name, jsResources.IJoined.format(box.name), location.href);
                cd.analytics.trackEvent('Follow', 'Follow', 'Clicking on follow button, on the departement level');
            };

            $scope.unsubscribe = function (box) {
                var isok = false,
                isDelete = box.userType === 'owner' || (box.membersCount <= 2 && box.commentCount < 2 && box.itemCount === 0);

                if (isDelete) {
                    isok = confirm(jsResources.DeleteCourse);
                }
                else {
                    isok = confirm(jsResources.SureYouWantTo + ' ' + jsResources.ToLeaveGroup);
                }
                if (!isok) {
                    return;
                }

                sBox.remove({ id: box.id }).then(function (response) {
                });

                box.userType = 'none';

                if (isDelete) {
                    var index = self.info.boxes.indexOf(box);
                    if (index > -1) {
                        self.info.boxes.splice(index, 1);
                    }
                    return;
                }

           
            };



            //#endregion

            //#region privileges

            $scope.isAdmin = function () {
                if (parseInt($scope.info.universityId, 10) === sUserDetails.getDetails().id) {
                    return true;
                }

                return false;
            };

            $scope.createDepartmentVisible = function () {
                if (!$scope.isAdmin()) {
                    return false;
                }

                if (!$scope.info.items.length) {
                    return true;
                }

                if ($scope.info.type === types.department) {
                    return true;
                }

                return false;
            };

            $scope.createBoxVisible = function () {
                if (!$scope.info.libraryId) {
                    return false;
                }

                if (!$scope.info.items.length) {
                    return false; //empty state
                }

                if ($scope.info.type === types.box) {
                    return true;
                }

                return false;
            };


            //#endregion


            //#region analytics
            $('.u-Website').click(function () {
                cd.analytics.trackEvent('Library', 'Go to org website', 'number of clicks on the union website icon');
            });
            $('.u-Fb').click(function () {
                cd.analytics.trackEvent('Library', 'Go to org Facebok page', 'number of clicks on the union facebook page icon');
            });
            cd.analytics.setLibrary($('.unionName').text());

            //#endregion

        }
    ]
);
