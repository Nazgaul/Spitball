import { page } from './../../data'
import ResultItem from './ResultItem.vue'
import ResultTutor from './ResultTutor.vue'
import ResultBook from './ResultBook.vue'
import ResultJob from './ResultJob.vue'
import ResultVideo from './ResultVideo.vue'
import ResultFood from './ResultFood.vue'
const RadioList = () => import('./../../helpers/radioList.vue');
export const pageMixin =
    {
        data() {
            console.log('data')
            this.$store.subscribe((mutation, state) => {
                if (mutation.type === "UPDATE_LOADING" && mutation.payload) {
                    this.pageData = {};
                }
                else if (mutation.type === "PAGE_LOADED") {
                    this.pageData = mutation.payload;
                }
            })
            return {
                page: page[this.$route.name],
                sort: this.$route.query.sort ? this.$route.query.sort:'relevance',
                currentQuery: this.$route.query,
                filter: '',
                items: '',
                position: {},
                pageData: {}
            }
        },

        computed: {
            filterOptions: function () {
                let fil = Object.keys(this.$route.query).filter(item => (item !== 'sort' && item !== 'filter')).toString();
                return fil.length ? fil : (this.currentQuery.filter ? this.currentQuery.filter : 'all')
            },
            subFilter: function () { return this.currentQuery[this.filter]; },
            dynamicHeader: function () { return this.pageData.title },
            isEmpty: function () { return this.pageData.isEmpty||false},
            subFilters: function () {
                var list = this.pageData[this.filter];
                return list ? list.map(item => { return { id: item, name: item } }) : [];
            }
        },

        created: function () {
            this.filter = this.filterOptions;
        },

        components: { RadioList, ResultItem, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood },

        mounted: function () {
            if (this.$route.name==='food'&&navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(position => {
                    this.position = position;
                })
            }
        },
        methods: {
            $_updateSort(sort) {
                this.$router.push({ query: { ... this.currentQuery, sort: this.sort } });
            },
            $_changeFilter(filter) {
                console.log('filter')
                delete this.currentQuery[this.filter];
                this.filter = filter;
                let query = this.currentQuery.sort ? { sort: this.currentQuery.sort } : {};
                if (!this.subFilters.length) {
                    //TODO think on better way to detect all
                    this.$router.push({ query: { ...query, filter} });
                }
            },
            $_changeSubFilter(val) {
                let sub = {};
                sub[this.filter] = val
                this.$router.push({ query: { ... this.currentQuery, ...sub } });
                console.log('change sub filter');
            }
        }
    };
export const itemsList = {
    computed: {
        items: function () { return this.$store.getters.items }
    }
}