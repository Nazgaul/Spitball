module app {
    "use strict";
    export interface IChatTimeAgo extends ng.IScope {
        fromTime: any;
        format: any;
    }
    class ChatTimeAgo implements angular.IDirective {
        scope: { [boundProperty: string]: string } = {
            fromTime: "@",
            format: "@"
        };
        restrict = "EA";

        constructor(private timeAgo: any, private nowTime: any) {
            
        }
        link = (scope: IChatTimeAgo, element: ng.IAugmentedJQuery) => {
            /*handle all your linking requirements here*/
            const threeDaysInMilliseconds = 2.592e+8;
            var fromTime;

            // Track changes to fromTime
            scope.$watch('fromTime', () => {
                fromTime = this.timeAgo.parse(scope.fromTime);
            });

            // Track changes to time difference
            var unregister = scope.$watch(() => {
                return this.nowTime() - fromTime;
            }, (value) => {

                if (value > threeDaysInMilliseconds) {
                    element.text('');
                    unregister();
                    return;
                }
                element.text(this.timeAgo.inWords(value, fromTime, scope.format));
            });
        };
        static factory(): angular.IDirectiveFactory {
            const directive = (timeAgo, nowTime) => {
                return new ChatTimeAgo(timeAgo, nowTime);
            };
            directive["$inject"] = ["timeAgo", "nowTime"];
            return directive;
        }
    }

    angular
        .module("app.chat")
        .directive("chatTimeAgo", ChatTimeAgo.factory());
};
