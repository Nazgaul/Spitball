mBox.controller('createTabCtrl',
		['$scope',
		 '$modalInstance',
		 'sBox',
		 'data',

		 function ($scope, $modalInstance, Box, data) {
		     console.log($scope);
		 	$scope.formData = {
		 		boxId: data.boxId,
		 		tabId: data.tabId,
		 		name: data.tabName
		 	};

		 	$scope.params = {
		 	    title : data.tabId ? JsResources.FolderRename : JsResources.FolderCreate,
		 	    action: data.tabId ? JsResources.Rename : JsResources.Create
		 	};

		 	$scope.create = function (isValid) {
		 		if (!isValid) {
		 			return;
		 		}

		 		if ($scope.formData.tabId) {
		 			Box.renameTab($scope.formData).then(function (response) {
		 				if (response.success) {
		 					$modalInstance.close($scope.formData.name);
		 					return;
		 				}

		 				alert(JsResources.RenameError);
		 			});

		 			return;
		 		}

		 		Box.createTab($scope.formData).then(function (tab) {
		 			if (!tab.success) {
		 				alert(tab.payload);
		 				return;
		 			}

		 			$modalInstance.close(tab.payload || tab.Payload);
		 		});
		 	};

		 	$scope.cancel = function () {
		 		$modalInstance.dismiss();
		 	};
		 }
	]);
