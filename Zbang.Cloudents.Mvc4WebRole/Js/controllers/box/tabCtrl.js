define('tabCtrl',['app'], function (app) {
	app.controller('createTabCtrl',
		['$scope',
		 '$modalInstance',
		 'Box',
		 'data',

		 function ($scope, $modalInstance, Box, data) {		 
		 	$scope.formData = {
		 		boxId: data.boxId,
		 		tabId: data.tabId,
		 		name: data.tabName
		 	};

		 	$scope.params = {
		 		title : data.tabId ? 'Rename folder' : 'Create folder',
		 		action: data.tabId ? 'Rename' : 'Create'
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

		 				alert('error renaming tab');
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
});