mBox.controller('createTabCtrl',
		['$scope',
		 '$modalInstance',
		 'sBox',
		 'data',

		 function ($scope, $modalInstance, sBox, data) {
		     "use strict";
		     var jsResources = window.JsResources;
		     $scope.formData = {
		         boxId: data.boxId,
		         tabId: data.tabId,
		         name: data.tabName
		     };

		     $scope.params = {
		         title: data.tabId ? jsResources.FolderRename : jsResources.FolderCreate,
		         action: data.tabId ? jsResources.Rename : jsResources.Create
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
		                 alert(jsResources.RenameError);
		             });

		             return;
		         }



		         sBox.createTab($scope.formData).then(function (tab) {
		             $modalInstance.close(tab);
		             $analytics.eventTrack('Box Tabs', {
		                 category: 'Created Tab'
		             });
		         }, function (response) {
		             alert(response)
		         });
		     };

		     $scope.cancel = function () {
		         $modalInstance.dismiss();
		         //TODO analytics 
		     };
		 }
		]);
