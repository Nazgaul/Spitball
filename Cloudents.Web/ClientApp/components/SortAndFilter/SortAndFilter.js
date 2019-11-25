import { mapActions, mapMutations, mapGetters } from 'vuex'

export default {
    name: "sort-and-filter",
    data() {
        return {
            isEdgeRtl :global.isEdgeRtl
        };
    },
    props: {
        sortVal: {},
        filterOptions: {type: Object, default: () => {}},
        filterVal: {type: Array, default: () => []},

    },
    computed: {
        ...mapGetters(['getSort']),
        filterList() {
            let result = [];
            if(!!this.filterOptions){
                result = this.filterOptions.filterChunkList;
            }
            return result;
        },
        sortOptions() {
            return this.getSort;
        },
        panelList(){
            return [[true], [true]];
        } 
    },
    watch:{
        filterList(){
            setTimeout(()=>{
                let expandedElms = document.getElementsByClassName('v-expansion-panel__body');
                Array.prototype.forEach.call(expandedElms, function(expandElm){
                    expandElm.style.display = "";
                });
            }, 300);
        }
    },
    methods: {
        ...mapActions(['setFilteredCourses', 'updateCurrentStep']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),
        ...mapGetters(['getAllSteps', 'getSchoolName']),
        updateSort(val) {
            this.UPDATE_SEARCH_LOADING(true);
            this.$router.push({query: {...this.$route.query, sort: val}});
        },
        isChecked(singleFilter, filterItem) {
            //keep this way to make sure integer is converted and compare returns correct result
            let keyString = filterItem.key ? filterItem.key : '';
            keyString = keyString + "";
            return this.filterVal.find((item) => {
            let filterId = item.filterId + "";
              return item.filterType === singleFilter.id && filterId === keyString;

            });
        },
        openEditClass(){
            let schoolName = this.getSchoolName();
            let steps = this.getAllSteps();
            let step = !!schoolName ? steps.set_class : steps.set_school;
            this.updateCurrentStep(step);
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
                query.sort = "price";
            }
            if (id.toLowerCase() === 'course') {
                this.setFilteredCourses(query.course);
            }
            this.$router.push({query});
        },
        isRadioChecked(singleSort, index){
           return this.sortVal ? this.sortVal === singleSort.key : index===0;
        },

    }
}