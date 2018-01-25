import ResultItem from './ResultItem.vue';
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
import SuggestCard from './suggestCard.vue'
import studyblueCard from './studyblueCard.vue'
const ResultFood = () => import('./ResultFood.vue');
const foodExtra = () => import('./foodExtra.vue');
const SortAndFilter = () => import('../SortAndFilter/SortAndFilter.vue');
const MobileSortAndFilter = () => import('./MobileSortAndFilter.vue');
import plusBtn from "../settings/svg/plus-button.svg";
import emptyState from "./svg/no-match-icon.svg";
import {page, verticalsName} from '../../data'
import { typesPersonalize } from "../settings/consts.js";
import { mapActions, mapGetters, mapMutations } from 'vuex'
export const sortAndFilterMixin = {
    data() {
        return {
            filter: ''
        };
    },
    components: { SortAndFilter, plusBtn, MobileSortAndFilter },

    props: {
        name: { type: String },
        query: { type: Object },
        params: { type: Object }
    },
    computed: {
        ...mapGetters(['loading']),
        page(){return page[this.name]},
        sort(){return this.query.sort},
        filterSelection(){
            let filterOptions = [];
            let filtersList=['jobType','source','course','filter'];
            Object.entries(this.query).forEach(([key, val])=>{
                if(val&&val.length&&filtersList.includes(key)) {
                    [].concat(val).forEach(value=>filterOptions=filterOptions.concat({key,value}));
                }
            });
            return filterOptions;
        }
    },
    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    }
};
let promotions={note:{title:"Study Documents",content:"Spitball curates study documents from the best sites on the web. Our notes, study guides and exams populate based on student ratings and are filtered by your school, classes and preferences."},
    flashcard:{title:"Flashcard",content:"Search millions of study sets and improve your grades by studying with flashcards."},
    ask:{title:"Ask A Question",content:"Ask any school related question and immediately get answers and information that relates specifically to you, your classes, and your university."},
    tutor:{title:"Tutors",content:"Spitball has teamed up with the most trusted tutoring services to help you ace your classes."},
    book:{title:"Textbooks",content:"Find the best prices to buy, rent and sell your textbooks by comparing hundreds of sites simultaneously."},
    job:{title:"Jobs",content:"Easily search and apply to paid internships, part-time jobs and entry-level opportunities from local businesses all the way to Fortune 500 companies."},
    food:{title:"Food and Deals",content:"Discover exclusive deals to local businesses, restaurants and bars near campus."}
};
//update data function update the page content and selected filters
let updateData = function (data, isFilterUpdate = false) {
    const { facet } = data;
    facet?this.updateFacet(facet):'';
    this.pageData = {};
    this.content = data;
    (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : '';
    this.filter = this.filterSelection;
    this.UPDATE_LOADING(false);

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
            if (to.name && to.name === 'home') {
                this.cleanData();
            }
                next();

        },

        //When route has been updated(query,filter,vertical)
        beforeRouteUpdate(to, from, next) {
            this.UPDATE_LOADING(true);
            const toName = to.path.slice(1);
            this.pageData = {};
            this.items = [];
            //if the term for the page is as the page saved term use it else call to luis and update the saved term
            new Promise((resolve, reject) => {
                if (!to.query.q || !to.query.q.length) {
                    resolve();
                }else {
                    this.getAIDataForVertical(toName).then(({text=""}) => {
                        if (!text || (text !== to.query.q)) {
                            this.updateSearchText({text: to.query.q, vertical: toName}).then(() => {
                                resolve();
                            })
                        }else{resolve()}
                    });
                }

            }).then(() => {
                //After luis return term and optional docType fetch the data
                const updateFilter = (to.path === from.path && to.query.q === from.query.q);
                this.fetchingData({ name: toName, params: { ...to.query, ...to.params }})
                    .then(({ data }) => {
                        //update data for this page
                        updateData.call(this, data, updateFilter);
                    }).catch(reason => {
                        //when error from fetching data remove the loader
                        this.UPDATE_LOADING(false);
                    });
                //go to the next page
                next();
            });
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
            ...mapGetters(['term', 'isFirst', 'myCourses','getFacet']),
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
            userText(){return this.query.q}
        },

        data() {
            return {
                items: '',
                pageData: '',
                selectedItem: null,
                filterObject: null,
                showFilters: false,
                showPersonalizeField: false,
                showPromo:this.isPromo
            };
        },

        components: { emptyState, foodExtra, ResultItem, SuggestCard, studyblueCard, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood },

        created() {
            if (!this.isFirst) { this.showPersonalizeField = true }
            this.$root.$on("closePersonalize", () => { this.showPersonalizeField = true });
            //If query have courses save those courses
            if (this.query.course) this.setFilteredCourses(this.query.course);
            this.UPDATE_LOADING(true);
            //if the term is empty fetch the data
            if (!this.query.q || !this.query.q.length) {
                this.fetchingData({ name: this.name, params: { ...this.query, ...this.params } })
                    .then(({ data }) => {
                        updateData.call(this, data);
                    }).catch(reason => { this.UPDATE_LOADING(false); });
            } else {
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
                                updateData.call(this, data);
                            });
                    }
                });
            }

        },
        methods: {
            //Get functions from vuex actions
            ...mapActions(['updateSearchText', 'fetchingData','getAIDataForVertical','setFilteredCourses','cleanData','updateFacet']),
            //Function for update the filter object(when term or vertical change)
            $_updateFilterObject() {
                //validate current page have filters
                if (!this.page || !this.page.filter) { this.filterObject = null }
                else if (!this.subFilterVertical) {
                    this.filterObject = [{ title: 'filter', modelId: "filter", data: this.page.filter }];
                }
                else {
                    //create filter object as the above structure but from list while the data is computed according to the filter id
                    this.filterObject = this.page.filter.map((i) => {
                        const item = { title: i.name, modelId: i.id };
                        item.data = (i.id === "course") ? this.myCourses : this.pageData[i.id] ? this.pageData[i.id] : this.getFacet ? this.getFacet : [];
                        if (i.id === "filter" && this.name === "job") item.data = [{ id: "Paid", name: "Paid" }];
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
            hasExtra: { type: Boolean },
            isPromo:  {type:Boolean}
        }

    };
