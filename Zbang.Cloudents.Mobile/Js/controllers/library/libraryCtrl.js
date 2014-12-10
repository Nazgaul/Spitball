var mLibrary = angular.module('mLibrary', []).
    controller('LibraryCtrl',
    ['$scope', '$location', 'resManager', '$routeParams', '$timeout', 'sModal', 'sUserDetails', 'sFacebook', 'sLibrary', 'sBox', '$rootScope', '$analytics', 'sNotify',
function ($scope, $location, resManager, $routeParams, $timeout, sModal, sUserDetails, sFacebook, sLibrary, sBox, $rootScope, $analytics, sNotify) {
    "use strict";
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

    addItems();

    function addItems() {
        sLibrary.items({ section: $scope.info.libraryId }).then(function (response) {
            processData(response);
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

        sModal.open('createBoxWizard', {
            data: {
                isAcademic: true,
                department: {
                    id: $scope.info.libraryId,
                    name: $scope.info.libraryName
                }
            },
            callback: {
                close: function (response) {
                    $rootScope.params.createBoxWizard = false;

                    $analytics.eventTrack('Library', {
                        category: 'Finish Wizard'
                    });

                    if (response) {
                        $location.path(response.url);
                        if (response.isItems) {
                            $location.hash('items');
                        }
                    }

                },
                always: function () {
                    $rootScope.params.createBoxWizard = false; //user cancelled
                }
            }
        });
    };

    $scope.createDepartment = function () {
        sModal.open('createDep', {
            callback: {
                close: function (result) {
                    result.parentId = $scope.info.libraryId;

                    var item = _.find($scope.info.items, function (item2) {
                        return item2.name === result.name;
                    });

                    if (item) {
                        sNotify.alert('already exists');
                        return;
                    }

                    sLibrary.createDepartment(result).then(function (response) {
                        $scope.info.items.push(response);
                        $scope.info.type = types.department;
                    }, function (response) {
                        sNotify.alert(response);
                    });

                    $analytics.eventTrack('Library', {
                        category: 'Create Department'
                    });

                }
            }
        });
    };

    //$scope.deleteDepartment = function () {
    //    sLibrary.department.delete({ id: $scope.info.libraryId }).then(function (response) {
    //        TODO: nav to parent
    //    });
    //};

    $scope.subscribe = function (box) {
        sFacebook.postFeed(resManager.getParsed('IJoined', [box.name]), box.url);

        box.userType = 'subscribe';
        sBox.follow({ boxId: box.id });

        $analytics.eventTrack('Library', {
            category: 'Follow box'
        });
    };

    $scope.unsubscribe = function (box) {
        var defer,
            isDelete = box.userType === 'owner' || (box.membersCount <= 2 && box.commentCount < 2 && box.itemCount === 0);

        if (isDelete) {
            defer = sNotify.tConfirm('DeleteCourse');
        }
        else {
            defer = sNotify.confirm(resManager.get('SureYouWantTo') + ' ' + resManager.get('ToLeaveGroup'));
        }

        defer.then(function () {
            sBox.remove({ id: box.id });
            $analytics.eventTrack('Library', {
                category: 'Leave Box'
            });

            box.userType = 'none';

            if (isDelete) {
                var index = $scope.info.items.indexOf(box);
                if (index > -1) {
                    $scope.info.items.splice(index, 1);
                }                
            }
        });


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


        sModal.open('depSettings', {
            data: {
                name: $scope.back.title,
                canDelete: $scope.info.type === types.empty || $scope.info.items.length === 0
            },
            callback: {
                close: function (d) {
                    if (d === 'delete') {
                        $analytics.eventTrack('Library', {
                            category: 'Delete Department'
                        });
                        sLibrary.deleteDepartment({ id: $scope.info.libraryId }).then(function (response) {
                            $location.path($scope.back.url).replace();
                        });
                        return;
                    }
                    if (!(d.newName && d.newName.length) || d.newName === $scope.back.title) {
                        return;
                    }

                    $analytics.eventTrack('Library', {
                        category: 'Rename Department'
                    });

                    sLibrary.renameNode({ id: $scope.info.libraryId, newName: d.newName }).then(function (response) {
                        $location.path('/library/' + $scope.info.libraryId + '/' + d.newName).replace(); //TODO maybe return new url
                    }, function (response) {
                        sNotify.alert(response);
                    });
                }
            }
        });
    };

    //#region analytics        
    $analytics.setVariable('dimension1', $scope.info.universityName);

    //#endregion

}
    ]
);