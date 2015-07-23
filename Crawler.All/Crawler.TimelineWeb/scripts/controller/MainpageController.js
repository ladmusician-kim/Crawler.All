'use strict';
/* App Controllers */
angular.module('crawlerApp.controllers')
.controller('MainpageCtrl', ['$scope', 'Website', 'Board', 'Content', 'Snapshot', 'Srcdata', 'Timeperiod', function ($scope, Website, Board, Content, Snapshot, Srcdata, Timeperiod)
{
    $scope.boardList = [];
    $scope.selectedBoardContentsList = [];
    $scope.SelectedcontentRevisionList = [];

    //BoardList 들을 들고 오는 부분
    Board.getBoardList(function (data) {
        $scope.boardList = data;
        console.log(data);
    }, function (msg) {
        alret(msg);
    });

    $scope.websiteList = [];
    Website.getWebsiteList(function (data) {
        $scope.websiteList = data;

    }, function (msg) { 
        alret(msg);
    });

    $scope.DateTimeNow = new Date();
}])