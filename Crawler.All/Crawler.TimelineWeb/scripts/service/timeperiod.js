'use strict';


// Demonstrate how to register services
// In this case it is a simple constant service.
angular.module('crawlerApp.services')
    .factory('Timeperiod', function ($http, $state, $cookieStore, CrawlerApi) {
        return {
            //안쓰는 서비스 임.
            getTimeperiod: function (timeperiodId, callback, error) {
                CrawlerApi.get("/TimePeriodGetbyIdRequestDTO", { TimeperiodId: timeperiodId },
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
