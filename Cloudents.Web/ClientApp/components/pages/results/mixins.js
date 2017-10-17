import { page } from './../../data'
import ResultItem from './ResultItem.vue'
import ResultTutor from './ResultTutor.vue'
import ResultBook from './ResultBook.vue'
import ResultJob from './ResultJob.vue'
import ResultVideo from './ResultVideo.vue'
import ResultFood from './ResultFood.vue'
import ResultBookPrice from './ResultBookPrice.vue'
const RadioList = () => import('./../../helpers/radioList.vue');

export const pageMixin =
    {
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
                position: {},
                pageData: {}
            }
        },

        computed: {
            page: function() {return page[this.name] },
            subFilter: function () { return this.currentQuery[this.filterOptions]; },
            dynamicHeader: function () { return this.pageData.title },
            isEmpty: function () { return this.pageData.data ? !this.pageData.data.length:true},
            subFilters: function () {
                const list = this.pageData[this.filter];
                return list ? list.map(item => { return { id: item, name: item } }) : [];
            }
        },

        props: {
            name: { type: String }, currentQuery: { type: Object }, filterOptions: { type: String }, sort: {type:String}
        },

        components: { RadioList, ResultItem, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood, ResultBookPrice },

        mounted: function () {
            if (this.$route.name==='food'&&navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(position => {
                    this.position = position;
                });
            }
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
                    this.$router.push({ query: { ...query, filter} });
                }
            },
            $_changeSubFilter(val) {
                let sub = {};
                sub[this.filter] = val;
                this.$router.push({ query: { ... this.currentQuery, ...sub,filter:this.filter } });
                console.log('change sub filter');
            }
        }
    };