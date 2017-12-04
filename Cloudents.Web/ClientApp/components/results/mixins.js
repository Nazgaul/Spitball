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
        name: { type: String }, query: { type: Object }, filterSelection: { type: [String,Array] }, sort: { type: String }, fetch: { type: String }, params: { type: Object }
    },

    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    }
};
let updateData = function (data) {
    this.pageData={};
    this.content = data;
    (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : '';
    this.filter = this.filterSelection;
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

            isEmpty: function () { return this.pageData.data ? !this.pageData.data.length : true },
            subFilterVertical(){
                return this.name.includes('note')||this.name==='flashcard'
            },
            filterObject(){
                if(!this.subFilterVertical&&this.page.filter){
                    return [{title:'filter',modelId:"filter",data:this.page.filter}];
                }
                else if(this.page.filter&&this.subFilterVertical){
                    return this.page.filter.map((i)=>{
                        let item={title:i.name,modelId:i.id};
                        item.data=this.pageData[i.id]?this.pageData[i.id]:this.myCourses;
                        return item;
                    })
                }
        }
        },

        data() {
            return {
                items: '',
                pageData: '',
                isfirst: false,
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
            $_changeSubFilter({id,val,type}) {
                let currentFilter=!this.query[id]?[]:Array.isArray(this.query[id])?this.query[id]:[this.query[id]];
                let listo=[val,...currentFilter];
                if(!type.target.checked){
                    listo=currentFilter.filter(i=>i!==val);
                }
                let newFilter={[id]:listo};
                let {q,sort,course,source,filter}=this.query;
                if(val==='inPerson'&&type)sort="price";
                this.$router.push({ query: {q,sort,course,source,filter, ...newFilter}});
            },
            $_removeFilter(val){
                let {source,course,filter}=this.query;
                source=source?[].concat(source).filter(i=>i!== val):source;
                course=course?[].concat(course).filter(i=>i!== val):course;
                filter=filter?[].concat(filter).filter(i=>i!== val):filter;
                this.$router.push({path:this.name,query:{...this.query,source,course,filter}});
            },
            $_openPersonalize(){
                this.$parent.$el.querySelector("#myCourses").click();
            },
            $_showSelectedFilter(item){
                console.log(!Number.isNaN(item));
                return !Number.isNaN(item)&&this.myCourses.find(x=>x.id===Number(item))?this.myCourses.find(x=>x.id===Number(item)).name:item;
                // this.myCourses.find(x=>x.id===item).name
            }
        },
        props: { hasExtra: {type:Boolean},currentTerm:{type:[String,Object]}}

    };
