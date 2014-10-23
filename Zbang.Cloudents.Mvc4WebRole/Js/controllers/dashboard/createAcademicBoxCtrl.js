mDashboard.controller('createAcademicBoxCtrl',
        ['$scope', 'sBox', '$modal', 'sLibrary',
        function ($scope, sBox, $modal, sLibrary) {

            var nodeHistory = [];

            $scope.params = {};

            $scope.formData = {
                academicBox: {}
            };
           
            $scope.create = function (isValid) {
                if (!isValid) {
                    return;
                }
                $scope.formSubmit = true;
                sBox.createAcademic($scope.formData.academicBox).then(function (response) {
                    $scope.formSubmit = false;
                    var data = response.success ? response.payload : [];
                    $scope.box.url = data.url;
                    $scope.box.id = data.id;
                    $scope.next();
                });
            };

            $scope.createDepartment = function () {
                var modalInstance = $modal.open({
                    windowClass: "boxSettings dashMembers",
                    templateUrl: '/Library/CreateDepartmentPartial/',
                    controller: 'CreateDepartmentCtrl',
                    backdrop: 'static',
                });

                modalInstance.result.then(function (result) {
                    var node = _.find($scope.departments, function (item2) {
                        return item2.name === result.name;
                    });

                    if (node) {
                        alert('already exists');
                        return;
                    }

                    result.parentId = _.last(nodeHistory).id;

                    sLibrary.createDepartment(result).then(function (response) {
                        if (!response.success) {
                            alert(response.payload);
                            return;
                        }                        

                        var department = {
                            id: response.payload.id,
                            name: response.payload.name
                        };

                        nodeHistory.push(department);
                        $scope.formData.academicBox.departmentId = department.id;
                        $scope.params.selectedDepartment = department;

                    });
                });
            };

            $scope.selectDepartment = function (department) {
                nodeHistory.push(department);
                $scope.formData.academicBox.departmentId = department.id;                
                getNodes();
            };

            $scope.changeDepartment = function () {
                $scope.params.selectedDepartment = $scope.formData.academicBox.departmentId = null;
                nodeHistory.pop();
                getNodes();
            };

            if ($scope.department && $scope.department.id) {
                $scope.formData.academicBox.departmentId = $scope.department.id;
                $scope.params.selectedDepartment = $scope.department;
                $scope.params.disableChangeDep = true;
                return;
            }

            getNodes();

            function getNodes() {
                var nodesPromise,
                    department = _.last(nodeHistory);

                if (!department) {
                    nodesPromise = sLibrary.items();
                    query();
                    return;
                }

                nodesPromise = sLibrary.items({ section: department.id });
                query();

                function query() {
                    nodesPromise.then(function (response) {
                        var data = response.success ? response.payload : {};

                        if (data.nodes.length) {
                            $scope.departments = data.nodes; //show new  nodes;
                            return;
                        }

                        $scope.formData.academicBox.departmentId = department.id; //moves to create box
                        $scope.params.selectedDepartment = department;

                    });

                }
            }
        }
        ]);
