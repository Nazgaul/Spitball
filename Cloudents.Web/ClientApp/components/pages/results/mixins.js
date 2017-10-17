import { page } from './../../data'
import RadioList from './../../helpers/radioList.vue';
import ResultItem from './ResultItem.vue'
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
const ResultFood = () => import('./ResultFood.vue')
const ResultBookPrice = () => import('./ResultBookPrice.vue');
const sortAndFilterMixin = {
    data() {
        this.$store.subscribe((mutation, state) => {
            if (mutation.type === "UPDATE_LOADING" && mutation.payload) {
                this.pageData = {};
                this.filter = 'all';
            } else if (mutation.type === "PAGE_LOADED") {
                this.pageData = mutation.payload;
                this.filter = this.filterOptions
            }
        });
        return {
            filter: '',
            items: '',
            pageData: {}
        }
    },
    components: { RadioList },

    computed: {
        page: function () { return page[this.name] },
        subFilter: function () { return this.currentQuery[this.filterOptions]; },
        subFilters: function () {
            const list = this.pageData[this.filter];
            return list ? list.map(item => { return { id: item, name: item } }) : [];
        }
    },
    props: {
        name: { type: String }, currentQuery: { type: Object }, filterOptions: { type: String }, sort: { type: String }
    },

    methods: {
        $_defaultSort(defaultSort) {
            let sort = this.currentQuery.sort ? this.currentQuery.sort : defaultSort;
            return sort;
        },
        $_updateSort(sort) {
            this.$router.push({ query: { ... this.currentQuery, sort: sort } });
        },
        $_changeFilter(filter) {
            delete this.currentQuery[this.filter];
            this.filter = filter;
            let query = this.currentQuery.sort ? { sort: this.currentQuery.sort } : {};
            if (!this.subFilters.length) {
                this.$router.push({ query: { ...query, filter } });
            }
        },
        $_changeSubFilter(val) {
            let sub = {};
            sub[this.filter] = val;
            this.$router.push({ query: { ... this.currentQuery, ...sub, filter: this.filter } });
        }
    }
};
export const pageMixin =
    {
        mixins:[sortAndFilterMixin],
        data() {
            return {
                position: {},
            }
        },

        computed: {
            term: function () { return this.$store.getters.term },
            dynamicHeader: function () { return this.pageData.title },
            isEmpty: function () { return this.pageData.data? !this.pageData.data.length : true }
        },

        components: { ResultItem, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood },

        mounted: function () {
            if (this.$route.name === 'food' && navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(position => {
                    this.position = position;
                });
            }
        }
    };
export const detailsMixin = {
    mixins:[sortAndFilterMixin],
    components: { ResultBookPrice },
    computed: {
        filteredList: function () {
            return this.filter === 'all' ? this.pageData.data:this.pageData.data.filter(item => item.condition === this.filter);
        }
    }
}
