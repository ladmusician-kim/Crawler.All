'use strict';
/* App Controllers */
angular.module('crawlerApp.controllers')
.controller('WebsiteDetailsCtrl', ['$scope', '$stateParams', 'Website', 'Board', 'Content', 'Snapshot', 'Srcdata', 'Timeperiod', 'ContentRevision', function ($scope, $stateParams, Website, Board, Content, Snapshot, Srcdata, Timeperiod, ContentRevision)
{
    $scope.boardId = $stateParams.websiteId;

    $scope.selectedBoardContentsList = [];
    $scope.SelectedcontentRevisionList = [];

    ContentRevision.getContentRevisionListbyBoardId($scope.boardId, function (data) {
        $scope.SelectedcontentRevisionList = data;
        console.log("$scope.SelectedcontentRevisionList ");
        console.log($scope.SelectedcontentRevisionList);
    });
    Board.getBoard($scope.boardId, function (data) {
        console.log("data");
        console.log(data);

        $scope.board = data;

        Website.getWebsite($scope.board.id, function (website) {
            $scope.website = website;
            console.log($scope.website);
        })
    }, function (msg) {
        alret(msg);
    });

    //모두 날짜를 선택하는 함수

    $scope.today = function () {
        $scope.startDate = null;
        $scope.endDate = null;
    };
    $scope.today();

    $scope.clear = function () {
        $scope.startDate = null;
        $scope.endDate = null;
    };

    // Disable weekend selection
    $scope.disabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() === 0 || date.getDay() === 6));
    };

    $scope.toggleMin = function () {
        $scope.minDate = $scope.minDate ? null : new Date();
    };
    $scope.toggleMin();

    $scope.open = function ($event, which) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.formDates[which] = true;
    };

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.formDates = {
        startdate: false,
        enddate: false
    };

    $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];
}])
