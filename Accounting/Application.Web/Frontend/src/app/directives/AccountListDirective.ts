module Directives {
  export class AccountListDirective {
    restrict = 'E';
    templateUrl = '/app/directives/AccountListDirective.html';
    scope = {
      accounts: '=',
      search: '=',
      selected:'=?'
    };
    link: any;
    constructor() {
      this.link = (scope)=>{
        scope.onAccountClick = (account) =>{
          console.log('account clicked: ' + account);

        }
      }  
    }
  }
  angular.module('frontend').directive('accountList', [() => new AccountListDirective()]);
}