'use strict';


// Demonstrate how to register services
// In this case it is a simple constant service.
angular.module('crawlerApp.services')
    .factory('Board', function ($http, $state, $cookieStore, CrawlerApi) {
        return {
            //BoardId를 통해서 Board의 정보를 가져오는 서비스
            getBoard: function (boardid, callback, error) {
                CrawlerApi.get("/BoardGetbyIdRequestDTO", { BoardId: boardid },
					function (data) {
					    callback(data);
					},
					function (msg) {
					    error(msg);
					}
				);
                return null;
            },
            //Board의 정보를 List로 가져오는 서비스
            getBoardList: function (callback, error) {
                CrawlerApi.get("/BoardGetListRequestDTO", {},
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
