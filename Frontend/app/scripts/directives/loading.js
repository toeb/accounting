'use strict';

/**
 * @ngdoc directive
 * @name accountingApp.directive:loading
 * @description
 * # loading
 */
angular.module('accountingApp')
  .directive('loading', function () {
    return {
      template: '<img src="/images/loading.gif" alt="loading..."></img>',
      restrict: 'E',
      link: function postLink(scope, element, attrs) {
      }
    };
  });
