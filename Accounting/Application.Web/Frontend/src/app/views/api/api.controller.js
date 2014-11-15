'use strict';

angular.module('frontend')
  .controller('ApiCtrl', function ($scope, accountingService) {
    $scope.service = accountingService;
    $scope.message = 'hello from api controller';
  });
