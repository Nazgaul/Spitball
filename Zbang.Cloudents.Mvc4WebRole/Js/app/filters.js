var filters = angular.module('Filters', []);

filters.filter('orderByFilter', function () {
    return function (items, filterData, reverse) {
        var filtered = [],           
            searchValue = filterData.input ? filterData.input.toLowerCase() : filterData.input;
        
        angular.forEach(items, function (item) {
            filtered.push(item);
        });
        filtered.sort(function (a, b) {         
            var aName = a.name.toLowerCase(),
                      bName = b.name.toLowerCase(),
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
});
