'use strict';

angular.module('frontend')
  .controller('AccountsCtrl', function ($scope, accountingService) {
    $scope.service = accountingService;
    $scope.message = 'hello from accounts controller';
  });
