'use strict';


// Demonstrate how to register services
// In this case it is a simple constant service.
angular.module('crawlerApp.services')
    .factory('Snapshot', function ($http, $state, $cookieStore, CrawlerApi) {
        return {
            //안쓰는 서비스 임.
            getSnapshotbyId: function (snapshotId, callback, error) {
                CrawlerApi.get("/SnapshotGetbyIdRequestDTO", { SnapshotId: snapshotId },
					function (data) {
					    callback(data);
					},
					function (msg) {
					    error(msg);
					}
				);
            },
            //안쓰는 서비스 임.
            getSnapshotbyBoardId: function (for_boardid, callback, error) {
                CrawlerApi.get("/SnapshotGetbyBoardIdRequestDTO", { For_BoardId: for_boardid },
					function (data) {
					    callback(data);
					},
					function (msg) {
					    error(msg);
					}
				);
                return null;
            },
            //안쓰는 서비스 임.
            getSnapshotListbyBoardId: function (for_boardid, callback, error) {
                CrawlerApi.get("/SnapshotListGetbyBoardIdRequestDTO", { For_BoardId: for_boardid },
					function (data) {
					    callback(data);
					},
					function (msg) {
					    error(msg);
					}
				);
                return null;
            },
            //startdate, enddate, boardidList를 이용하여 해당하는 snapshotList를 불러오는 서비스
            getSnapshotListbyDateAndBoardIdList: function (startdate,enddate,BoardList, callback, error) {
                CrawlerApi.get("/SnapshotListGetbyDataAndBoardIdRequestDTO", { StartDate: startdate, EndDate: enddate, boardIdList: BoardList },
					function (data) {
					    callback(data);
					},
					function (msg) {
					    error(msg);
					}
				);
                return null;
            },
        };
    });
