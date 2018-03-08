import sortAndFilterMixin from '../mixins/sortAndFilterMixin'
import ResultBook from './bookCell.vue';
import TabsSort from "./bookDetailsSort"
import { details } from "../../data";
const all = "all";
const filterOptions = [{ title: "Book Type", modelId: "filter", data: details.bookDetails.filter }];
const FILTER_LIST=details.bookDetails.filter;
import Analytics from '../mixins/googleAnalyticsTracking'
export default {
    mixins: [sortAndFilterMixin, TabsSort,Analytics],
    created() {
        this.filter = all;
        this.UPDATE_LOADING(true);
        this.$store.dispatch("bookDetails", { pageName: "bookDetails", isbn13: this.params.id, type: "buy" }).then(({ data }) => {
            this.pageData = data;
            this.UPDATE_LOADING(false);
        });
    },
    data() {
        return {
            pageData: '',
            sortVal: "buy",
            showFilters: false,
            filterOptions
        };
    },
    components: { ResultBook },
    watch:{
        pageData(val){
            if(this.sortVal==='buy') {
                this.filterOptions=filterOptions;
                this.filterOptions[0].data = (!val || !val.data || val.data.length === 1) ? [] : FILTER_LIST.filter(i => val.data.map(t => t.condition).includes(i.id));
            }else{this.filterOptions=[]}
        }
    },
    computed: {
        currentType() { return this.sortVal ? this.sortVal.charAt(0).toUpperCase() : "" },
        selectedFilter() { return this.query.filter ? [].concat(this.query.filter).map(i => ({ key: 'filter', value: i })) : [] },
        sort: () => 'price',
        filteredList: function () {
            return !this.pageData.data ? [] : !this.selectedFilter.length ? this.pageData.data : this.pageData.data.filter(item => [].concat(this.query.filter).includes(item.condition));
        }
    },

    filters: {
        floatDot: function (value, a) {
            return value.toFixed(a)
        }
    },
    beforeRouteUpdate(to, from, next) {
        if (to.query.sort && to.query.sort !== this.sortVal) {
            this.updateSort(to.query.sort)
        } else if (to.query.sort) {
            this.$router.replace({ query: { filter: to.query.filter } });
        } else {
            next();
        }
    },
    methods: {
        updateSort(val) {
            this.sortVal = val;
            this.$_changeTab(val);
        }
    }
}