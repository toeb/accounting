'use strict';

/**
 * @ngdoc function
 * @name accountingApp.controller:GlobalCtrl
 * @description
 * # GlobalCtrl
 * Controller of the accountingApp
 */
angular.module('accountingApp')
  .controller('GlobalCtrl', function ($scope, translationService) {
    translationService.getTranslation($scope, 'en')
  });
