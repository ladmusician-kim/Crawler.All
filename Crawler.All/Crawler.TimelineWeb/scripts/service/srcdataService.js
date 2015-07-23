'use strict';


// Demonstrate how to register services
// In this case it is a simple constant service.
angular.module('crawlerApp.services')
    .factory('Srcdata', function ($http, $state, $cookieStore, CrawlerApi) {
        return {
            //안쓰는 서비스 임.
            getSrcdata: function (for_contentid, callback, error) {
                CrawlerApi.get("/SrcdataGetbycontentIdRequestDTO", { For_ContentId: for_contentid },
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
