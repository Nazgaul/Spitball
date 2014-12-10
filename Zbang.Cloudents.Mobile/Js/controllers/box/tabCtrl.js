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
                            $analytics.eventTrack('Box Tabs', {
                                category: 'Renamed Tab'
                            });
                        }, function () {
                            sNotify.tAlert('RenameError');
                        });

                        return;
                    }



                    sBox.createTab($scope.formData).then(function (tab) {
                        $modalInstance.close(tab);
                        $analytics.eventTrack('Box Tabs', {
                            category: 'Created Tab'
                        });
                    }, function (response) {
                        sNotify.alert(response)
                    });
                };

                $scope.cancel = function () {
                    $modalInstance.dismiss();
                    //TODO analytics 
                };
            }
		]);
