import { mapGetters, mapMutations } from "vuex";
import { page } from "../../services/navigation/vertical-navigation/nav";
import mobileSortAndFilter from '../SortAndFilter/MobileSortAndFilter.vue';
import sortAndFilter from '../SortAndFilter/SortAndFilter.vue';


export default {
    data() {
        return {
            filter: '',
            filtersDefault: {}
        };
    },
    components: {SortAndFilter: sortAndFilter, MobileSortAndFilter: mobileSortAndFilter},

    props: {
        name: {type: String},
        query: {type: Object},
        params: {type: Object}
    },
    computed: {
        ...mapGetters({
            'loading': 'getIsLoading',
            'getFilters': 'getFilters',
            'getSort': 'getSort'
            }),
        page() {
            return page[this.name];
        },
        sort() {
            return this.query.sort;
        },

        filterSelection() {
            /**
             * return {
             *  key,
             *  value,
             *  name
             * }
             */
            let filterOptions = [];
            let filters = this.getFilters;
            if (!!filters) {
                // iterate filter and add/remove filter value
                Object.entries(this.query).forEach(([key, vals]) => {
                    if(!filters.filterChunkDictionary[key] || key === 'sort') return;
                    let filterIds = vals; //could be a string not only array (e.g: sort)
                    if (typeof filterIds === 'object') { // TODO sort could be alos an object
                        if (filterIds.length === 0) {
                            return filterOptions;
                        }
                        filterIds.forEach((id) => {
                            let filterId = id;
                            let filterType = key;
                            let name = filters.filterChunkDictionary[filterType].dictionaryData[filterId];
                            let filterItem = {
                                filterId,
                                filterType,
                                name
                            };
                            filterOptions.push(filterItem);
                        });
                    } else {
                        let filterId = filterIds;
                        let filterType = key;
                        let name = filters.filterChunkDictionary[filterType].dictionaryData[filterId];
                        let filterItem = {
                            filterId,
                            filterType,
                            name
                        };
                        filterOptions.push(filterItem);
                    }
                });
            }

            return filterOptions;
        }
    },
    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    }
};