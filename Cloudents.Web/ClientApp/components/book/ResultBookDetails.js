import sortAndFilterMixin from '../mixins/sortAndFilterMixin'
import ResultBook from './bookCell.vue';
import TabsSort from "./bookDetailsSort"
import { details } from "../../services/navigation/vertical-navigation/nav";
import SearchService from '../../services/searchService'

const sortOptions = {title: "Sort", id: "sort", data: details.bookDetails.sort};
const filters = SearchService.createFilters([{title: "Book Type", id: "filter", data: details.bookDetails.filter}]);
//const FILTER_LIST = details.bookDetails.filter;
export default {
    mixins: [sortAndFilterMixin, TabsSort],

    data() {
        return {
            pageData: '',
            sortVal: 'buy',
            showFilters: false,
            filterOptions: filters,
            sortOptions
        };
    },
    components: {ResultBook},
    watch: {
        pageData(val) {
            if (this.sortVal === 'buy') {
                this.filterOptions = filters;
            } else {
                this.filterOptions = {}
            }
        }
    },
    computed: {
        currentType() {
            return this.sortVal ? this.sortVal.charAt(0).toUpperCase() : ""
        },
        sort: () => 'price',
        filteredList: function () {
            let result = [];
            if (this.pageData.data) {
                if (this.filterSelection.length === 0) {
                    return this.pageData.data;
                } else {
                   return this.pageData.data.filter((item) => {
                            return [].concat(this.query.filter).includes(item.condition.toLowerCase())
                        }
                    );
                }
            }
            return result;
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
            this.$router.replace({query: {filter: to.query.filter}});
        } else {
            next();
        }
    },
    methods: {
        updateSort(val) {
            this.sortVal = val;
            this.$_changeTab(val);
        },
        onLoaded(event){
            console.log('!!!Image loaded', event.target.value)
        }
    },
    created() {
        // this.filter = all;
        this.UPDATE_LOADING(true);
        this.$store.dispatch("bookDetails", {
            pageName: "bookDetails",
            isbn13: this.params.id,
            type: "buy"
        }).then((data) => {
            this.pageData = data;
            this.UPDATE_LOADING(false);
        });
    },
}