'use strict';
/* App Controllers */
angular.module('crawlerApp.controllers')
.controller('DatepageCtrl', ['$scope', 'Website', 'Board', 'Content', 'Snapshot', 'Srcdata', 'Timeperiod', function ($scope, Website, Board, Content, Snapshot, Srcdata, Timeperiod)
{
    $scope.boardList = [];
    //Board_ContentRevision의 정보를 들고있는 List
    $scope.boardContentRevisionList = [];
    //ContentRevision의 정보를 들고있는 List
    $scope.contentRevisionList = [];

    //boardList를 불러오는 코드
    Board.getBoardList(function (data) {
        $scope.boardList = data;
        $scope.boardList.forEach(
             function (Board) {
                 Board.snapshots.forEach(
                     function (Snapshot) {
                         Snapshot.contentRevisions.forEach(
                         function (eachContentRevision) {
                             $scope.contentRevisionList.push(eachContentRevision);
                         });
                     });
                 $scope.boardContentRevisionList.push($scope.contentRevisionList);
                 $scope.contentRevisionList = [];
             });
    }, function (msg) {
        alret(msg);
    });

    //websiteList를 불러오는 코드
    $scope.websiteList = [];
    Website.getWebsiteList(function (data) {
        $scope.websiteList = data;


    }, function (msg) {
        alret(msg);
    });



    //체크된 Board의 정보를 들고있는 List
    $scope.selectedBoardList = [];
    //체크된 Board들의 Snapshot을 모아 놓은 List의 ContentRevision 정보들만 모아놓은 List
    $scope.selectedcontentRevisionList = [];
    //조회 버튼을 눌린 여부를 판단하는 Bolean 값.
    $scope.isShowResult = false;

    //체크 박스를 클릭했을때의 함수. -> 해당 board 아이디의 contentrevigion을 갯수 만큼 들고온다 -> 해당 contentrevigion의 for_cotentid를 통해서 content를 들고온다
    $scope.clickBoard = function (board) {
        $scope.isShowResult = !$scope.isShowResult;
        if ($scope.isShowResult)
        {
            $scope.selectedBoardList.push($scope.boardList[board.id - 1]);
            $scope.selectedcontentRevisionList.push($scope.boardContentRevisionList[board.id - 1]);
            console.log("$scope.selectedcontentRevisionList");
            console.log($scope.selectedcontentRevisionList);
        }
        else
        {
            $scope.selectedcontentRevisionList.delete($scope.boardContentRevisionList[board.id - 1]);
        }
    }

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

        //$scope.toggleMin = function () {
        //    $scope.minDate = $scope.minDate ? null : new Date();
        //};
        //$scope.toggleMin();

        $scope.open = function ($event, which) {
            $event.preventDefault();
            $event.stopPropagation();
        
            $scope.formDates[which] = true;
        };

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1
        };

        $scope.formDates= {
            startdate: false,
            enddate: false
        };

        $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
        $scope.format = $scope.formats[0];

    // 웹사이트 선택
        $scope.DateBoardRequestList = [];

        $scope.addBoard = function (boardid)
        {
            $scope.DateBoardRequestList.push(boardid);
            console.log($scope.DateBoardRequestList);
        }
        $scope.radioModel = 'Middle';

        $scope.checkModel = {
            left: false,
            middle: true,
            right: false
        };
        //Service로 고우!
        Website.getWebsiteList(function (data) {
            $scope.websiteList = data;
        }, function (msg) {
            alret(msg);
        });

        $scope.selectedSnapshotList = [];
        $scope.DateSearch = function()
        {
            console.log("$scope.startDate");
            console.log($scope.startDate);
            console.log("$scope.endDate");
            console.log($scope.endDate);

            Snapshot.getSnapshotListbyDateAndBoardIdList($scope.startDate, $scope.endDate, $scope.DateBoardRequestList, function (data) {
                $scope.selectedSnapshotList = data;
            }, function (msg) {
                alert(msg);
            });
        }

}])
