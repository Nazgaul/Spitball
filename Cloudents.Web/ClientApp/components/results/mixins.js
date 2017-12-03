﻿import { page } from './../data'
 import RadioList from './../helpers/radioList.vue';
 import SortSwitch from './../helpers/sortSwitch.vue';
import ResultItem from './ResultItem.vue';
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultPersonalize = () => import('../settings/ResultPersonalize.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
import SuggestCard from './suggestCard.vue'
const ResultFood = () => import('./ResultFood.vue');
const foodExtra = () => import('./foodExtra.vue');
import AppMenu from './../navbar/TheNavbar.vue';
import { mapActions, mapGetters, mapMutations } from 'vuex'
export const sortAndFilterMixin = {
   
    data() {
        return {
            filter: ''
        };
    },

    components: { RadioList,SortSwitch,AppMenu },

    computed: {
        page: function () { return page[this.name] }
    },
    props: {
        name: { type: String }, query: { type: Object }, filterOptions: { type: String }, sort: { type: String }, fetch: { type: String }, params: { type: Object }
    },

    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    }
};
let updateData = function (data) {
    this.pageData={};
    this.content = data;
    (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : '';
    this.filter = this.filterOptions;
    this.UPDATE_LOADING(false);
    this.$nextTick(()=>{
    this.$el.querySelector(`#${this.name} a`).click();
    })
};

export const pageMixin =
    {
        mixins: [sortAndFilterMixin],
        beforeRouteEnter (to, from, next) {
            next((vm)=>{
                vm.$store.commit('ADD',{result:vm.$route.path.slice(1)});
            });
        },

        beforeRouteUpdate(to, from, next) {

            this.UPDATE_LOADING(true);
            let toName=to.path.slice(1);
            let savedTerm=to.meta[this.$_calcTerm(toName)];
                this.pageData={};
                this.items=[];
                new Promise((resolve, reject) => {
                    if(savedTerm.term!==to.query.q){
                        this.updateSearchText(to.query.q).then((response)=> {
                            this.$route.meta[this.$_calcTerm(toName)] = {term: to.query.q, luisTerm: response.term};
                            resolve(to.meta[this.$_calcTerm(toName)].luisTerm);
                        })}else{
                        resolve(savedTerm.luisTerm)
                    }
                    }).then((luisTerm)=>{
                    this.fetchingData({name: toName, params: {...to.query, ...to.params}, luisTerm})
                        .then((data) => {
                            updateData.call(this, data);
                        });
                    next();
                });
        },
        computed: {
            ...mapGetters(['term', 'isFirst','myCourses','flowNode','currenFlow','luisTerm']),
            content: {
                get() {
                    return this.pageData;
                },
                set(val) {
                    if (val) {
                        this.pageData = val;
                        this.items = val.data;
                        this.$nextTick(() => {
                            this.UPDATE_LOADING(false);
                        });
                    }
                }
            },
            showCourses() {
                return this.page && this.page.filter ?
                    new Set(this.page.filter.map((i) => i.id)).has('course') : false;
            },
            titleText: function () {if(!this.page)return '';
                if(this.isEmpty)return this.page.emptyText.replace('$subject', this.term);
                return this.page.title},
            isEmpty: function () { return this.pageData.data ? !this.pageData.data.length : true },
            subFilter: function () { return this.query[this.filterOptions]; },
            subFilterVertical(){
                return this.name.includes('note')||this.name==='flashcard'
            },
            subFilters: function () {
                if (this.filter === 'course') {
                    return [... this.myCourses, { id: 'addCourse', name: 'Select Course'}];
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
            };
        },

        components: { foodExtra, ResultItem,SuggestCard, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood,ResultPersonalize },

        created() {
            this.isfirst = this.isFirst;
            this.$nextTick(() => {
                if (this.isFirst) this.updateFirstTime();
            });
            this.UPDATE_LOADING(true);
                this.updateSearchText(this.query.q).then((response)=>{
                    this.$route.meta[this.$_calcTerm(response.result)]={term:this.query.q,luisTerm:response.term};
                        if(response.result!==this.name){
                        this.UPDATE_LOADING(false);
                        let routeParams={ path: '/'+response.result, query: {...this.query, q: this.query.q } };
                        this.$router.replace(routeParams);}else{
                        this.fetchingData({name: this.name, params: {...this.query, ...this.params},luisTerm:response.term})
                            .then((data) => {
                                updateData.call(this, data);
                            });
                    }});

        },
        methods: {
            //TODO update the food in theNavber to purchase also
            $_calcTerm(name){return (name.includes('food')||name.includes('purchase'))?'foodTerm':name.includes('job')?'jobTerm':'term'},
            ...mapActions(['updateSearchText', 'fetchingData','updateFirstTime','updateFlow']),
            $_changeFilter(filter) {
                if (this.subFilters.length) {
                    delete this.query[this.filter];
                }
                this.filter = filter;
                if (!this.subFilters.length) {
                    let currentQuery=this.query;
                    if(this.filter==='inPerson')currentQuery={... this.query,sort:"price"};
                    this.$router.push({ query: { ... currentQuery, filter } });
                }
            },
            $_updateCurrentFlow(index){
                this.updateFlow(index).then(()=>{
                    this.$router.push({path:'/'+this.currenFlow,query:{q:this.query.q}});
                });
            },
            $_defaultSort(defaultSort) {
                return this.query.sort ? this.query.sort : defaultSort;
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
                    });
                    return;
                }
                this.$router.push({ query: { ... this.query, ...sub, filter: this.filter } });
            }
        },
        props: { hasExtra: {type:Boolean},currentTerm:{type:[String,Object]}}

    };
