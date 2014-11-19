'use strict';

angular.module('frontend', ['ngResource', 'ui.router', 'ngMaterial'])
  .config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider
      .state('app', {
        url: '/',
        views: {
          "": {
            templateUrl: 'app/views/main/main.html',
            controller: 'MainCtrl'
          },
          "navigation": {
            templateUrl: 'components/navbar/navbar.html',
            controller: 'NavbarCtrl'
          }
        }
      })
    .state('accounts', {
      url: '/accounts',
      abstract:true,
      views: {
        "": {
          templateUrl: 'app/views/accounts/accounts.html',
          controller: 'AccountsCtrl'
        },
        "navigation": {
          templateUrl: 'components/navbar/navbar.html',
          controller: 'NavbarCtrl'
        }
      },
      resolve: {
        accounts: ['accountingService', function (accountingService) {
          return accountingService.getAccounts();
        }]
      }
    })
    .state('accounts.open', {
      url: '/open',
      displayName:'Open Account',
      views: {
        "": {
          templateUrl: 'app/views/accounts/open.html',
          controller: 'AccountsCtrl'
        }
      }
    })
    .state('accounts.list', {
      displayName:'Show Accounts',
      url: '',
      views: {
        "": {
          templateUrl: 'app/views/accounts/list.html',
          controller:'AccountsCtrl'
        }
      }
    })
    .state('accounts.details', {
      url: '/{accountId:[0-9]{1,4}}',
      views:{
        '':{
          templateUrl: 'app/views/accounts/details.html',
          controller:'AccountsCtrl'
        }
      }
    });


    $urlRouterProvider.otherwise('/');
  })
;
