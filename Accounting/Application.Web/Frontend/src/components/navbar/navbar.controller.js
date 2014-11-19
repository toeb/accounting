'use strict';

angular.module('frontend')
  .controller('NavbarCtrl', function ($scope, $state) {
    $scope.selectedItem = {};

    var states = $state.get();
    console.log(states);
    $scope.items = [];
    states.forEach(function (state) {
      console.log(state);
      if (!!state.name && !state.abstract &&!!state.displayName) {
        $scope.items.push(state);
      }
    });

  });
