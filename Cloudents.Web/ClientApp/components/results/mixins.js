import { page } from './../data'
import RadioList from './../helpers/radioList.vue';
import ResultItem from './ResultItem.vue'
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
const ResultFood = () => import('./ResultFood.vue')
const ResultBookPrice = () => import('./ResultBookPrice.vue');
let dataContent = {};
let itoom = [];
const sortAndFilterMixin = {
   
    data() {
        return {
            filter: ''
        }
    },

    components: { RadioList },

    computed: {
        isLoading: function () { return this.$store.getters.loading },
        page: function () { return page[this.name] }
    },
    props: {
        name: { type: String }, query: { type: Object }, filterOptions: { type: String }, sort: { type: String }, fetch: { type: String }, params: { type: Object }
    }
};
export const pageMixin =
    {
        mixins: [sortAndFilterMixin],
        beforeRouteUpdate(to, from, next) {
            // just use `this`
            this.$store.commit("UPDATE_LOADING", true);
            if (to.query.q && from.query.q && to.query.q === from.query.q) {
                this.$store.dispatch("fetchingData", { pageName: to.path.slice(1), queryParams: { ...to.query, ...to.params } })
                    .then((data) => {
                        dataContent = data;
                        console.log(data)
                        this.pageData = data;
                        itoom=data.data
                        console.log(dataContent);
                        this.filter = this.filterOptions
                        this.$store.commit("UPDATE_LOADING", false);
                    })
            }
            else {
                this.$store.dispatch("updateLuisAndFetch", to).then((data) => {
                    dataContent = data;
                    console.log(data)
                    this.pageData = data;
                    itoom = data.data
                    console.log(dataContent);
                    this.filter = this.filterOptions
                    this.$store.commit("UPDATE_LOADING", false);
                })
            }
            next();
        },
  
        data() {
            return {
                position: {},
                items: itoom
            }
        },

        computed: {
            pageData: {
                get() {
                    //this.items = dataContent.data;
                    return dataContent
                },
                set(val) {
                    //for simple filter
                    val ? this.items = val.data : '';
                }
            } ,
            term: function () { return this.$store.getters.term },
            dynamicHeader: function () { return this.pageData.title },
            isEmpty: function () { return this.pageData.data ? !this.pageData.data.length : true },
            subFilter: function () { return this.query[this.filterOptions]; },
            subFilters: function () {
                const list = this.pageData[this.filter];
                return list ? list.map(item => { return { id: item, name: item } }) : [];
            }
        },

        components: { ResultItem, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood },

        mounted: function () {
            if (this.name === 'food' && navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(position => {
                    this.position = position;
                });
            }
        },

        created() {
            console.log("created")
            this.$store.commit("UPDATE_LOADING", true);
            this.$store.dispatch("updateLuisAndFetch", this.$route).then((data) => {
                dataContent = data;
                console.log(data)
                this.pageData = data;
                itoom = data.data
                console.log(dataContent);
                this.filter = this.filterOptions
                this.$store.commit("UPDATE_LOADING", false);
            })
        },
        methods: {
            $_changeFilter(filter) {
                this.filter = filter;
                if (!this.subFilters.length) {
                    this.$router.push({ query: { ... this.query, filter } });
                }
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
export const detailsMixin = {
    mixins: [sortAndFilterMixin],
    created() {
        this.filter = 'all';
        this.$store.commit("UPDATE_LOADING", true);
        this.$store.dispatch("bookDetails", { pageName: this.name, params: this.params }).then((res) => {
            this.pageData = res
            this.$store.commit("UPDATE_LOADING", false);
        })
    },
    data() {
        return { pageData: ''}
    },
    components: { ResultBookPrice, ResultBook},
    computed: {
        filteredList: function () {
            return this.filter === 'all' ? this.pageData.data.sort((a, b) => b.price - a.price):this.pageData.data.filter(item => item.condition === this.filter);
        }
    }
}
