'use strict';

describe('Controller: CreateaccountCtrl', function () {

  // load the controller's module
  beforeEach(module('accountingApp'));

  var CreateaccountCtrl,
    scope;

  // Initialize the controller and a mock scope
  beforeEach(inject(function ($controller, $rootScope) {
    scope = $rootScope.$new();
    CreateaccountCtrl = $controller('CreateaccountCtrl', {
      $scope: scope
    });
  }));

  it('should attach a list of awesomeThings to the scope', function () {
    expect(scope.awesomeThings.length).toBe(3);
  });
});
