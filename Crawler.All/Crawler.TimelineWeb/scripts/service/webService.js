'use strict';


// Demonstrate how to register services
// In this case it is a simple constant service.
angular.module('crawlerApp.services')
    .factory('Website', function ($http, $state, $cookieStore, CrawlerApi) {
        return {
            //Website를 통해서 Website의 정보를 가져오는 서비스
            getWebsite: function (websiteId, callback, error) {
                CrawlerApi.get("/WebsiteGetbyIdRequestDTO", { WebsiteId: websiteId },
					function (data) {
					    callback(data);
					},
					function (msg) {
					    error(msg);
					}
				);
                return null;
            },
            //Website의 정보를 List로 가져오는 서비스
            getWebsiteList: function (callback, error) {
                CrawlerApi.get("/WebsiteGetListRequestDTO", {},
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
