var Directives;
(function (Directives) {
    var AccountViewDirective = (function () {
        function AccountViewDirective() {
            this.restrict = 'E';
            this.templateUrl = '/app/directives/AccountViewDirective.html';
            this.scope = {
                account: '='
            };
        }
        return AccountViewDirective;
    })();
    Directives.AccountViewDirective = AccountViewDirective;
    angular.module('frontend').directive('accountView', [function () { return new AccountViewDirective(); }]);
})(Directives || (Directives = {}));
//# sourceMappingURL=AccountViewDirective.js.map