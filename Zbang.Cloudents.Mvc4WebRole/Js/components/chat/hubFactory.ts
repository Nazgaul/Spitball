module app {
    "use scrict";
    declare var dChat: any;

    // export type hubEvent = "hub-chat" | "hub-status" | "preview-ready" | "update-thumbnail" | "connection-state"
    export class Guid {
        static newGuid(): string {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        static toBase64(guid: string): string {
            var binary = '';
            var bytes = new Uint8Array(guidToBytes(guid));
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);

            function guidToBytes(guid) {
                var bytes = [];
                guid.split('-').map((number, index) => {
                    var bytesInChar = index < 3 ? number.match(/.{1,2}/g).reverse() : number.match(/.{1,2}/g);
                    bytesInChar.map((byte) => { bytes.push(parseInt(byte, 16)); });
                });
                return bytes;
            }
        }
        static fromBase64(base64: string): string {
            var raw = window.atob(base64);
            var rawLength = raw.length;
            var array = new Uint8Array(new ArrayBuffer(rawLength));

            for (let i = 0; i < rawLength; i++) {
                array[i] = raw.charCodeAt(i);
            }

            var arr = new Array(36);
            a2hs(array, 0, 3, false, arr, 0, true); arr[8] = "-";
            a2hs(array, 4, 5, false, arr, 9, true); arr[13] = "-";
            a2hs(array, 6, 7, false, arr, 14, true); arr[18] = "-";
            a2hs(array, 8, 9, false, arr, 19, false); arr[23] = "-";
            a2hs(array, 10, 15, false, arr, 24, false);
            var str = arr.join("");
            return str;

            function a2hs(bytes, begin, end, uppercase, str, pos, needReplace) {
                var mkNum = (num, uppercase) => {
                    var base16 = num.toString(16);
                    if (base16.length < 2)
                        base16 = "0" + base16;
                    if (uppercase)
                        base16 = base16.toUpperCase();
                    return base16;
                };
                if (needReplace) {
                    for (let i = end; i >= begin; i--)
                        str[pos++] = mkNum(bytes[i], uppercase);
                } else {
                    for (let i = begin; i <= end; i++)
                        str[pos++] = mkNum(bytes[i], uppercase);
                }
                return str;
            };


        }
    }

    var connectionStatus: boolean = false;
    export interface IRealtimeFactory {
        sendMsg(userId: number, message: string, conversationId: Guid, blob?: string): void;
        changeUniversity(): void;
        assingBoxes(boxIds): void;
        isConnected(): boolean;
    }

    //interface ISbHub extends ngSignalr.Hub {
    //    sendMsg(userId: number, message: string, conversationId: Guid, blob: string): void;
    //}

    class RealTimeFactory implements IRealtimeFactory {
        static $inject = ["Hub", "$rootScope", "ajaxService2"];
        private commands: Array<Function> = [];
        private canSend: boolean;
        private hub: ngSignalr.Hub;
        constructor(private Hub: ngSignalr.HubFactory, private $rootScope: angular.IRootScopeService, private ajaxService: IAjaxService2) {
            this.hub = new Hub('spitballHub', {
                rootPath: (dChat || 'https://connect.spitball.co') + '/s',
                listeners: {
                    chat(message, chatRoom, userId, blob) {
                        $rootScope.$broadcast("hub-chat",
                            {
                                message: message,
                                chatRoom: chatRoom,
                                userId: userId,
                                blob: blob
                            });
                    },
                    online(userId) {
                        $rootScope.$broadcast('hub-status', {
                            userId: userId,
                            online: true
                        });
                    },
                    offline(userId) {
                        $rootScope.$broadcast('hub-status', {
                            userId: userId,
                            online: false
                        });
                    },
                    updateImage(blob) {
                        $rootScope.$broadcast('preview-ready', blob);
                    },
                    updateThumbnail(itemId) {
                        $rootScope.$broadcast('update-thumbnail', itemId);
                    },
                    echo(i) {
                        //console.log('echo', i);
                    }
                },
                errorHandler(error) {
                    ajaxService.logError('signalr', 'errorHandler', error);
                    //console.error(error);
                },

                methods: ['send', 'changeUniversity', 'enterBoxes'],
                stateChanged: (state) => {
                    switch (state.newState) {
                        case $.signalR.connectionState.connecting:
                            this.canSend = false;
                            this.changeStatus(false);
                            break;
                        case $.signalR.connectionState.connected:
                            this.canSend = true;
                            for (let i = 0; i < this.commands.length; i++) {
                                this.commands[i]();
                            }
                            this.commands = [];
                            this.changeStatus(true);
                            break;
                        case $.signalR.connectionState.reconnecting:
                            this.canSend = false;
                            ajaxService.logError('signalr', 'reconnecting');
                            this.changeStatus(false);
                            break;
                        case $.signalR.connectionState.disconnected:
                            this.canSend = false;
                            this.changeStatus(false);
                            connectionStatus = false;
                            break;
                    }
                }
            });
            $rootScope.$on("$stateChangeSuccess", (event: angular.IAngularEvent, toState: angular.ui.IState,
                toParams: spitaball.ISpitballStateParamsService) => {
                if (toParams.boxId) {
                    if (this.boxIds.indexOf(toParams.boxId) === -1) {
                        this.assingBoxes(toParams.boxId);
                    }

                }
            });


            this.hub.connection.disconnected(() => {
                setTimeout(() => {
                    this.hub.connection.start();
                }, 5000); // Restart connection after 5 seconds.
            });
        }

        private changeStatus = (isConnected: boolean): void => {
            connectionStatus = isConnected;
            this.$rootScope.$applyAsync();
        };
        isConnected = (): boolean => {
            return connectionStatus;
        };
        //isConnected() {
        //    return connectionStatus;
        //};
        sendMsg(userId: number, message: string, conversationId: Guid, blob: string) {
            this.hub.invoke('send', userId, message, conversationId, blob);
        }
        changeUniversity() {
            this.hub.invoke('changeUniversity');
        };
        boxIds: Array<number> = [];

        assingBoxes = (boxIds) => {
            if (!boxIds) {
                return;
            }

            if (!angular.isArray(boxIds)) {
                boxIds = [boxIds];
                this.boxIds = this.boxIds.concat(boxIds);
            }
            if (this.canSend) {
                // hub.invoke('enterBox', [boxId]);
                this.hub.invoke('enterBoxes', boxIds);
            } else {
                this.commands.push(() => {
                    this.assingBoxes.apply(this, [boxIds]);
                });
            }
        }
    }
    angular.module('app.chat').service('realtimeFactory', RealTimeFactory);
}


