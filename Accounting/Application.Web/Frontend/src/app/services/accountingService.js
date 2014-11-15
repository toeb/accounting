
angular.module('frontend').service('accountingService', ['$http', function ($http) {

  var service = {};

  service.result = {};
  service.query = function () {
    service.result = "pending";
    $http.get('/api/Values')
      .success(function (res) { service.result = res; })
      .error(function () { service.result = "failed"; });
  }

  return service;

}]);