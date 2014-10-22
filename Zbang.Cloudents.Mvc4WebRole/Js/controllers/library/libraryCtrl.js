var mLibrary = angular.module('mLibrary', []);
mLibrary.controller('LibraryCtrl',
    ['$scope', '$location', '$routeParams', '$timeout', '$modal', 'sUserDetails', 'sLibrary', 'sBox', '$rootScope', '$analytics',
function ($scope, $location, $routeParams, $timeout, $modal, sUserDetails, sLibrary, sBox, $rootScope, $analytics) {

    var jsResources = window.JsResources;

    var types = {
        box: 'box',
        department: 'department',
        empty: 'empty'
    }

    //#region data
    $scope.info = {
        libraryId: $routeParams.libraryId,
        libraryName: $routeParams.libraryName,
        isRootLevel: !$routeParams.libraryId,
        items: []
    };

    var partials = {
        //    //createAcademicBox: '/Library/CreateAcademicBoxPartial/',
        createDepartment: '/Library/CreateDepartmentPartial/'
    }

    addItems();

    function addItems() {
        sLibrary.items({ section: $scope.info.libraryId }).then(function (response) {
            processData(response.payload);
        });
    }

    function processData(data) {
        var pageData;
        if (data.nodes && data.nodes.length) {
            pageData = data.nodes;
            $scope.info.type = types.department;
        }
        else if (data.boxes && data.boxes.length) {
            pageData = data.boxes;
            $scope.info.type = types.box;
        } else {
            pageData = null;
            $scope.info.type = types.empty;
        }

        if (!$scope.info.isRootLevel) {
            $scope.back.title = data.details.name;
            $scope.back.url = data.details.parentUrl;
        }
        //if ($scope.info.type === types.empty) {
        //    return;
        //}

        if (pageData) {
            $scope.info.items.push.apply($scope.info.items, pageData);
        }
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });

        //if (pageData.length === $scope.info.pageSize) {
        //    $scope.info.paggingnNeeded = true;
        //    return;
        //}

        //$scope.info.paggingnNeeded = false;
    }

    //#endregion

    //#region actions

    $scope.createBox = function () {
        $rootScope.params.createBoxWizard = true;
        var modalInstance = $modal.open({
            templateUrl: '/Dashboard/CreateBox/',
            controller: 'CreateBoxWizardCtrl',
            backdrop: false,
            keyboard: false,
            resolve: {
                data: function () {
                    return {
                        isAcademic: true,
                        department: {
                            id: $scope.info.libraryId,
                            name: $scope.info.libraryName
                        }
                    }
                }
            }
        });
        modalInstance.result.then(function (response) {

            $rootScope.params.createBoxWizard = false;
            if (response) {
                $location.path(response.url);
                if (response.isItems) {
                    $location.hash('items');
                }
            }
        }, function () {
            $rootScope.params.createBoxWizard = false; //user cancelled
        })['finally'](function () {
            modalInstance = undefined;
        });

        $scope.$on('$destroy', function () {
            if (modalInstance) {
                modalInstance.dismiss();
                modalInstance = undefined;
            }
        });
    };

    $scope.createDepartment = function () {
        var modalInstance = $modal.open({
            windowClass: "boxSettings dashMembers",
            templateUrl: partials.createDepartment,
            controller: 'CreateDepartmentCtrl',
            backdrop: 'static',
        });

        modalInstance.result.then(function (result) {
            result.parentId = $scope.info.libraryId;

            var item = _.find($scope.info.items, function (item2) {
                return item2.name === result.name;
            });

            if (item) {
                alert('already exists');
                return;
            }

            sLibrary.createDepartment(result).then(function (response) {
                $scope.info.items.push(response.payload);
                $scope.info.type = types.department;
            });
        });


    };

    //$scope.deleteDepartment = function () {
    //    sLibrary.department.delete({ id: $scope.info.libraryId }).then(function (response) {
    //        TODO: nav to parent
    //    });
    //};

    $scope.subscribe = function (box) {
        box.userType = 'subscribe';
        sBox.follow({ boxId: box.id });
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

        sBox.remove({ id: box.id });

        box.userType = 'none';

        if (isDelete) {
            var index = $scope.info.items.indexOf(box);
            if (index > -1) {
                $scope.info.items.splice(index, 1);
            }
            return;
        }
    };



    //#endregion

    //#region privileges

    $scope.isAdmin = function () {
        return sUserDetails.getDetails().isAdmin;
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

        if (!$scope.info.type) {
            return false;
        }
        if ($scope.info.isRootLevel) {
            return false;
        }

        if ($scope.info.type === types.department) {
            return false;
        }
        return true;
    };

    $scope.renameWindow = function () {
        var modalInstance = $modal.open({
            windowClass: 'deptSettings',
            templateUrl: '/Library/Rename/',
            controller: 'libraryRenameCtrl',
            resolve: {
                data: function () {
                    return {
                        name: $scope.back.title,
                        canDelete:  $scope.info.type === types.empty
                    };
                }
            }
        });
        modalInstance.result.then(function (d) {
            if (d === 'delete') {
                sLibrary.deleteDepartment({ id: $scope.info.libraryId }).then(function (response) {
                    $location.path($scope.back.url).replace();
                });
                return;
            }
            if (!(d.newName && d.newName.length)) {
                return;
            }
            sLibrary.renameNode({ id: $scope.info.libraryId, newName: d.newName }).then(function (response) {
                if (!(response.success || response.Success)) {
                    alert(response.Payload);
                    return;
                }
                $location.path('/library/' + $scope.info.libraryId + '/' + d.newName).replace(); //TODO maybe return new url
            });
        })['finally'](function () {
            modalInstance = undefined;
        });

        $scope.$on('$destroy', function () {
            if (modalInstance) {
                modalInstance.dismiss();
                modalInstance = undefined;
            }
        });
    };


    //$scope.rename = function (newName) {
    //    if (!(newName && newName.length)) {
    //        return;
    //    }
    //    sLibrary.renameNode({ id: $scope.info.libraryId, newName: newName }).then(function (response) {
    //        if (!(response.success || response.Success)) {
    //            alert(response.Payload);
    //            return;
    //        }
    //        $location.path('/library/' + $scope.info.libraryId + '/' + newName).replace(); //TODO maybe return new url
    //    });
    //};


    //#endregion


    //#region analytics    
    $('.u-Website').click(function () {
        cd.analytics.trackEvent('Library', 'Go to org website', 'number of clicks on the union website icon');
    });
    $('.u-Fb').click(function () {
        cd.analytics.trackEvent('Library', 'Go to org Facebok page', 'number of clicks on the union facebook page icon');
    });
    //cd.analytics.setLibrary($('.unionName').text());
    $analytics.setVariable('dimension1', $scope.info.universityName);

    //#endregion

}
    ]
);