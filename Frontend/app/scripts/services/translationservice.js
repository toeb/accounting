'use strict';

/**
 * @ngdoc service
 * @name accountingApp.translationService
 * @description
 * # translationService
 * Service in the accountingApp.
 */
angular.module('accountingApp')
  .service('translationService', function($resource) {
    this.getTranslation = function($scope, language) {
      var languageFilePath = '/translations/' + language + '.json';
      $resource(languageFilePath).get(function (data) {
        $scope.translation = data;
      });
    };
  });
