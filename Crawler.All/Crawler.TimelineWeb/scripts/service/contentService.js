'use strict';
// Demonstrate how to register services
// In this case it is a simple constant service.
angular.module('crawlerApp.services')
    .factory('Content', function ($http, $state, $cookieStore, CrawlerApi) {
        return {
            //안쓰는 서비스 임.
            getContent: function (contentId, callback, error) {
                CrawlerApi.get("/ContentGetbyIdRequestDTO", { ContentId: contentId },
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
