﻿
app.filter('orderByFilter',
[
    function () {
        "use strict";
        return function (items, filterData, reverse) {
            var filtered,
                searchValue = filterData.input ? filterData.input.toLowerCase() : '';

            filtered = _.filter(items, function (item) {
                var filterObj = item[filterData.field] || '';
                return filterObj.toLowerCase().indexOf(searchValue) > -1;
            });
            filtered.sort(function (a, b) {
                var aName = (a.name || '').toLowerCase(),
                          bName = (b.name || '').toLowerCase(),
                          aIndex, bIndex;

                if (searchValue && searchValue.length) {
                    aIndex = aName.indexOf(searchValue);
                    bIndex = bName.indexOf(searchValue);
                    if (aIndex === -1 && bIndex === -1 || aIndex === bIndex) {
                        return sortByName();
                    } else if (aIndex === -1) {
                        return 1;
                    } else if (bIndex === -1) {
                        return -1;
                    }

                    return sortByIndex();
                }

                return sortByName();

                function sortByIndex() {
                    if (aIndex < bIndex) {
                        return -1;
                    } else if (aIndex > bIndex) {
                        return 1;
                    }
                    return 0;
                }

                function sortByName() {
                    if (aName < bName) {
                        return -1;
                    } else if (aName > bName) {
                        return 1;
                    }

                    return 0;
                }
            });

            if (reverse) {
                filtered.reverse();
            }
            return filtered;
        };
    }
]);
