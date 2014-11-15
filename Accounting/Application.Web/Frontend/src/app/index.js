'use strict';

angular.module('frontend', ['ngResource', 'ui.router'])
  .config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider
      .state('home', {
        url: '/',
        templateUrl: 'app/views/main/main.html',
        controller: 'MainCtrl'
      })
    .state('api', {
      url: '/api',
      templateUrl: 'app/views/api/api.html',
      controller:'ApiCtrl'
    });

    $urlRouterProvider.otherwise('/');
  })
;
