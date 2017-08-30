﻿"use strict";
angular.module('wizard', []).
    directive('wzStep', ['$templateCache', function ($templateCache) {
        return {
            restrict: 'EA',
            replace: true,
            transclude: true,
            scope: {
                wzTitle: '@',
                title: '@'
            },
            require: '^wizard',
            templateUrl: function (element, attributes) {
                return $templateCache.get(attributes.template) || "step.html";
            },
            link: function ($scope, $element, $attrs, wizard) {
                $scope.title = $scope.title || $scope.wzTitle;
                wizard.addStep($scope);
            }
        };
    }]).
    factory('WizardHandler', function () {
        var service = {};

        var wizards = {};

        service.defaultName = "defaultWizard";

        service.addWizard = function (name, wizard) {
            wizards[name] = wizard;
        };

        service.removeWizard = function (name) {
            delete wizards[name];
        };

        service.wizard = function (name) {
            var nameToUse = name;
            if (!name) {
                nameToUse = service.defaultName;
            }

            return wizards[nameToUse];
        };

        return service;
    }).
    directive('wizard', ['$templateCache', function ($templateCache) {
        return {
            restrict: 'EA',
            replace: true,
            transclude: true,
            scope: {
                currentStep: '=',
                onFinish: '&',
                hideIndicators: '=',
                editMode: '=',
                name: '@'
            },
            templateUrl: function (element, attributes) {

                return $templateCache.get(attributes.template) || "wizard.html";
            },
            controller: ['$scope', '$element', 'WizardHandler', function ($scope, $element, WizardHandler) {

                WizardHandler.addWizard($scope.name || WizardHandler.defaultName, this);
                $scope.$on('$destroy', function () {
                    WizardHandler.removeWizard($scope.name || WizardHandler.defaultName);
                });

                $scope.steps = [];

                $scope.$watch('currentStep', function (step) {
                    if (!step) return;
                    var stepTitle = $scope.selectedStep.title || $scope.selectedStep.wzTitle;
                    if ($scope.selectedStep && stepTitle !== $scope.currentStep) {
                        $scope.goTo(_.findWhere($scope.steps, { title: $scope.currentStep }));
                    }

                });

                $scope.$watch('[editMode, steps.length]', function () {
                    var editMode = $scope.editMode;
                    if (_.isUndefined(editMode) || _.isNull(editMode)) return;

                    if (editMode) {
                        _.each($scope.steps, function (step) {
                            step.completed = true;
                        });
                    }
                }, true);

                this.addStep = function (step) {
                    $scope.steps.push(step);
                    if ($scope.steps.length === 1) {
                        $scope.goTo($scope.steps[0]);
                    }
                };

                $scope.goTo = function (step) {
                    unselectAll();
                    $scope.selectedStep = step;
                    if (!_.isUndefined($scope.currentStep)) {
                        $scope.currentStep = step.title || step.wzTitle;
                    }
                    step.selected = true;
                    $scope.$emit('wizard:stepChanged', { step: step, index: _.indexOf($scope.steps, step) });
                };

                $scope.currentStepNumber = function () {
                    return _.indexOf($scope.steps, $scope.selectedStep) + 1;
                }

                function unselectAll() {
                    _.each($scope.steps, function (step) {
                        step.selected = false;
                    });
                    $scope.selectedStep = null;
                }

                this.next = function (draft) {
                    var index = _.indexOf($scope.steps, $scope.selectedStep);
                    if (!draft) {
                        $scope.selectedStep.completed = true;
                    }
                    if (index === $scope.steps.length - 1) {
                        this.finish();
                    } else {
                        $scope.goTo($scope.steps[index + 1]);
                    }
                };

                this.goTo = function (step) {
                    var stepTo;
                    if (_.isNumber(step)) {
                        stepTo = $scope.steps[step];
                    } else {
                        stepTo = _.findWhere($scope.steps, { title: step });
                    }
                    $scope.goTo(stepTo);
                };

                this.finish = function () {
                    if ($scope.onFinish) {
                        $scope.onFinish();
                    }
                };

                this.cancel = this.previous = function () {
                    var index = _.indexOf($scope.steps, $scope.selectedStep);
                    if (index === 0) {
                        throw new Error("Can't go back. It's already in step 0");
                    } else {
                        $scope.goTo($scope.steps[index - 1]);
                    }
                };
            }]
        };
    }]);