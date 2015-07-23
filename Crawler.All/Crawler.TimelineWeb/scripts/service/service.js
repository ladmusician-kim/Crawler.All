'use strict';

// Demonstrate how to register services
// In this case it is a simple constant service.
angular.module('crawlerApp.services', [])
    .factory('CrawlerApi', ['$http', function ($http) {

        //var sendError = function (requestUrl, params, errorMessage) {
        //    var fromUrl = document.URL;
        //    var strparams = JSON.stringify(params);
        //    //$http.post('http://localhost:61910/BugReportSendRequestDTO', { FromUrl: fromUrl, RequestUrl: requestUrl, Params: strparams, ErrorMessage: errorMessage });
        //}

        return {
            get: function ($url, $params, successcallback, failcallback) {
                $http.get('http://localhost:61910' + $url, { params: $params })
                    .success(function (data) {
                        if (data.resultType == "Success") {
                            successcallback(data.returnBody);
                        } else if (data.ResultType != "Success") {
                            //sendError($url, $params, data.errorMessage);
                            if (failcallback != undefined) {
                                failcallback(data.errorMessage);
                            }
                        }
                    }).error(function () {
                        //sendError($url, $params, data.errorMessage);
                    });
            },
            post: function ($url, $params, successcallback, failcallback) {
                $http.post('http://localhost:61910' + $url, $params)
                    .success(function (data) {
                        if (data.resultType == "Success") {
                            successcallback(data.returnBody);
                        } else if (data.ResultType != "Success") {
                            //sendError($url, $params, data.errorMessage);
                            if (failcallback != undefined) {
                                failcallback(data.errorMessage);
                            }
                        }
                    }).error(function () {
                        //sendError($url, $params, data.errorMessage);
                    });
            }
        };
    }]);

    //.factory('Auth', function ($http, $state, $cookieStore, CrawlerApi) {
    //    var accessLevels = routingConfig.accessLevels
    //        , userRoles = routingConfig.userRoles
    //        , currentUser = $cookieStore.get('user') || { username: '', role: userRoles.public };


    //    if (currentUser == null) {
    //        $cookieStore.remove('user');
    //    }

    //    function changeUser(user) {
    //        angular.extend(currentUser, user);
    //    }

    //    return {
    //        authorize: function (accessLevel, role) {
    //            if (role === undefined) {
    //                role = currentUser.role;
    //            }

    //            return accessLevel.bitMask & role.bitMask;
    //        },
    //        isLoggedIn: function (user) {
    //            if (user === undefined) {
    //                user = currentUser;
    //            }
    //            return user.role.title === userRoles.user.title || user.role.title === userRoles.admin.title;
    //        },
    //        register: function (user, success, error) {
    //            $http.post('/register', user).success(function (res) {
    //                changeUser(res);
    //                success();
    //            }).error(error);
    //        },
    //        signup: function (email, password, callback, error) {
    //            CrawlerApi.get("/UserCreateRequestDTO", { Email: email, Password: password },
	//				function (data) {
	//				    callback(data);
	//				},
	//				function (msg) {
	//				    error(msg);
	//				}
	//			);
    //        },
    //        login: function (user, success, error) {
    //            $http.post('http://api.mathbada.com/auth', user).success(function (user) {
    //                CrawlerApi.get("/UserGetRequestDTO", { UserName: user.userName },
	//					function (data) {
	//					    user.role = userRoles.user;
	//					    changeUser(user);
	//					    $cookieStore.put('user', user);
	//					    success(user);
	//					});
    //            }).error(error);
    //        },
    //        logout: function (success, error) {
    //            $http.post('http://api.mathbada.com/auth/logout').success(function () {
    //                changeUser({
    //                    username: '',
    //                    role: userRoles.public
    //                });
    //                $cookieStore.remove('user');
    //                $state.go("anon.signin")
    //                //success();
    //            }).error(error);
    //        },
    //        sendEmailToFindPassword: function (email, callback, error) {
    //            CrawlerApi.get("/UserSendEmailToFindPasswordRequestDTO", { Email: email },
	//				function (data) {
	//				    callback(data);
	//				},
	//				function (msg) {
	//				    error(msg);
	//				}
	//			);
    //        },
    //        validateGuidToChangePassword: function (guid, callback, error) {
    //            CrawlerApi.get("/UserValidateGuidToChangePasswordRequestDTO", { Guid: guid },
	//				function (data) {
	//				    callback(data);
	//				}, function (msg) {
	//				    error(msg);
	//				}
	//			);
    //        },
    //        changePassword: function (oldpassword, newpassword, callback, error) {
    //            CrawlerApi.get("/UserChangePasswordRequestDTO", { OldPassword: oldpassword, NewPassword: newpassword },
	//				function (data) {
	//				    callback(data);
	//				}, function (msg) {
	//				    error(msg);
	//				});
    //        },
    //        changePasswordByGuid: function (guid, newpassword, callback, error) {
    //            CrawlerApi.get("/UserChangePasswordByGuidRequestDTO", { Guid: guid, NewPassword: newpassword },
	//				function (data) {
	//				    callback(data);
	//				}, function (msg) {
	//				    error(msg);
	//				}
	//			);
    //        },
    //        accessLevels: accessLevels,
    //        userRoles: userRoles,
    //        user: currentUser
    //    };
    //})