import { page } from './../../data'

const RadioList = () => import('./../../helpers/radioList.vue');
export const pageMixin =
    {
        data() {
            return {
                page: page[this.$route.name],
                sort: this.$route.query.sort ? this.$route.query.sort:'relevance',
                currentQuery: this.$route.query,
                filter: ''
            }
        },

        computed: {
            filterOptions: function () {
                let fil = Object.keys(this.$route.query).filter(item => item !== 'sort').toString();
                return this.currentQuery.filter ? this.currentQuery.filter : (fil.length ? fil : 'all')
            },
            subFilter: function () { return this.currentQuery[this.filter]; },
            dynamicHeader: function () { return this.$store.getters.pageTitle },
            isEmpty: function () { return this.$store.getters.isEmpty },
            pageData: function () { return this.$store.getters.pageContent },
            subFilters: function () {
                var list = this.pageData[this.filter];
                return list ? list.map(item => { return { id: item, name: item } }) : [];
            }
        },

        created: function () {
            this.filter = this.filterOptions;
        },

        components: { RadioList },

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