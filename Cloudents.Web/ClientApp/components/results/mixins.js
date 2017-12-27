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
        $_calcTerm: { type: Function },
        sort: { type: String },
        page: { type: Object },
        params: { type: Object },
    },
    computed: {
        ...mapGetters(['loading'])
    },
    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    }
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
                this.$route.meta.jobTerm = null;
                this.$route.meta.foodTerm = null;
                this.$route.meta.term = null;
                this.$route.meta.myClasses = [];
                this.$nextTick(() => {
                    next();
                });
            } else {
                next();
            }
        },

        //When route has been updated(query,filter,vertical)
        beforeRouteUpdate(to, from, next) {
            this.UPDATE_LOADING(true);
            const toName = to.path.slice(1);
            const savedTerm = to.meta[this.$_calcTerm(toName)];
            this.pageData = {};
            this.items = [];
            //if the term for the page is as the page saved term use it else call to luis and update the saved term
            new Promise((resolve, reject) => {
                if (!to.query.q || !to.query.q.length) { resolve({ luisTerm: "" }) }
                else if (!savedTerm || (savedTerm.term !== to.query.q)) {
                    this.updateSearchText(to.query.q).then(({ term, docType }) => {
                        this.$route.meta[this.$_calcTerm(toName)] = { term: to.query.q, luisTerm: term, docType };
                        resolve({ luisTerm: to.meta[this.$_calcTerm(toName)].luisTerm, docType });
                    })
                } else {
                    resolve({ luisTerm: savedTerm.luisTerm, docType: savedTerm.docType });
                }
            }).then(({ luisTerm, docType }) => {
                //After luis return term and optional docType fetch the data
                const updateFilter = (to.path === from.path && to.query.q === from.query.q);
                this.fetchingData({ name: toName, params: { ...to.query, ...to.params }, luisTerm, docType })
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
                const courseIndex = this.filterObject.findIndex(i => i.modelId === "course");
                if (courseIndex > -1)
                    this.filterObject[courseIndex].data = val;
            }
        },
        computed: {
            //get data from vuex getters
            ...mapGetters(['term', 'isFirst', 'myCourses', 'luisTerm',]),
            ...mapGetters({ universityImage: 'getUniversityImage', university: 'getUniversity' }),
            //isMobile() {
            //    return this.$vuetify.breakpoint.xsOnly;
            //},
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
            subFilterVertical() {
                return this.name.includes('note') || this.name === 'flashcard' || this.name === 'job';
            }
        },

        data() {
            return {
                items: '',
                pageData: '',
                selectedItem: null,
                filterObject: null,
                showFilters: false
            };
        },

        components: { foodExtra, ResultItem, SuggestCard, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood  },

        //If change term on book details page stay in book vertical(don't update vertical) according vertical flag
        beforeRouteEnter(to, from, next) {
            new Promise(resolve => {
                if (from.name && from.name === "bookDetails") {
                    to.meta.vertical = true;
                    resolve();
                } else {
                    resolve();
                }
            }).then(() => next());

        },
        created() {
            //If query have courses save those courses
            if (this.query.course) this.$route.meta.myClasses = this.query.course;
            this.UPDATE_LOADING(true);
            //if the term is empty fetch the data
            if (!this.query.q || !this.query.q.length) {
                this.fetchingData({ name: this.name, params: { ...this.query, ...this.params } })
                    .then(({ data }) => {
                        updateData.call(this, data);
                    }).catch(reason => { this.UPDATE_LOADING(false); });
            } else {
                //call luis with the userText
                this.updateSearchText(this.userText).then(({ term, result, docType }) => {
                    //save the data in the appropriate meta according the box methodology
                    this.$route.meta[this.$_calcTerm(result)] = { term: this.userText, luisTerm: term, docType };
                    //If should update vertical(not book details) and luis return not identical vertical as current vertical replace to luis vertical page
                    if (!this.vertical && result !== this.name) {
                        this.UPDATE_LOADING(false);
                        const routeParams = { path: '/' + result, query: { ...this.query, q: this.userText } };
                        this.$router.replace(routeParams);
                    } else {
                        //fetch data with the params
                        this.fetchingData({
                            name: this.name,
                            params: { ...this.query, ...this.params },
                            luisTerm: term, docType
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
            ...mapActions(['updateSearchText', 'fetchingData']),
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
                    this.$route.meta.myClasses = updatedList;
                }
                const newFilter = { [id]: updatedList };
                let { q, sort, course, source, filter } = this.query;
                if (val === 'inPerson' && type) sort = "price";
                //Combine current filters and the updated filter and call query
                this.$router.push({ query: { q, sort, course, source, filter, ...newFilter } });
            },
            //functionality when remove filter from the selected filters
            $_removeFilter(val) {
                //take all filters options from the query and remove the selected val from the correct option and make query with the updated filters
                let { source, course, filter, jobType } = this.query;
                source = source ? [].concat(source).filter(i => i !== val) : source;
                course = course ? [].concat(course).filter(i => i.toString() !== val.toString()) : course;
                filter = filter ? [].concat(filter).filter(i => i !== val) : filter;
                jobType = jobType ? [].concat(jobType).filter(i => i !== val) : jobType;
                this.$route.meta.myClasses = course;
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
            currentTerm: { type: [String, Object] },
            getFacet: { type: [Array] },
            currentSuggest: { type: String },
            vertical: {},
            userText: { type: String }
        }

    };
