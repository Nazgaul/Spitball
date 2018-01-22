import ResultItem from './ResultItem.vue';
const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultJob = () => import('./ResultJob.vue');
import ResultVideo from './ResultVideo.vue'
import SuggestCard from './suggestCard.vue'
const ResultFood = () => import('./ResultFood.vue');
const foodExtra = () => import('./foodExtra.vue');
const SortAndFilter = () => import('./SortAndFilter.vue');
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
        filterSelection: { type: [String, Array] },
        sort: { type: String },
        params: { type: Object }
    },
    computed: {
        ...mapGetters(['loading']),
        page(){return page[this.name]}
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
    const { source, jobType } = data;

    this.pageData = {};
    this.content = data;
    (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : '';
    this.filter = this.filterSelection;
    this.UPDATE_LOADING(false);

    //if the vertical or search term has been changed update the optional filters according
    if (!isFilterUpdate) {
        this.$_updateFilterObject();
        if (source || jobType) this.$route.meta[`${this.name}Facet`] = source ? source : jobType;
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
            // this.getAIDataForVertical(toName).then((val)=>{savedTerm=val});
            //if the term for the page is as the page saved term use it else call to luis and update the saved term
            new Promise((resolve, reject) => {
                if (!to.query.q || !to.query.q.length) {
                    //update vertical data
                    this.updateSearchText({vertical:toName});
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
            ...mapGetters(['term', 'isFirst', 'myCourses']),
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
            currentSuggest(){return verticalsName.filter(i => i !== this.name)[(Math.floor(Math.random() * (verticalsName.length - 2)))]}
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

        components: { emptyState, foodExtra, ResultItem, SuggestCard, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood },

        created() {
            if (!this.isFirst) { this.showPersonalizeField = true }
            this.$root.$on("closePersonalize", () => { this.showPersonalizeField = true });
            //If query have courses save those courses
            if (this.query.course) this.setFileredCourses(this.query.course);
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
            ...mapActions(['updateSearchText', 'fetchingData','getAIDataForVertical','setFileredCourses','cleanData']),
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

                //check if filter selection have values that not exist in the current filter options make query replace without the filters
                let filterPotential = (this.filterObject || []).map(i => {if(i.data&&i.data.length&&i.data[0].id){
                                                                            return i.data.map(i=>i.id);
                                                                          } return i.data?i.data.map(c=>c.toString()):[]});
                filterPotential=filterPotential.join(',').split(',');
                if (Array.from(this.filterSelection).filter(i => filterPotential.includes(i.toString())).length !== this.filterSelection.length) {
                    const routeParams = { path: '/' + this.name, query: { q: this.userText } };
                    this.$router.replace(routeParams);
                }

            },
            //when sort updated update sort in params query
            $_updateSort(sort) {
                this.$router.push({ query: { ... this.query, sort: sort } });
            },
            //update page filters functionality(click on one of the filters)
            $_updateFilter({ id, val, type }) {
                //Take the filter list from query according the id(course,source,filter,jobType..)
                const currentFilter = !this.query[id] ? [] : Array.isArray(this.query[id]) ? this.query[id] : [this.query[id]];
                let updatedList = [val, ...currentFilter];
                //if the selected was uncheck remove this filter from the list
                if (!type.target.checked) {
                    updatedList = currentFilter.filter(i => i.toString() !== val.toString());
                }
                //If it course l;ist save it for next
                if (id === 'course') {
                    this.setFileredCourses(updatedList);
                }
                const newFilter = { [id]: updatedList };
                let { q, sort, course, source, filter,jobType } = this.query;
                if (val === 'inPerson' && type) sort = "price";
                //Combine current filters and the updated filter and call query
                this.$router.push({ query: { q, sort, course, source, jobType, filter, ...newFilter } });
            },
            //functionality when remove filter from the selected filters
            $_removeFilter(val) {
                //take all filters options from the query and remove the selected val from the correct option and make query with the updated filters
                let { source, course, filter, jobType } = this.query;
                let isCourseFiltered=course;
                source = source ? [].concat(source).filter(i => i !== val) : source;
                course = course ? [].concat(course).filter(i => i.toString() !== val.toString()) : course;
                filter = filter ? [].concat(filter).filter(i => i !== val) : filter;
                jobType = jobType ? [].concat(jobType).filter(i => i !== val) : jobType;
                isCourseFiltered?this.setFileredCourses(course):"";
                this.$router.push({ path: this.name, query: { ...this.query, source, course, filter, jobType } });
            },
            //Open the personalize dialog when click on select course in class filter
            $_openPersonalize() {
                this.$root.$emit("personalize", typesPersonalize.course);
            },
            //The presentation functionality for the selected filter(course=>take course name,known list=>take the terms from the const name,else=>the given name)
            $_showSelectedFilter(item) {
                if (this.page && !this.subFilterVertical) return this.page.filter.find(i => i.id === item).name;
                return !Number.isNaN(item) && this.myCourses.find(x => x.id === Number(item)) ? this.myCourses.find(x => x.id === Number(item)).name : item;
            }
        },
        //Page props come from the route
        props: {
            hasExtra: { type: Boolean },
            getFacet: { type: [Array] },
            userText: { type: String },
            isPromo:  {type:Boolean}
        }

    };
