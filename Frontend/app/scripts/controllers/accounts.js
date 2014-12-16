'use strict';

/**
 * @ngdoc function
 * @name accountingApp.controller:AccountsCtrl
 * @description
 * # AccountsCtrl
 * Controller of the accountingApp
 */
angular.module('accountingApp')
  .controller('AccountsCtrl', function ($scope, $http, ENV) {
    $scope.accounts = [];

    $scope.loading = false;
    $scope.init = function () {
      $scope.loading = true;
      $http.get(ENV.apiEndpoint + '/api/accounting/accounts', {}).
        success(function (data) {
          $scope.accounts = data;
        }).
        finally(function () {
          $scope.loading = false;
        });
    }

    $scope.init();
  });
