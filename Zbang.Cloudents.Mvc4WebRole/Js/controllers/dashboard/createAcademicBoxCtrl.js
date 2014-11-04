
mDashboard.controller('createAcademicBoxCtrl',
        ['$scope', 'sBox', 'sModal', 'sLibrary',
        function ($scope, sBox, sModal, sLibrary) {
            "use strict";
            var nodeHistory = [];

            $scope.params = {
                isAdmin: sUserDetails.getDetails().isAdmin
            };

            $scope.formData = {
                academicBox: {}               
            };
           
            var createDisabled = false;
            $scope.create = function (isValid) {
                $scope.params.customError = null;

                if (!isValid) {
                    return;
                }
                createDisabled = true;

                sBox.createAcademic($scope.formData.academicBox).then(function (response) {
                    if (response.success) {
                        var data = response.success ? response.payload : [];
                        $scope.box.url = data.url;
                        $scope.box.id = data.id;                        
                        $scope.next();
                        return;
                    }
                    //TODO: add error msg
                    $scope.params.customError = response.payload[0].value[0];
                }).finally(function () {
                    createDisabled = false;

                });
            };

            $scope.createDepartment = function () {
                sModal.open('createDep', {
                    callback: {
                        close: function (result) {
                            var node = _.find($scope.departments, function (item2) {
                                return item2.name === result.name;
                            });

                            if (node) {
                                alert('already exists');
                                return;
                            }

                            parent = _.last(nodeHistory);
                            if (parent) {
                                result.parentId = parent.id;
                            }

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
                        }
                    }
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

            $scope.clearErrors = function () {
                $scope.params.customError = null;
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
