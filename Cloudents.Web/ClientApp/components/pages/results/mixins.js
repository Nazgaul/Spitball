import { page } from './../../data'
import RadioList from './../../helpers/radioList.vue';
import ResultItem from './ResultItem.vue'
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
const ResultFood = () => import('./ResultFood.vue')
const ResultBookPrice = () => import('./ResultBookPrice.vue');
const bobo = (obj) => {
    obj.$store.commit("UPDATE_LOADING", true);
    obj.$store.dispatch("fetchingData", { pageName: obj.name, queryParams: { ...obj.query, ...obj.params } })
        .then((data) => {
            obj.pageData = data;
            obj.filter = obj.filterOptions
            obj.$store.commit("UPDATE_LOADING", false);
        })
}
const sortAndFilterMixin = {
    beforeRouteUpdate(to, from, next) {
        // just use `this`
        this.$store.commit("UPDATE_LOADING", true);
        this.$store.dispatch("fetchingData", { pageName: to.name, queryParams: { ...to.query, ...to.params } })
            .then((data) => {
                this.pageData = data;
                this.filter = this.filterOptions
                this.$store.commit("UPDATE_LOADING", false);
            })
        next();
    },
    watch: {
        '$route': '$_routeChange'
    },
    data() {
        bobo(this);
        return {
            filter: '',
            pageData: ''
        }
    },

    components: { RadioList },

    computed: {
        isLoading: function () { return this.$store.getters.loading },
        page: function () { return page[this.name] },
        subFilter: function () { return this.query[this.filterOptions]; },
        subFilters: function () {
            const list = this.pageData[this.filter];
            return list ? list.map(item => { return { id: item, name: item } }) : [];
        }
    },
    props: {
        name: { type: String }, query: { type: Object }, filterOptions: { type: String }, sort: { type: String }, fetch: { type: String }, params: { type: Object }
    },

    methods: {
        $_routeChange(current, prev) {
            if(current.name!==prev.name)bobo(this);
        },
        $_defaultSort(defaultSort) {
            let sort = this.query.sort ? this.query.sort : defaultSort;
            return sort;
        },
        $_updateSort(sort) {
            this.$router.push({ query: { ... this.query, sort: sort } });
        },
        $_changeSubFilter(val) {
            let sub = {};
            sub[this.filter] = val;
            this.$router.push({ query: { ... this.query, ...sub, filter: this.filter } });
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
        },

        methods: {
            $_changeFilter(filter) {
                this.filter = filter;
                if (!this.subFilters.length) {
                    this.$router.push({ query: { ... this.query, filter } });
                }
            }
        }

    };
export const detailsMixin = {
    mixins:[sortAndFilterMixin],
    components: { ResultBookPrice, ResultBook},
    computed: {
        filteredList: function () {
            return this.filter === 'all' ? this.pageData.data:this.pageData.data.filter(item => item.condition === this.filter);
        }
    }
}
