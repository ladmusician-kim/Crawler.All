'use strict';
/* App Controllers */
angular.module('crawlerApp.controllers')
.controller('SearchDetailCtrl', ['$scope', '$stateParams', 'ContentRevision', function ($scope, $stateParams,ContentRevision)
{
    // Keyword를 통해서 찾은 ContentRevision의 List
    $scope.ContentRevisionListSearchResultList = null;

    //검색란에 들어갈 Text가 들어가는곳
    $scope.searchText = '';

    //검색 버튼을 눌렀을때 실행 되는 함수
    $scope.search = function () {
        //ContentReviison의 Content의 Article에서 비슷한 
        ContentRevision.getContentRevisionListSearchResult($scope.searchText, function (data) {
            $scope.ContentRevisionListSearchResultList = data;
            console.log("getContentRevisionListSearchResult");
            console.log($scope.searchText);
            console.log($scope.ContentRevisionListSearchResultList);
        }, function (msg) {
            $scope.ContentRevisionListSearchResultList = [];
            console.log("$scope.ContentRevisionListSearchResultList.length");
            console.log($scope.ContentRevisionListSearchResultList.length);
        });
    };

    $scope.recommendKeywordList = ["김동진 폭행", "김동진 자퇴", "강용석", "이준석", "강적들", "클라쎄스튜디오", "세종시", "일베", "병신들", "오유"];

    $scope.goInput = function (keyword) {
        $scope.searchText = keyword;
        $scope.search();
    };
}])
