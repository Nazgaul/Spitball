﻿angular.module('libChoose', ['ajax']).
    controller('LibChooseController',
    ['libChooseService', function (libChooseService) {
        var libChoose = this;

        var page,
            isChosen,
            isSearching,
            endResult;

        libChooseService.facebookSuggestions().then(function (response) {
            libChoose.facebookSuggestions = response;
            libChooseService.doneLoad();
        });

        libChoose.selectUniversity = function (universityId) {
            isChosen = true;
            libChooseService.selectUniversity(universityId).catch(function (response) {
                isChosen = false;
            });
        };

        libChoose.search = function (isAppend) {
            if (isSearching) {
                return;
            }

            if (isAppend && endResult) {
                return;
            }

            if (libChoose.query && libChoose.query.length < 2) {
                return;
            }

            if (!libChoose.query) {
                libChoose.universities = [];
                return;
            }

            libChoose.noResults = false;
            isSearching = true;

            if (!isAppend) {
                page = 0;
                endResult = false;
            }

            libChooseService.searchUnis(libChoose.query, page).then(function (response) {
                response = response || [];
                page++;

                if (!isAppend) {

                    if (!response.length) {
                        libChoose.noResults = true;
                    }

                    libChoose.universities = response;
                    return;
                }

                if (!response.length) {
                    endResult = true;                    
                    return;
                }

                libChoose.universities = libChoose.universities.concat(response);

            }).finally(function () {
                isSearching = false;
            });


        };



    }]
);