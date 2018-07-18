import { mapGetters, mapMutations } from "vuex";
import { page } from '../../data'
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
        ...mapGetters({'loading':'getIsLoading'}),
        page() { return page[this.name] },
        sort() { return this.query.sort },
        filterSelection() {
            let filterOptions = [];
            let filtersList = ['jobType', 'source', 'course', 'filter'];
            Object.entries(this.query).forEach(([key, val]) => {
                if (val && val.length && filtersList.includes(key)) {
                    [].concat(val).forEach(value => filterOptions = filterOptions.concat({ key, value }));
                }
            });
            return filterOptions;
        }
    },
    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    }
};