import ResultItem from './ResultItem.vue';
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
import ResultVideoSkeleton from './ResultVideoSkeleton.vue'
import SuggestCard from './suggestCard.vue'
import studyblueCard from './studyblueCard.vue'
import emptyState from "./svg/no-match-icon.svg";
import {verticalsName} from '../../data'
import { typesPersonalize } from "../settings/consts.js";
import { mapActions, mapGetters } from 'vuex'
const ACADEMIC_VERTICALS=['note','ask','flashcard','book','tutor'];
import sortAndFilterMixin from '../mixins/sortAndFilterMixin'

import {skeletonData,promotions} from './consts'
//update data function update the page content and selected filters
let updateData = function (data, isFilterUpdate = false) {
    const { facet } = data;
    facet?this.updateFacet(facet):'';
    this.pageData = {};
    this.content = data;
    this.$emit('dataUpdated', data.data.length ? data.data[0] : null)
    // (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : '';
    this.filter = this.filterSelection;
    this.UPDATE_LOADING(false);
    (this.isLoad)?this.isLoad=false:this.UPDATE_LOADING(false);


    //if the vertical or search term has been changed update the optional filters according
    if (!isFilterUpdate) {
        this.$_updateFilterObject();
    }
};
//The vue functionality for result page
export const pageMixin =
    {
        //use basic sort and filter functionality( same for book details and result page)
        mixins: [sortAndFilterMixin],
        //when go back to home clear the saved term and classes
        beforeRouteLeave(to, from, next) {
            this.leavePage(to,from,next);
        },

        //When route has been updated(query,filter,vertical)
        beforeRouteUpdate(to, from, next) {
            this.updatePageData(to,from,next)
        },

        watch: {
            //update the course list of filters if have in page while the course list changes
            myCourses(val) {
                if (this.filterObject) {
                    const courseIndex = this.filterObject.findIndex(i => i.modelId === "course");
                    if (courseIndex > -1)
                        this.filterObject[courseIndex].data = val;
                }
            }
        },
        computed: {
            //get data from vuex getters
            ...mapGetters(['isFirst', 'myCourses','getFacet','getVerticalData']),
            ...mapGetters({ universityImage: 'getUniversityImage', university: 'getUniversity' }),
            currentPromotion(){return promotions[this.name]},
            content: {
                get() {
                    return this.pageData;
                },
                set(val) {
                    if (val) {
                        this.pageData = val;
                        this.items = val.data;
                        this.$nextTick(() => {
                            if(!this.items.length){
                                Promise.resolve(()=>{
                                    let filters={};
                                    Object.entries(this.query).forEach(([key, currentVal]) => {
                                        if(key!=="sort"&&key!=="q"&&currentVal){
                                            filters[key]=currentVal;
                                        }
                                    });
                                    return filters;
                                }).then(filters=>{
                                    let myFilters=filters();
                                    let ExtraContent="";
                                    if(myFilters&&Object.keys(myFilters).length){
                                        ExtraContent="#";
                                        Object.entries(myFilters).forEach(([key, currentVal]) => {
                                            ExtraContent+=`${key}:[${currentVal}]`;
                                        });
                                        ExtraContent+="#";
                                    }
                                    this.$ga.event("Empty_State",this.name,ExtraContent+this.userText);
                                });
                            }
                            this.UPDATE_LOADING(false);
                        });
                    }
                }
            },
            isEmpty: function () { return this.pageData.data ? !this.pageData.data.length : true },
            subFilterVertical() {
                return this.name.includes('note') || this.name === 'flashcard' || this.name === 'job'||this.name.includes('ask');
            },
            currentSuggest(){return verticalsName.filter(i => i !== this.name)[(Math.floor(Math.random() * (verticalsName.length - 2)))]},
            userText(){return this.query.q},
            isAcademic(){return ACADEMIC_VERTICALS.includes(this.name)},
            showSkelaton(){return this.loading||this.isLoad}
        },

        data() {
            return {
                items: '',
                pageData: '',
                selectedItem: null,
                filterObject: null,
                showFilters: false,
                showPersonalizeField: false,
                showPromo:this.isPromo,
                isLoad:false
            };
        },

        components: { emptyState, ResultItem, SuggestCard, studyblueCard, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultVideoSkeleton },

        created() {
            if (this.isAcademic&&!this.isFirst) { this.showPersonalizeField = true }
            this.$root.$on("closePersonalize", () => { this.showPersonalizeField = true });
            //If query have courses save those courses
            if (this.query.course) this.setFilteredCourses(this.query.course);
            this.UPDATE_LOADING(true);
            this.items=skeletonData[this.name];
            let vertical=this.name === "result"?"":this.name;
            //call luis with the userText
            this.updateSearchText({text:this.userText,vertical}).then(({ term, result}) => {
                //If should update vertical(not book details) and luis return not identical vertical as current vertical replace to luis vertical page
                if (this.name === "result") { //from homepage
                    this.UPDATE_LOADING(false);
                    const routeParams = { path: '/' + result, query: { ...this.query, q: this.userText } };
                    this.$router.replace(routeParams);
                }
                else {
                    //fetch data with the params
                    this.fetchingData({
                        name: this.name,
                        params: { ...this.query, ...this.params }

                    })
                        .then(({ data }) => {
                            updateData.call(this, data);//irena
                            }).catch(reason => {
                            //when error from fetching data remove the loader
                            this.UPDATE_LOADING(false);
                            this.items=[];
                        });
                }
            });
        },
        methods: {
            updatePageData(to,from,next){
                (to.path===from.path&&to.q===from.q)?this.isLoad=true:this.UPDATE_LOADING(true);
                const toName = to.path.slice(1);
                this.pageData = {};
                this.items = [];
                this.items = skeletonData[toName];
                //if the term for the page is as the page saved term use it else call to luis and update the saved term
                this.updateSearchText({text: to.query.q, vertical: toName}).then(() => {
                    const updateFilter = (to.path === from.path && to.query.q === from.query.q);
                    this.fetchingData({ name: toName, params: { ...to.query, ...to.params }})
                        .then(({ data }) => {
                            //update data for this page
                            updateData.call(this, data, updateFilter);
                        }).catch(reason => {
                        //when error from fetching data remove the loader
                        (to.path===from.path&&to.q===from.q)?this.isLoad=false:this.UPDATE_LOADING(false);
                        this.items=[];
                    });
                    //go to the next page
                    next();
                })
            },
            leavePage(to,from,next){
                if (to.name && to.name === 'home') {
                    this.cleanData();
                }
                next();
            },
            //Get functions from vuex actions
            ...mapActions(['updateSearchText', 'fetchingData','getAIDataForVertical','setFilteredCourses','cleanData','updateFacet']),
            //Function for update the filter object(when term or vertical change)
            $_updateFilterObject() {
                //validate current page have filters
                if (!this.page || !this.page.filter) { this.filterObject = null }
                else if (!this.subFilterVertical) {
                    this.filterObject = [{ title: 'Status', modelId: "filter", data: this.page.filter }];
                }
                else {
                    //create filter object as the above structure but from list while the data is computed according to the filter id
                    this.filterObject = this.page.filter.map((i) => {
                        const item = { title: i.name, modelId: i.id };
                        item.data = (i.id === "course") ? this.myCourses : this.pageData[i.id] ? this.pageData[i.id] : this.getFacet ? this.getFacet : [];
                        return item;
                    });
                }

                let matchValues=this.filterSelection.filter(i=>this.filterObject.find(t=>
                    (t.modelId===i.key&&
                        (t.data.find(
                                k=>i.value.toString()===(k.id?k.id.toString():k.toString()))
                        ))
                ));
                if(matchValues.length!==this.filterSelection.length){
                    const routeParams = { path: '/' + this.name, query: { q: this.userText } };
                    this.$router.replace(routeParams);
                }
            },

            //functionality when remove filter from the selected filters
            $_removeFilter({value,key}) {
                let updatedList=this.query[key];
                updatedList = [].concat(updatedList).filter(i => i.toString() !== value.toString());
                key==='course'?this.setFilteredCourses(updatedList):"";
                this.$router.push({ path: this.name, query: {...this.query,[key]:updatedList} });
            },
            //Open the personalize dialog when click on select course in class filter
            $_openPersonalize() {
                this.$root.$emit("personalize", typesPersonalize.course);
            },
            //The presentation functionality for the selected filter(course=>take course name,known list=>take the terms from the const name,else=>the given name)
            $_showSelectedFilter({value,key}) {
                if (this.page && !this.subFilterVertical) return this.page.filter.find(i => i.id === value).name;
                return key==='course' && this.myCourses.find(x => x.id === Number(value)) ? this.myCourses.find(x => x.id === Number(value)).name : value;
            }
        },
        //Page props come from the route
        props: {
            isPromo:  {type:Boolean}
        }
    };
