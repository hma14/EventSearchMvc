/// <reference path="F:\Visual_Studio_2015_Apps\EventSearchMvc\EventSearchMvc\Scripts/angular.min.js" />

'use strict'

var app = angular.module('App', [])

app.controller('AppController', ['$scope', '$filter', '$http', '$localStorage',
    function ($scope, $filter, $http, $localStorage) {
        //$scope.startDate = $filter('date')(new Date('01/01/2016'), 'yyyy-MM-dd')
        //$scope.endDate = $filter('date')(new Date(), 'yyyy-MM-dd')

        $scope.init = function (location, category, startDate, endDate) {
            if (location !== undefined) {
                $scope.location = location
                $localStorage.location = location
            }
            else {
                $scope.location = $localStorage.location
            }
            if (category !== undefined) {
                $scope.category = category
                $localStorage.category = category
            }
            else {
                $scope.category = $localStorage.category
            }

        
            $scope.startDate = startDate == null ? $filter('date')(new Date('01/01/2016'), 'yyyy-MM-dd') : startDate
            $scope.endDate = endDate == null ?  $filter('date')(new Date(), 'yyyy-MM-dd') : endDate           
        }
    }])