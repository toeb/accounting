var Directives;
(function (Directives) {
    var AccountListDirective = (function () {
        function AccountListDirective() {
            this.restrict = 'E';
            this.templateUrl = '/app/directives/AccountListDirective.html';
            this.scope = {
                accounts: '=',
                search: '=',
                selected: '=?'
            };
            this.link = function (scope) {
                scope.onAccountClick = function (account) {
                    console.log('account clicked: ' + account);
                };
            };
        }
        return AccountListDirective;
    })();
    Directives.AccountListDirective = AccountListDirective;
    angular.module('frontend').directive('accountList', [function () { return new AccountListDirective(); }]);
})(Directives || (Directives = {}));
//# sourceMappingURL=AccountListDirective.js.map