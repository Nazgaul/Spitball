var mLibrary = angular.module('mLibrary', []);
mLibrary.controller('LibraryCtrl',
    ['$scope', '$location', '$routeParams', '$timeout', '$modal', 'sUserDetails', 'sLibrary', 'sBox', '$rootScope',
function ($scope, $location, $routeParams, $timeout, $modal, sUserDetails, sLibrary, sBox, $rootScope) {

    var jsResources = window.JsResources;

    var types = {
        box: 'box',
        department: 'department'
    }

    //#region data
    $scope.info = {
        libraryId: $routeParams.libraryId,
        libraryName: $routeParams.libraryName,
        items: []

    };

    $scope.back.title = $scope.info.libraryName;


    //var partials = {
    //    //createAcademicBox: '/Library/CreateAcademicBoxPartial/',
    //    createDepartment: '/Library/CreateDepartmentPartial/'
    //}

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
        }

        if (pageData) {
            $scope.info.items.push.apply($scope.info.items, pageData);
        }

        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });

        //if (pageData.length === $scope.info.pageSize) {
        //    //$scope.info.paggingnNeeded = true;
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
            keyboard: false
            // resolve: {
            //friends: function () {
            //return data.payload.my;
            //}
            // }
        });
        modalInstance.result.then(function (url) {

            $rootScope.params.createBoxWizard = false;
            if (url) {
                $location.path(url);
            }
        });
    };

    //$scope.createDepartment = function () {
    //    var modalInstance = $modal.open({
    //        //windowClass: "boxSettings dashMembers",
    //        templateUrl: partials.createDepartment,
    //        controller: 'CreateDepartmentCtrl',
    //        backdrop: 'static',
    //    });

    //    modalInstance.result.then(function (result) {
    //        result.parentId = $scope.info.libraryId;

    //        var item = _.find($scope.info.items, function (item) {
    //            return item.name === result.name;
    //        });

    //        if (item) {
    //            alert('already exists');                      
    //            return;
    //        }

    //        sLibrary.department.create(result).then(function (response) {
    //            $scope.info.items.push(response.payload);
    //        });
    //    }, function () {
    //        //dismiss
    //    });


    //};

    //$scope.deleteDepartment = function () {
    //    sLibrary.department.delete({ id: $scope.info.libraryId }).then(function (response) {
    //        //TODO: nav to parent
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

    //$scope.isAdmin = function () {
    //    if (parseInt($scope.info.universityId, 10) === sUserDetails.getDetails().id) {
    //        return true;
    //    }

    //    return false;
    //};

    //$scope.createDepartmentVisible = function () {
    //    if (!$scope.isAdmin()) {
    //        return false;
    //    }

    //    if (!$scope.info.items.length) {
    //        return true;
    //    }

    //    if ($scope.info.type === types.department) {
    //        return true;
    //    }

    //    return false;
    //};

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

    //$scope.renameBox = function (newName) {
    //    if (!(newName && newName.length)) {
    //        return;
    //    }

    //    $location.path('/library/' + $scope.info.libraryId + '/' + newName).replace(); //TODO maybe return new url

    //    sLibrary.renameNode({ id: $scope.info.libraryId, newName: newName }).then(function (response) {
    //        if (!(response.success || response.Success)) {
    //            alert(response.Payload);
    //            return;
    //        }
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
    cd.analytics.setLibrary($('.unionName').text());

    //#endregion

}
    ]
);