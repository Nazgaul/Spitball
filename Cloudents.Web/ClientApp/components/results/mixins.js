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

    components: { SortAndFilter, plusBtn },

    props: {
        name: { type: String }, query: { type: Object }, filterSelection: { type: [String, Array] }, $_calcTerm: { type: Function }, sort: { type: String }, page: { type: Object }, params: { type: Object }
    },
    computed: {
        ...mapGetters(['loading'])
    },
    methods: {
        ...mapMutations(['UPDATE_LOADING'])
    }
};
let updateData = function (data, isFilterUpdate = false) {
    const { source, jobType } = data;

    this.pageData = {};
    this.content = data;
    (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : '';
    this.filter = this.filterSelection;
    this.UPDATE_LOADING(false);

    if (!isFilterUpdate) {
        this.$_updateFilterObject();
        if (source || jobType) this.$route.meta[`${this.name}Facet`] = source ? source : jobType;
    }
};

export const pageMixin =
    {
        mixins: [sortAndFilterMixin],
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

        beforeRouteUpdate(to, from, next) {

            this.UPDATE_LOADING(true);
            const toName = to.path.slice(1);
            const savedTerm = to.meta[this.$_calcTerm(toName)];
            this.pageData = {};
            this.items = [];
            new Promise((resolve, reject) => {
                if(!to.query.q||!to.query.q.length){resolve({})}
                else if(!savedTerm||(savedTerm.term!==to.query.q)){
                    this.updateSearchText(to.query.q).then(({term,docType})=> {
                        this.$route.meta[this.$_calcTerm(toName)] = {term: to.query.q, luisTerm: term,docType};
                        resolve({luisTerm:to.meta[this.$_calcTerm(toName)].luisTerm,docType});
                    })}else{
                    resolve({luisTerm:savedTerm.luisTerm,docType:savedTerm.docType});
                }
            }).then(({luisTerm,docType})=>{
                const updateFilter = (to.path===from.path&&to.query.q===from.query.q);
                this.fetchingData({ name: toName, params: { ...to.query, ...to.params}, luisTerm,docType})
                    .then(({data}) => {
                        updateData.call(this, data,updateFilter);
                    }).catch(reason => {
                        this.UPDATE_LOADING(false);
                });
                next();
            });
        },

        watch: {
            myCourses(val) {
                const courseIndex = this.filterObject.findIndex(i => i.modelId === "course");
                if (courseIndex > -1)
                    this.filterObject[courseIndex].data = val;
            }
        },
        computed: {
            ...mapGetters(['term', 'isFirst', 'myCourses', 'luisTerm']),
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
                filterObject: null
            };
        },

        components: { foodExtra, ResultItem, SuggestCard, ResultTutor, ResultJob, ResultVideo, ResultBook, ResultFood },

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
            if (this.query.course) this.$route.meta.myClasses = this.query.course;
            this.UPDATE_LOADING(true);
            if (!this.query.q || !this.query.q.length) {
                this.fetchingData({ name: this.name, params: { ...this.query, ...this.params } })
                    .then(({ data }) => {
                        updateData.call(this, data);
                    }).catch(reason => {this.UPDATE_LOADING(false);});
            }else {
                this.updateSearchText(this.query.q).then(({term,result,docType}) => {
                    console.log(docType);
                    this.$route.meta[this.$_calcTerm(result)] = {term: this.query.q, luisTerm: term,docType};
                    if (!this.vertical&&result !== this.name) {
                        this.UPDATE_LOADING(false);
                        const routeParams = {path: '/' + result, query: {...this.query, q: this.query.q}};
                        this.$router.replace(routeParams);
                    } else {
                        this.fetchingData({
                            name: this.name,
                            params: {...this.query, ...this.params},
                            luisTerm: term,docType
                        })
                            .then(({ data }) => {
                                updateData.call(this, data);
                            });
                    }
                });
            }

        },
        methods: {
            ...mapActions(['updateSearchText', 'fetchingData']),
            $_updateFilterObject() {
                if (!this.page.filter) { this.filterObject = null }
                else if (!this.subFilterVertical) {
                    this.filterObject = [{ title: 'filter', modelId: "filter", data: this.page.filter }];
                }
                else {
                    this.filterObject = this.page.filter.map((i) => {
                        const item = { title: i.name, modelId: i.id };
                        item.data = (i.id === "course") ? this.myCourses : this.pageData[i.id] ? this.pageData[i.id] : this.getFacet ? this.getFacet : [];
                        if (i.id === "filter" && this.name === "job") item.data = [{ id: "Paid", name: "Paid" }];
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
            $_changeSubFilter({ id, val, type }) {
                const currentFilter = !this.query[id] ? [] : Array.isArray(this.query[id]) ? this.query[id] : [this.query[id]];
                let listo = [val, ...currentFilter];
                if (!type.target.checked) {
                    listo = currentFilter.filter(i => i.toString() !== val.toString());
                }
                if (id === 'course') {
                    this.$route.meta.myClasses = listo;
                }
                const newFilter = { [id]: listo };
                let { q, sort, course, source, filter } = this.query;
                if (val === 'inPerson' && type) sort = "price";
                this.$router.push({ query: { q, sort, course, source, filter, ...newFilter } });
            },
            $_removeFilter(val) {
                let { source, course, filter, jobType } = this.query;
                source = source ? [].concat(source).filter(i => i !== val) : source;
                course = course ? [].concat(course).filter(i => i.toString() !== val.toString()) : course;
                filter = filter ? [].concat(filter).filter(i => i !== val) : filter;
                jobType = jobType ? [].concat(jobType).filter(i => i !== val) : jobType;
                this.$route.meta.myClasses = course;
                this.$router.push({ path: this.name, query: { ...this.query, source, course, filter, jobType } });
            },
            $_openPersonalize() {
                this.$root.$el.querySelector("#myCourses").click();
            },
            $_showSelectedFilter(item) {
                if (!this.subFilterVertical) return this.page.filter.find(i => i.id === item).name;
                return !Number.isNaN(item) && this.myCourses.find(x => x.id === Number(item)) ? this.myCourses.find(x => x.id === Number(item)).name : item;
            }
        },
        props: { hasExtra: { type: Boolean }, currentTerm: { type: [String, Object] }, getFacet: { type: [Array] }, currentSuggest: { type: String }, vertical: {} }

    };
