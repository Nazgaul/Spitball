import { page } from './../data'
import RadioList from './../helpers/radioList.vue';
const ResultItem = () => import('./ResultItem.vue');
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultPersonalize = () => import('../settings/ResultPersonalize.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
import SuggestCard from './suggestCard.vue'
const ResultFood = () => import('./ResultFood.vue')
const ResultBookPrice = () => import('./ResultBookPrice.vue');
const foodExtra = () => import('./foodExtra.vue');
import { mapActions, mapGetters, mapMutations } from 'vuex'
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
    },

    methods: {
        ...mapMutations(['UPDATE_LOADING']),
    }
};
var updateData = function (data) {
    this.content = data;
    (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : ''
    this.filter = this.filterOptions
    this.$nextTick(() => {
        this.UPDATE_LOADING(false);
    })
}
var updateLuis = function (page) {
    let self = this;
    this.updateLuisAndFetch(page).then((data) => {
        updateData.call(self,data)
    }).catch((page)=>{
        alert(`we still don\'t have this page ${page}`);
        this.content = {};
        this.pageData = {};
        this.items=[];
        this.filter =null;
        this.UPDATE_LOADING(false);
    })
}

export const pageMixin =
    {
        mixins: [sortAndFilterMixin],
        beforeRouteEnter (to, from, next) {
            next((vm)=>{
                vm.$store.commit('ADD',{result:vm.$route.path.slice(1)})
            })
        },

        beforeRouteUpdate(to, from, next) {
            
            // just use `this`
            if (to.path == from.path) {

                this.UPDATE_LOADING(true);
                if (to.query.q && from.query.q && to.query.q === from.query.q) {
                    this.fetchingData({ pageName: to.path.slice(1), queryParams: { ...to.query, ...to.params } })
                        .then((data) => {
                            updateData.call(this, data)
                        })
                }
                else {
                    updateLuis.call(this, to);
                }
            }
            next();
        },
        computed: {
            ...mapGetters(['term', 'isFirst','myCourses','flowNode','currenFlow']),
            content: {
                get() {
                    return this.pageData
                },
                set(val) {
                    //for simple filter
                    if (val) {
                        this.pageData = val;
                        this.items = val.data;
                    }
                }
            },
            showCourses() { return this.page.filter?new Set(this.page.filter.map((i)=>i.id)).has('course'):false},
            titleText: function () { return (this.name != 'ask')?this.page.title : this.dynamicHeader ? this.page.title.short :  this.page.title.normal},
            dynamicHeader: function () { return this.pageData.title },
            isEmpty: function () { return this.pageData.data ? !this.pageData.data.length : true },
            subFilter: function () { return this.query[this.filterOptions]; },
            subFilters: function () {
                if (this.filter === 'course') {
                    return [... this.myCourses, { id: 'addCourse', name: 'Select Course'}]
                }
                const list = this.pageData[this.filter];
                return list ? list.map(item => { return { id: item, name: item } }) : [];
            }
        },

        data() {
            return {
                items: '',
                pageData: '',
                isfirst: false,
                showSearch: false,
                selectedItem:null
            }
        },

        components: { foodExtra, ResultItem,SuggestCard, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood,ResultPersonalize },

        created() {
            this.isfirst = this.isFirst;
            this.$nextTick(() => {
                if (this.isFirst) this.updateFirstTime();
            })
            this.UPDATE_LOADING(true);
            updateLuis.call(this,this.$route)
        },
        methods: {
            ...mapActions(['updateLuisAndFetch', 'fetchingData','updateFirstTime','updateFlow']),
            $_changeFilter(filter) {
                if (this.subFilters.length) {
                    delete this.query[this.filter]
                }
                this.filter = filter;
                if (!this.subFilters.length) {
                    this.$router.push({ query: { ... this.query, filter } });
                }
            },
            $_showSuggest(index) {
                return (this.flowNode && this.flowNode.children &&
                    ((this.items.length < 7 && index == (this.items.length - 1)) ||
                    (this.items.length >= 7&&index == 6)));
            },
            $_updateCurrentFlow(index){
                this.updateFlow(index).then(()=>{
                    this.$router.push({path:'/'+this.currenFlow,query:{q:this.query.q}})
                })
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
                if (val === 'addCourse') {
                    this.showSearch = false;
                    this.$nextTick(() => {
                        this.showSearch = true;
                    })
                    return;
                }
                this.$router.push({ query: { ... this.query, ...sub, filter: this.filter } });
            }
        },
        props: { hasExtra: {type:Boolean}}

    };
export const detailsMixin = {
    mixins: [sortAndFilterMixin],
    created() {
        this.filter = 'all';
        this.UPDATE_LOADING(true)
        this.$store.dispatch("bookDetails", { pageName: this.name, params: this.params }).then((res) => {
            this.pageData = res
            this.UPDATE_LOADING(false)
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
