import DialogToolbar from '../dialog-toolbar/DialogToolbar.vue'
import {mapActions, mapGetters, mapMutations} from 'vuex'

export default {
    model: {
        prop: "value",
        event: "input"
    },
    data() {
        return {
            filters: {},
            sort: '',
            filtersSelected: []
        }

    },
    components: {DialogToolbar},
    props: {
        value: {type: Boolean},
        // sortOptions: {type: Array, default: () => []},
        filterOptions: {type: Array, default: () => []},
        filterVal: {type: Array, default: () => []},
        sortVal: {}
    },
    methods: {
        ...mapActions(['setFilteredCourses']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),

        initFilters(filters = []) {
            //init sort
            this.sortOptions = this.getSort;
            if(!!this.sortOptions){
                if(this.$route.query.sort){
                    this.sort = this.$route.query.sort
                }else{
                    this.sort = this.sortOptions[0];
                }
            }


            //init filters
            this.filters = this.getFilters;
            this.filtersSelected = [];
            filters.forEach((filter)=>{
                this.filtersSelected.push(`${filter.key}_${filter.value}`);
            })
        },


        applyFilters() {
            let query = {};
            this.filtersSelected.forEach((filter)=>{
                let currentFilter = filter.split('_');
                //currentFilter = [source, biology];
                if(!query[currentFilter[0]]){
                    query[currentFilter[0]] = [];
                }
                query[currentFilter[0]].push(currentFilter[1])
            });

            if (this.sort){
                query.sort = this.sort;
            }

            if (this.$route.query.q){
                query.q = this.$route.query.q;
            }

            this.UPDATE_SEARCH_LOADING(true);
            this.$router.push({query});
            this.$emit('input', false);
        },

        resetFilters() {
            // if (this.filtersSelected.find(i => i.id === 'course')) {
            //     this.setFilteredCourses([]);
            // }
            this.initFilters();
            if (this.sortOptions.length) {
                this.sort = this.sortOptions[0].id;
            }
            this.$router.push({query: {q: this.$route.query.q}});
            this.applyFilters();
            this.$emit('input', false);
        },

        $_backAction() {
            this.sort = this.sortVal;
            this.initFilters(this.filterVal);
            this.$emit('input', false)
        }
    },
    computed:{
        ...mapGetters(['getFilters', 'getSort']),
        filterList(){
            return this.filterOptions
        }
    },
    watch: {
        filterVal(val) {
            this.initFilters(val);
            this.sort = this.sortVal ? this.sortVal : (this.sortOptions && this.sortOptions.length) ? this.sortOptions[0] : "";
        }
    },

    created() {
        this.initFilters(this.filterVal);
        console.log('options', this.sortOptions)

    },
}
