var app;
(function (app) {
    "use scrict";
    var Guid = (function () {
        function Guid() {
        }
        Guid.newGuid = function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        };
        Guid.toBase64 = function (guid) {
            var binary = '';
            var bytes = new Uint8Array(guidToBytes(guid));
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
            function guidToBytes(guid) {
                var bytes = [];
                guid.split('-').map(function (number, index) {
                    var bytesInChar = index < 3 ? number.match(/.{1,2}/g).reverse() : number.match(/.{1,2}/g);
                    bytesInChar.map(function (byte) { bytes.push(parseInt(byte, 16)); });
                });
                return bytes;
            }
        };
        Guid.fromBase64 = function (base64) {
            var raw = window.atob(base64);
            var rawLength = raw.length;
            var array = new Uint8Array(new ArrayBuffer(rawLength));
            for (var i = 0; i < rawLength; i++) {
                array[i] = raw.charCodeAt(i);
            }
            var arr = new Array(36);
            a2hs(array, 0, 3, false, arr, 0, true);
            arr[8] = "-";
            a2hs(array, 4, 5, false, arr, 9, true);
            arr[13] = "-";
            a2hs(array, 6, 7, false, arr, 14, true);
            arr[18] = "-";
            a2hs(array, 8, 9, false, arr, 19, false);
            arr[23] = "-";
            a2hs(array, 10, 15, false, arr, 24, false);
            var str = arr.join("");
            return str;
            function a2hs(bytes, begin, end, uppercase, str, pos, needReplace) {
                var mkNum = function (num, uppercase) {
                    var base16 = num.toString(16);
                    if (base16.length < 2)
                        base16 = "0" + base16;
                    if (uppercase)
                        base16 = base16.toUpperCase();
                    return base16;
                };
                if (needReplace) {
                    for (var i = end; i >= begin; i--)
                        str[pos++] = mkNum(bytes[i], uppercase);
                }
                else {
                    for (var i = begin; i <= end; i++)
                        str[pos++] = mkNum(bytes[i], uppercase);
                }
                return str;
            }
            ;
        };
        return Guid;
    }());
    app.Guid = Guid;
    var connectionStatus = false;
    var RealTimeFactory = (function () {
        function RealTimeFactory(Hub, $rootScope, ajaxService) {
            var _this = this;
            this.Hub = Hub;
            this.$rootScope = $rootScope;
            this.ajaxService = ajaxService;
            this.commands = [];
            this.changeStatus = function (isConnected) {
                connectionStatus = isConnected;
                _this.$rootScope.$applyAsync();
            };
            this.isConnected = function () {
                return connectionStatus;
            };
            this.boxIds = [];
            this.assingBoxes = function (boxIds) {
                if (!boxIds) {
                    return;
                }
                if (!angular.isArray(boxIds)) {
                    boxIds = [boxIds];
                    _this.boxIds = _this.boxIds.concat(boxIds);
                }
                if (_this.canSend) {
                    _this.hub.invoke('enterBoxes', boxIds);
                }
                else {
                    _this.commands.push(function () {
                        _this.assingBoxes.apply(_this, [boxIds]);
                    });
                }
            };
            this.hub = new Hub('spitballHub', {
                rootPath: (dChat || 'https://connect.spitball.co') + '/s',
                listeners: {
                    chat: function (message, chatRoom, userId, blob) {
                        $rootScope.$broadcast("hub-chat", {
                            message: message,
                            chatRoom: chatRoom,
                            userId: userId,
                            blob: blob
                        });
                    },
                    online: function (userId) {
                        $rootScope.$broadcast('hub-status', {
                            userId: userId,
                            online: true
                        });
                    },
                    offline: function (userId) {
                        $rootScope.$broadcast('hub-status', {
                            userId: userId,
                            online: false
                        });
                    },
                    updateImage: function (blob) {
                        $rootScope.$broadcast('preview-ready', blob);
                    },
                    updateThumbnail: function (itemId) {
                        $rootScope.$broadcast('update-thumbnail', itemId);
                    },
                    echo: function (i) {
                    }
                },
                errorHandler: function (error) {
                    ajaxService.logError('signalr', 'errorHandler', error);
                },
                methods: ['send', 'changeUniversity', 'enterBoxes'],
                stateChanged: function (state) {
                    switch (state.newState) {
                        case $.signalR.connectionState.connecting:
                            _this.canSend = false;
                            _this.changeStatus(false);
                            break;
                        case $.signalR.connectionState.connected:
                            _this.canSend = true;
                            for (var i = 0; i < _this.commands.length; i++) {
                                _this.commands[i]();
                            }
                            _this.commands = [];
                            _this.changeStatus(true);
                            break;
                        case $.signalR.connectionState.reconnecting:
                            _this.canSend = false;
                            ajaxService.logError('signalr', 'reconnecting');
                            _this.changeStatus(false);
                            break;
                        case $.signalR.connectionState.disconnected:
                            _this.canSend = false;
                            _this.changeStatus(false);
                            connectionStatus = false;
                            break;
                    }
                }
            });
            $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams) {
                if (toParams.boxId) {
                    if (_this.boxIds.indexOf(toParams.boxId) === -1) {
                        _this.assingBoxes(toParams.boxId);
                    }
                }
            });
            this.hub.connection.disconnected(function () {
                setTimeout(function () {
                    _this.hub.connection.start();
                }, 5000);
            });
        }
        RealTimeFactory.prototype.sendMsg = function (userId, message, conversationId, blob) {
            this.hub.invoke('send', userId, message, conversationId, blob);
        };
        RealTimeFactory.prototype.changeUniversity = function () {
            var _this = this;
            if (this.canSend) {
                this.hub.invoke('changeUniversity');
            }
            else {
                this.commands.push(function () {
                    _this.changeUniversity.apply(_this);
                });
            }
        };
        ;
        RealTimeFactory.$inject = ["Hub", "$rootScope", "ajaxService2"];
        return RealTimeFactory;
    }());
    angular.module('app.chat').service('realtimeFactory', RealTimeFactory);
})(app || (app = {}));
