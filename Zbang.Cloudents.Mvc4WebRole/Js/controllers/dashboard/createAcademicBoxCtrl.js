﻿mDashboard.controller('createAcademicBoxCtrl',
        ['$scope', 'sBox', 'sModal', 'sUserDetails','sLibrary',
        function ($scope, sBox, sModal, sUserDetails, sLibrary) {
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

                sBox.createAcademic($scope.formData.academicBox).then(function (data) {                    
                        $scope.box.url = data.url;
                        $scope.box.id = data.id;                        
                        $scope.next();
                                                            
                }, function (response) {
                    $scope.params.customError = response[0].value[0];

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

                                //TODO analytics
                            });
                        }
                    }
                });
            };

            $scope.selectDepartment = function (department) {
                nodeHistory.push(department);
                $scope.formData.academicBox.departmentId = department.id;                
                getNodes();
                //TODO analytics

            };

            $scope.changeDepartment = function () {
                $scope.params.selectedDepartment = $scope.formData.academicBox.departmentId = null;
                nodeHistory.pop();
                getNodes();
                //TODO analytics

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
                    nodesPromise.then(function (data) {
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
