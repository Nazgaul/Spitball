app.controller('ShareCtrl',
    ['$scope', '$modalInstance', 'sShare', 'sGoogle', 'sFocus', 'data',

    function ($scope, $modalInstance, Share, Google, Focus, data) {

        data = data || {};

        $scope.options = {
            singleMessage: false
        };


        $scope.sources = {
            google: false
        };

        $scope.views = {
            emailSelectedItem: 'emailSelectedItem',
            multiUserEmail: 'multiUserEmail',
            emailMenuItem: 'emailMenuItem'
        }

        $scope.formData = {
            emailList: []
        };

        $scope.friends = [];

        $scope.cancel = function () {
            $modalInstance.dismiss();
        };

        //$scope.validateRecepients = function () {
        //    var validRecepients = 0;

        //    console.log(validRecepients);
        //    return validRecepients;
        //};

        $scope.submit = function (formScope) {
            addFriendByEmail();

            for (var i = 0, l = $scope.formData.emailList.length; i < l; i++) {
                if ($scope.formData.emailList[i].invalid) {
                    formScope.recepients.$setValidity('invalid', false);
                    return;
                }
            }

            formScope.recepients.$setValidity('invalid', true);


            if (formScope.$invalid) {
                return;
            }

            $modalInstance.close();

            if ($scope.singleUser) {
                $scope.formData.recepients = [$scope.singleUser.id];                
                return;
            }

            var ids = $scope.formData.emailList.map(function (item) {
                return item.id;                
            });

            $scope.formData.recepients = ids;

            Share.message($scope.formData).then(function () {
            });



        }

        if (data.singleMessage) {
            $scope.singleUser = data.user;
            $scope.options.singleMessage = true;
            return;
        }
        if (data.groupMessage) {
            var item = data.users;//TO BE CHECKED
            $scope.emailList.push(item);
        }

        Share.cloudentsFriends().then(function (contacts) {
            var mapped = contacts.payload.my.map(function (contact) {
                return {
                    id: contact.uid,
                    name: contact.name,
                    image: contact.image,
                    cloudents: true
                }
            });
            $scope.friends = $scope.friends.concat(mapped);

        });

        getGoogleContacts(true);

        $scope.onSelectedItem = function ($item) {
            $scope.formData.searchInput = null;

            if ($scope.formData.emailList.indexOf($item) > -1) {
                alert('Contact already exists');//translate
                return;
            }
            $scope.formData.emailList.push($item);

            $scope.$broadcast('itemChange');
        };


        $scope.getView = function (item) {
            if (Array.isArray(item)) {
                return $scope.views.multiUserEmail;
            }

            return $scope.views.emailSelectedItem;
        };

        $scope.loadGoogleContacts = function () {
            getGoogleContacts(false);
        };


        $scope.filterBy = function (query) {
            return function (friend) {
                var tQuery, tName, tId;
                tQuery = query.toLowerCase();
                tName = friend.name.toLowerCase();
                if (friend.google) {
                    tId = friend.id.toLowerCase();
                    return tId.indexOf(tQuery) > -1 || tName.indexOf(tQuery) > -1;
                }

                return tName.indexOf(tQuery) > -1;
            }
        }

        $scope.removeItem = function (item) {
            var index = $scope.formData.emailList.indexOf(item);
            $scope.formData.emailList.splice(index, 1);
            $scope.$broadcast('itemChange');
            Focus('shareInput');
        };

        $scope.editItem = function (item) {
            if (!item.invalid) {
                return;
            }

            $scope.formData.searchInput = item.name;
            $scope.removeItem(item);

            $scope.$broadcast('itemChange');
            Focus('shareInput');
        };

        $scope.keydownListener = function (e) {
            if (e.keyCode === 9 || e.keyCode === 13 || e.keyCode === 186 || e.keyCode === 188) { // , ; TAB  
                e.preventDefault();
                return;
            }
            if (e.keyCode === 8 && !$scope.formData.searchInput) { //if backspace and value is empty delete last email
                $scope.formData.emailList.pop();
                $scope.$broadcast('itemChange');
                e.preventDefault();
                return;
            }
            if (e.keyCode === 9 && !$scope.formData.searchInput) {
                e.preventDefault();
                return;
            }
        };


        $scope.keyupListener = function (e) {
            if (e.keyCode === 188 || e.keyCode === 186 || e.keyCode === 9) { // , ; TAB 
                addFriendByEmail();
            }


        };
        
        function addFriendByEmail() {
            if (!$scope.formData.searchInput) {
                return;
            }

            var emailRegExp = new RegExp(/^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/),

            item = {
                id: $scope.formData.searchInput,
                name: $scope.formData.searchInput
            };

            for (var i = 0, l = $scope.formData.emailList.length; i < l; i++) {
                if ($scope.formData.emailList[i].name === item.name) {
                    alert('Contact already exists'); //translate
                    return;
                }
            }

            if (!emailRegExp.test($scope.formData.searchInput)) {
                item.invalid = true;
            }

            $scope.onSelectedItem(item);
        }

        function getGoogleContacts(isImmediate) {
            Google.contacts(isImmediate).then(function (contacts) {
                $scope.friends = $scope.friends.concat(contacts);
                $scope.sources.google = true;
            });
        }
    }
    ]);
app.directive('resizeInput',
    ['$timeout',
        function ($timeout) {

            return {
                restrict: 'A',
                scope: {
                    googleBtn: '='
                },
                link: function (scope, elem, attrs) {
                    var maxWidth = 430,
                        minWidth = 100,
                        container = $('.emailUser')[0],
                        $emailListWpr = $('.emailListWpr');


                    scope.$watch('googleBtn', function (newValue, oldValue) {
                        maxWidth = newValue ? 430 : 510;
                        $emailListWpr.css('max-width', maxWidth)
                        elem.css('max-width', maxWidth);
                    });

                    scope.$on('itemChange', function () {
                        $timeout(setWidth);
                    });

                    elem.on('keyup', function () {
                        if (elem[0].scrollWidth > elem[0].clientWidth) {
                            elem.css('max-width', maxWidth);
                        }
                        if (container.scrollHeight > container.clientHeight) {
                            container.scrollTop = container.scrollHeight;
                        }
                    });

                    function setWidth() {
                        var rowWidth = 0, itemWidth;
                        $('.emailItem').each(function (item) {
                            itemWidth = $(this).outerWidth(true);
                            if (rowWidth + itemWidth < maxWidth) {
                                rowWidth += itemWidth;
                            } else if (rowWidth + itemWidth >= maxWidth) {
                                rowWidth = itemWidth;
                            }
                        });


                        if (container.scrollHeight > container.clientHeight) {
                            container.scrollTop = container.scrollHeight;
                        }


                        if (maxWidth - rowWidth < minWidth) {
                            elem.css('max-width', maxWidth);
                            return;
                        }

                        elem.css('max-width', maxWidth - rowWidth - 3); //some weird value


                    }
                }
            };
        }
    ]);
