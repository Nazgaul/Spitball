﻿import { page } from './../../data'
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
                
                sort: this.$route.query.sort ? this.$route.query.sort:'relevance',
                filter: '',
                items: '',
                position: {},
                pageData: {}
            }
        },

        computed: {
            currentQuery: function () { return this.$route.query },
            page: function() {return page[this.$route.name] },
            filterOptions: function () {
                return  (this.currentQuery.filter ? this.currentQuery.filter : 'all')
            },
            subFilter: function () { return this.currentQuery[this.filterOptions]; },
            dynamicHeader: function () { return this.pageData.title },
            isEmpty: function () { return this.pageData.data ? !this.pageData.data.length:true},
            subFilters: function () {
                var list = this.pageData[this.filter];
                return list ? list.map(item => { return { id: item, name: item } }) : [];
            }
        },

        props: { userText: { type: String } },

        components: { RadioList, ResultItem, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood },

        mounted: function () {
            if (this.$route.name==='food'&&navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(position => {
                    this.position = position;
                })
            }
        },
        created: function () {
            this.filter = this.filterOptions;
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
                sub[this.filter] = val
                this.$router.push({ query: { ... this.currentQuery, ...sub,filter:this.filter } });
                console.log('change sub filter');
            }
        }
    };
export const itemsList = {
    computed: {
        items: function () { return this.$store.getters.items }
    }
}