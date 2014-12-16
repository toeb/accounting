'use strict';

/**
 * @ngdoc function
 * @name accountingApp.controller:AboutCtrl
 * @description
 * # AboutCtrl
 * Controller of the accountingApp
 */
angular.module('accountingApp')
  .controller('AboutCtrl', function ($scope) {
    $scope.awesomeThings = [
      'HTML5 Boilerplate',
      'AngularJS',
      'Karma'
    ];
  });
