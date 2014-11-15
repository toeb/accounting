
angular.module('frontend').service('accountingService', ['$http', function ($http) {

  var service = {};
  service.errors = [];

  service.result = {};
  service.query = function () {
    service.result = "pending";
    $http.get('/api/Values')
      .success(function (res) { service.result = res; })
      .error(function () { service.result = "failed"; });
  }




  service.accounts = [];
  service.getAccounts = function () {
    service.accounts = [];
    return $http.get('/api/Accounting/GetAccounts')
    .success(function (res) { service.accounts = res; })
    .error(function () { service.accounts = []; });
  };
  service.account = {};

  service.openAccount = function () {
    console.log(service.account);
    return $http.post('/api/Accounting/OpenAccount',  service.account )
   .success(function (res) {
     service.account = res;
     service.getAccounts();
   })
   .error(function (err) {
     service.errors.push(err);
   });

  };
  return service;

}]);