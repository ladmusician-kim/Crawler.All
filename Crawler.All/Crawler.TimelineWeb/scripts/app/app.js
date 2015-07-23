'use strict';

var app = angular.module('crawlerApp', ['ui.router'])
.config(["$stateProvider", "$urlRouterProvider"], function ($stateProvider, $urlRouterProvider) {
    //$urlRouterProvider.otherwise('/');

    $stateProvider

        // HOME STATES AND NESTED VIEWS ========================================
        .state('main', {
            url: '',
            templateUrl: '/tpl/aggregatePanel.tpl.html'
        })
        .state('main.aggregate', {
            url: '/aggregate',
            templateUrl: '/tpl/aggregateList.tpl.html',
        })

        // nested list with just some random string data
        .state('main.reality', {
            url: '/paragraph',
            template: 'I could sure use a drink right now.'
        })

        // WebsiteDetails Page with Paramas: WebsiteId
        .state('website', {
            url: '/website',
            templateUrl: '/tpl/website/websiteDetails.tpl.html',
        })

        // DatePage
        .state('date', {
            url: '/date',
            templateUrl: '/tpl/datePanel.tpl.html',
        })

        // SearchResultpage with Params: string Search
        .state('searchResult', {
            url: '/searchResult',
            templateUrl: '/tpl/search/searchResult.tpl.html',
        })

        // SearchResultDetails with Params: stirng Search
        .state('searchResultDetails', {
            url: '/searchResultDetails',
            templateUrl: '/tpl/search/searchResultDetails.tpl.html',
        })

        // ABOUT PAGE AND MULTIPLE NAMED VIEWS =================================
        .state('about', {
            url: '/about',
            views: {
                '': { templateUrl: 'partial-about.html' },
                'columnOne@about': { template: 'Look I am a column!' },
                'columnTwo@about': {
                    templateUrl: 'table-data.html',
                    controller: 'scotchController'
                }
            }
        });

});

app.controller('scotchController', function ($scope) {

    $scope.message = 'test';

    $scope.scotches = [
        {
            name: 'Macallan 12',
            price: 50
        },
        {
            name: 'Chivas Regal Royal Salute',
            price: 10000
        },
        {
            name: 'Glenfiddich 1937',
            price: 20000
        }
    ];
});

'use strict';

angular.module('app', [
    'ngAnimate',
    'ngCookies',
    'ngResource',
    'ngSanitize',
    'ngTouch',
    'ngStorage',
    'ui.router',
    'ui.bootstrap',
    'ui.load',
    'ui.jq',
    'ui.validate',
    'oc.lazyLoad',
    'pascalprecht.translate',
    'crawlerApp.services'
]); 