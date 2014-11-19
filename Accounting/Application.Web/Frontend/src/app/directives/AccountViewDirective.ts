module Directives {


  export class AccountViewDirective {
    restrict= 'E';
    templateUrl = '/app/directives/AccountViewDirective.html';
    scope= {
      account: '='
    };
    constructor() {
    }

  }

  angular.module('frontend').directive('accountView', [()=>new AccountViewDirective()]);

}