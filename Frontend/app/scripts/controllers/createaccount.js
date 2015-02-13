'use strict';

/**
 * @ngdoc function
 * @name accountingApp.controller:CreateaccountCtrl
 * @description
 * # CreateaccountCtrl
 * Controller of the accountingApp
 */
angular.module('accountingApp')
  .controller('CreateaccountCtrl', function ($scope, $http, $location, ENV) {

    $scope.submitting = false;
    $scope.submitAccount = function () {
      $scope.submitting = true;

      $http.post(ENV.apiEndpoint + '/api/accounting/OpenAccount', {
        AccountName: $scope.accountName,
        AccountNumber: $scope.accountNumber
      }).
        success(function () {
          $location.path('/accounts')
        }).
        error(function (data) {
          $scope.errorMessage = data.ExceptionMessage;
        }).
        finally(function () {
          $scope.submitting = false;
        });
    };
  });
