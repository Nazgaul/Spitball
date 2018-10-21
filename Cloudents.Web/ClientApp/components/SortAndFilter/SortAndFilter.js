import { mapActions, mapMutations, mapGetters } from 'vuex'

export default {
    name: "sort-and-filter",
    data() {
        return {
            //keep this as model for expand panel, to keep it always open
            panelList: [true, true]
        }
    },
    props: {
       // sortOptions: { type: Array, default: () => [] },
        sortVal: {},
        filterOptions: {type: Object, default: () => {}},
        filterVal: {type: Array, default: () => []},

    },
    computed: {
        ...mapGetters(['getSort']),
        filterList() {
            return this.filterOptions.filterChunkList;
        },
        sortOptions() {
            return this.getSort;
        }
    },
    methods: {
        ...mapActions(['setFilteredCourses']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),
        updateSort(val) {
            this.UPDATE_SEARCH_LOADING(true);
            this.$router.push({query: {...this.$route.query, sort: val}});
        },
        isChecked(singleFilter, filterItem) {
            return this.filterVal.find((item) => {
                //trying to fix bug of hebrew
                // return item.filterType === singleFilter.id && item.name === (filterItem.value ? filterItem.value.toString() : '')

                return item.filterType === singleFilter.id && item.filterId === (filterItem.key ? filterItem.key.toString() : '')
            });
        },
        updateFilter({id, val, name, event}) {
            this.UPDATE_SEARCH_LOADING(true);
            let query = {};
            let isChecked = event.target.checked;
            // data from query to obj
            Object.assign(query, this.$route.query);
            let currentFilter = query[id] ? [].concat(query[id]) : [];
            query[id] = [].concat([...currentFilter, val]);

            if (!isChecked) {
                query[id] = query[id].filter(item => item !== val);
            }
            if (val === 'inPerson' && isChecked) {
                query.sort = "price"
            }
            if (id === 'course') {
                this.setFilteredCourses(query.course)
            }
            this.$router.push({query});
        },
        isRadioChecked(singleSort, index){
           return this.sortVal ? this.sortVal === singleSort.key : index===0
        },

    },
}