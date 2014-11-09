mDashboard.controller('createAcademicBoxCtrl',
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

                            var parent = _.last(nodeHistory);
                            if (parent) {
                                result.parentId = parent.id;
                            }

                            sLibrary.createDepartment(result).then(function (response) {
                                var department = {
                                    id: response.id,
                                    name: response.name
                                };

                                nodeHistory.push(department);
                                $scope.formData.academicBox.departmentId = department.id;
                                $scope.params.selectedDepartment = department;

                                $analytics.eventTrack('Box Wizard', {
                                    category: 'Create Department'
                                });

                            }, function(response) {
                                alert(response);
                            });
                        }
                    }
                });
            };

            $scope.selectDepartment = function (department) {
                nodeHistory.push(department);
                $scope.formData.academicBox.departmentId = department.id;                
                getNodes();
                $analytics.eventTrack('Box Wizard', {
                    category: 'Select Department'
                });
            };

            $scope.changeDepartment = function () {
                $scope.params.selectedDepartment = $scope.formData.academicBox.departmentId = null;
                nodeHistory.pop();
                getNodes();
                $analytics.eventTrack('Box Wizard', {
                    category: 'Change Department'
                });
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
