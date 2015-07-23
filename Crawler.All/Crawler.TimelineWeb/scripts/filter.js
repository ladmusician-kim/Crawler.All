'use strict';

//filters
angular.module('filters', [])
.filter('fixTime', ['$filter', function ($filter) {
    return function (date) {

        var CurrentTime = new Date();
        var hours = Math.abs(CurrentTime - date) / 3600000;

        if (hours > 24)
        {
            var Time = $filter('date')(new Date(), 'yyyy-MM -dd');
        }
        else
        {
            var Time = $filter('date')(new Date(), 'HH:mm:ss');
        }

        var Time = $filter('date')(new Date(), 'HH:mm:ss');

        //yyyy - MM - dd
        return (Time);
    };
}])
.filter('substringArticle', ['$filter', function ($filter) {
    return function (article) {
        var subArticle = article.substring(0, 10) + "...";
        return (subArticle);
    };
}])

