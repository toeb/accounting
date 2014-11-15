'use strict';

angular.module('frontend', ['ngResource', 'ui.router', 'ngMaterial'])
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
    })
    .state('accounts',{
      url: '/accounts',
      templateUrl: 'app/views/accounts/accounts.html',
      controller:'AccountsCtrl'
    });

    $urlRouterProvider.otherwise('/');
  })
;
