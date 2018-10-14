import { mapGetters, mapMutations } from "vuex";
import { page } from "../../services/navigation/vertical-navigation/nav";
const MobileSortAndFilter = () => import('../SortAndFilter/MobileSortAndFilter.vue');
const SortAndFilter = () => import('../SortAndFilter/SortAndFilter.vue');
const plusBtn = () => import("../settings/svg/plus-button.svg");


export default {
    data() {
        return {
            filter: '',
            filtersDefault: {}
        };
    },
    components: {SortAndFilter, plusBtn, MobileSortAndFilter},

    props: {
        name: {type: String},
        query: {type: Object},
        params: {type: Object}
    },
    computed: {
        ...mapGetters({'loading': 'getIsLoading', 'getFilters': 'getFilters', 'getSort': 'getSort'}),
        page() {
            return page[this.name]
        },
        sort() {
            return this.query.sort
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
            if(!!filters){
                // iterate filter and add/remove filter value
                Object.entries(this.query).forEach(([key, vals]) => {
                    let filterIds = vals; //could be a string not only array (e.g: sort)
                    if(typeof filterIds === 'object'){ // TODO sort could be alos an object
                        if(filterIds.length === 0){
                            return filterOptions;
                        }
                        filterIds.forEach((id)=>{
                            let filterId = id;
                            let filterType = key;
                            let name = filters.filterChunkDictionary[filterType].dictionaryData[filterId];
                            let filterItem = {
                                filterId,
                                filterType,
                                name
                            };
                            console.log('item filter', filterItem)
                            filterOptions.push(filterItem);
                        })
                    }
                });
            }

            return filterOptions;
        }
    },
    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    },
    created() {
        console.log(this.query)
    }
};