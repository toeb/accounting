'use strict';

describe('Service: translationService', function () {

  // load the service's module
  beforeEach(module('accountingApp'));

  // instantiate service
  var translationService;
  beforeEach(inject(function (_translationService_) {
    translationService = _translationService_;
  }));

  it('should do something', function () {
    expect(!!translationService).toBe(true);
  });

});
