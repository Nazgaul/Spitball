import { mapGetters, mapMutations } from "vuex";
import { page }  from "../../services/navigation/vertical-navigation/nav";
const MobileSortAndFilter = () => import('../SortAndFilter/MobileSortAndFilter.vue');
const SortAndFilter = () => import('../SortAndFilter/SortAndFilter.vue');
const plusBtn = () => import("../settings/svg/plus-button.svg");


export default {
    data() {
        return {
            filter: ''
        };
    },
    components: { SortAndFilter, plusBtn, MobileSortAndFilter },

    props: {
        name: { type: String },
        query: { type: Object },
        params: { type: Object }
    },
    computed: {
        ...mapGetters({'loading':'getIsLoading', 'getFilters': 'getFilters'}),
        page() { return page[this.name] },
        sort() { return this.query.sort },
        filterSelection() {
            let filterOptions = [];
            let filtersList = Object.keys(this.getFilters);
            // let filtersList = ['jobType', 'subject', 'course', 'filter'];
            Object.entries(this.query).forEach(([key, val]) => {
                if (val && val.length && filtersList.includes(key)) {
                    [].concat(val).forEach((value) =>{
                        return  filterOptions = filterOptions.concat({ key, value })
                    }) ;
                }
            });
            console.log('options',filterOptions)
            return filterOptions;
        }
    },
    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    }
};