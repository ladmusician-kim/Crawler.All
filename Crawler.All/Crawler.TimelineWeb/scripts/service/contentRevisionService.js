'use strict';
// Demonstrate how to register services
// In this case it is a simple constant service.
angular.module('crawlerApp.services')
    .factory('ContentRevision', function ($http, $state, $cookieStore, CrawlerApi) {
        return {
            //BoardId를 통해서 각 Board에 해당하는 ContentRevision를 List로 가져오는 서비스
            getContentRevisionListbyBoardId: function (boardid, callback, error) {
                CrawlerApi.get("/ContentRevisionListGetbyBoardIdListRequestDTO", { boardId: boardid },
					function (data) {
					    callback(data);
					},
					function (msg) {
					    error(msg);
					}
				);
                return null;
            },
            //검색 Keywrod를 통해서 해당하는 ContentRevision를 List로 가져오는 서비스
            getContentRevisionListSearchResult: function (keyword, callback, error) {
                CrawlerApi.get("/ContentRevisionListGetbyKeywordRequestDTO", { Keyword: keyword },
					function (data) {
					    callback(data);
					},
					function (msg) {
					    error(msg);
					}
				);
                return null;
            }
        };
    });
