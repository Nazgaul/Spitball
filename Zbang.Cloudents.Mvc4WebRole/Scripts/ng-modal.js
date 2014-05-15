(function() {
  var app;

  app = angular.module("ngModal", ['apiService']);

  app.provider("ngModalDefaults", function() {
    return {
      options: {
        closeButtonHtml: "<span class='ng-modal-close-x'>X</span>"
      },
      $get: function() {
        return this.options;
      },
      set: function(keyOrHash, value) {
        var k, v, _results;
        if (typeof keyOrHash === 'object') {
          _results = [];
          for (k in keyOrHash) {
            v = keyOrHash[k];
            _results.push(this.options[k] = v);
          }
          return _results;
        } else {
          return this.options[keyOrHash] = value;
        }
      }
    };
  });

  app.directive('modalDialog', [
    'ngModalDefaults', '$sce', '$compile', 'PartialView', function (ngModalDefaults, $sce, $compile, PartialView) {
      return {
        restrict: 'E',
        scope: {
          show: '=',
          dialogTitle: '@',
          onClose: '&?',
          viewName: '@'
        },
        replace: true,
        transclude: true,
        link: function(scope, element, attrs) {
          var setupCloseButton, setupViewContent, setupStyle;
          setupCloseButton = function() {
            return scope.closeButtonHtml = $sce.trustAsHtml(ngModalDefaults.closeButtonHtml);
          };
          setupViewContent = function () {

              PartialView.fetch({
                  data: { name: scope.viewName },
                  success: function (data) {
                      var el = angular.element(data),
                          compiled = $compile(el);
                      
                      
                      element.find('.ng-modal-dialog-content').append(el);
                      compiled(scope.$parent);
                  },
                  error: function () {
                      console.log('view doesn\'t exist');
                  }
              });             
          }
          setupStyle = function() {
            scope.dialogStyle = {};
            if (attrs.width) {
              scope.dialogStyle['width'] = attrs.width;
            }
            if (attrs.height) {
              return scope.dialogStyle['height'] = attrs.height;
            }
          };
          scope.hideModal = function() {
            return scope.show = false;
          };
          scope.$watch('show', function (newVal, oldVal) {
              if (newVal && !scope.dialogContentHtml) {
                  setupViewContent();
              }
            if (newVal && !oldVal) {
              document.getElementsByTagName("body")[0].style.overflow = "hidden";
            } else {
              document.getElementsByTagName("body")[0].style.overflow = "";
            }
            if ((!newVal && oldVal) && (scope.onClose != null)) {
              return scope.onClose();
            }
          });
          setupCloseButton();
          return setupStyle();
        },
        template: "<div class='ng-modal' ng-show='show'>\n \
                    <div class='ng-modal-overlay'></div>\n \
                    <div class='ng-modal-dialog' ng-style='dialogStyle'>\n \
                    <span class='ng-modal-title' ng-show='dialogTitle && dialogTitle.length' ng-bind='dialogTitle'></span>\n  \
                    <div class='ng-modal-close' ng-click='hideModal()'>\n \
                    <div ng-bind-html='closeButtonHtml'></div>\n \
                    </div>\n \
                    <div class='ng-modal-dialog-content' ng-transclude ng-bind-html='dialogContentHtml'></div>\n  \
                    </div>\n</div>"
      };
    }
  ]);

}).call(this);
