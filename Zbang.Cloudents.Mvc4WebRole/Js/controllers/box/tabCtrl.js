mBox.controller('createTabCtrl',
		['$scope',
		 '$modalInstance',
         '$analytics',
		 'sBox',
         'resManager',
		 'data',
         'sNotify',
            function ($scope, $modalInstance, $analytics, sBox, resManager, data, sNotify) {
                "use strict";
                $scope.formData = {
                    boxId: data.boxId,
                    tabId: data.tabId,
                    name: data.tabName
                };

                $scope.params = {
                    title: data.tabId ? resManager.get('FolderRename') : resManager.get('FolderCreate'),
                    action: data.tabId ? resManager.get('Rename') : resManager.get('Create')
                };

                $scope.create = function (isValid) {
                    if (!isValid) {
                        return;
                    }

                    if ($scope.formData.tabId) {
                        sBox.renameTab($scope.formData).then(function () {
                            $modalInstance.close($scope.formData.name);
                            $analytics.eventTrack('Tabs - Renamed Tab', {
                                category: 'Box'
                            });
                        }, function () {
                            sNotify.tAlert('RenameError');
                        });

                        return;
                    }



                    sBox.createTab($scope.formData).then(function (tab) {
                        $modalInstance.close(tab);
                        $analytics.eventTrack('Tabs - Created Tab', {
                            category: 'Box'
                        });
                    }, function (response) {
                        sNotify.alert(response);
                    });
                };

                $scope.cancel = function () {
                    $modalInstance.dismiss();
                    //TODO analytics 
                };
            }
		]);
