'use strict';
/* App Controllers */
angular.module('crawlerApp.controllers', ['crawlerApp.services']);

app.controller('navController', ['$scope', function ($scope) {
    $scope.tabs = [true, false, false];
    $scope.tab = function (index) {
        angular.forEach($scope.tabs, function (i, v) {
            $scope.tabs[v] = false;
        });
        $scope.tabs[index] = true;
    }
    console.log("SearchDetailCtrl");


}])
