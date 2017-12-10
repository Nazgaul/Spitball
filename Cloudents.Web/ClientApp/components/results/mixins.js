﻿import ResultItem from './ResultItem.vue';
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
import SuggestCard from './suggestCard.vue'
const ResultFood = () => import('./ResultFood.vue');
const foodExtra = () => import('./foodExtra.vue');
import SortAndFilter from './SortAndFilter.vue'
import plusBtn from "../settings/svg/plus-button.svg";
import { mapActions, mapGetters, mapMutations } from 'vuex'
export const sortAndFilterMixin = {

    data() {
        return {
            filter: ''
        };
    },

    components: {SortAndFilter,plusBtn },

    props: {
        name: { type: String }, query: { type: Object }, filterSelection: { type: [String,Array] }, $_calcTerm:{type:Function},sort: { type: String }, page: { type: Object }, params: { type: Object }
    },

    methods: {
        ...mapMutations(['UPDATE_LOADING'])}
};
let updateData = function (data,isFilterUpdate=false) {
    let {source,jobType}=data;

    this.pageData = {};
    this.content = data;
    (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : '';
    this.filter = this.filterSelection;
    this.UPDATE_LOADING(false);

    if(!isFilterUpdate){
        this.$_updateFilterObject();
        if(source||jobType)this.$route.meta[`${this.name}Facet`]=source?source:jobType;
    }
};

export const pageMixin =
    {
        mixins: [sortAndFilterMixin],
        beforeRouteLeave (to, from, next) {
            if(to.name&&to.name==='home') {
                this.$route.meta.jobTerm = null;
                this.$route.meta.foodTerm = null;
                this.$route.meta.term = null;
                this.$route.meta.myClasses = [];
                this.$nextTick(() => {
                    next();
                })
            }else{
                next();
            }
        },

        beforeRouteUpdate(to, from, next) {

            this.UPDATE_LOADING(true);
            let toName=to.path.slice(1);
            let savedTerm=to.meta[this.$_calcTerm(toName)];
            this.pageData={};
            this.items=[];
            new Promise((resolve, reject) => {
                if(!to.query.q||!to.query.q.length){resolve()}
                else if(!savedTerm||(savedTerm.term!==to.query.q)){
                    this.updateSearchText(to.query.q).then((response)=> {
                        this.$route.meta[this.$_calcTerm(toName)] = {term: to.query.q, luisTerm: response.term};
                        resolve(to.meta[this.$_calcTerm(toName)].luisTerm);
                    })}else{
                    resolve(savedTerm.luisTerm);
                }
            }).then((luisTerm)=>{
                let updateFilter=(to.path===from.path&&to.query.q===from.query.q);
                this.fetchingData({name: toName, params: {...to.query, ...to.params}, luisTerm})
                    .then((data) => {
                        updateData.call(this, data,updateFilter);
                    });
                next();
            });
        },
        computed: {
            ...mapGetters(['term', 'isFirst','myCourses','luisTerm']),
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
                return this.name.includes('note')||this.name==='flashcard'||this.name==='job'
            }
        },

        data() {
            return {
                items: '',
                pageData: '',
                selectedItem:null,
                filterObject:{}
            };
        },

        components: { foodExtra, ResultItem,SuggestCard, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood },

        created() {
            if(this.query.course)this.$route.meta.myClasses=this.query.course;
            this.UPDATE_LOADING(true);
            if(!this.query.q||!this.query.q.length){
                this.fetchingData({name: this.name, params: {...this.query, ...this.params}})
                    .then((data) => {
                        updateData.call(this, data);
                    });
            }else {
                this.updateSearchText(this.query.q).then((response) => {
                    this.$route.meta[this.$_calcTerm(response.result)] = {term: this.query.q, luisTerm: response.term};
                    if (response.result !== this.name) {
                        this.UPDATE_LOADING(false);
                        let routeParams = {path: '/' + response.result, query: {...this.query, q: this.query.q}};
                        this.$router.replace(routeParams);
                    } else {
                        this.fetchingData({
                            name: this.name,
                            params: {...this.query, ...this.params},
                            luisTerm: response.term
                        })
                            .then((data) => {
                                updateData.call(this, data);
                            });
                    }
                });
            }

        },
        methods: {
            ...mapActions(['updateSearchText', 'fetchingData']),
            $_updateFilterObject(){
                if(!this.page.filter){ this.filterObject={}}
                else if(!this.subFilterVertical){
                    this.filterObject=[{title:'filter',modelId:"filter",data:this.page.filter}];
                }
                else{
                    this.filterObject=this.page.filter.map((i)=>{
                        let item={title:i.name,modelId:i.id};
                        item.data=(i.id==="course")?this.myCourses:this.pageData[i.id]?this.pageData[i.id]:this.getFacet;
                        return item;
                    });
                }
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
                if(id==='course'){
                    this.$route.meta.myClasses=listo;
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
                this.$root.$el.querySelector("#myCourses").click();
            },
            $_showSelectedFilter(item){
                if(!this.subFilterVertical)return this.page.filter.find(i=>i.id===item).name;
                return !Number.isNaN(item)&&this.myCourses.find(x=>x.id===Number(item))?this.myCourses.find(x=>x.id===Number(item)).name:item;
            }
        },
        props: { hasExtra: {type:Boolean},currentTerm:{type:[String,Object]},getFacet:{type:[Array]},currentSuggest:{type:String}}

    };
