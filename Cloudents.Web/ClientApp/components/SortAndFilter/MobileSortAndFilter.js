import dialogToolbar from '../dialog-toolbar/DialogToolbar.vue'
import {mapActions, mapGetters, mapMutations} from 'vuex'
import { LanguageService } from "../../services/language/languageService";
export default {
    model: {
        prop: "value",
        event: "input"
    },
    components: {DialogToolbar: dialogToolbar},
    data() {
        return {
            filters: {},
            sort: '',
            filtersSelected: [],
            toolBarTitle: LanguageService.getValueByKey("mobileSortAndFilter_toolbarTitle")
        };
    },
    props: {
        value: {type: Boolean},
        // sortOptions: {type: Array, default: () => []},
        filterOptions: {type: Object, default: () =>{}},
        filterVal: {type: Array, default: () => []},
        sortVal: {}
    },
    computed:{
        ...mapGetters(['getSort']),
        filterList(){
            return this.filterOptions.filterChunkList;
        }
    },
    watch: {
        filterVal(val) {
            this.initFilters(val);
            this.sort = this.sortVal ? this.sortVal : (this.sortOptions && this.sortOptions.length) ? this.sortOptions[0] : "";
        },
        value(val){
            if(!!val){
                document.getElementsByTagName("body")[0].className="noscroll";
            }else{
                document.body.removeAttribute("class","noscroll");
            }
        }
    },
    methods: {
        ...mapActions(['setFilteredCourses', 'updateSort']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),

        initFilters(filters = []) {
            //init sort
            this.sortOptions = this.getSort;
            if(!!this.sortOptions){
                if(this.$route.query.sort){
                    this.sort = this.$route.query.sort;
                }else{
                    this.sort = this.sortOptions[0].key;
                }
            }else{
                this.sort = "";
            }

            //init filters
            this.filtersSelected = [];
            filters.forEach((filter)=>{
                this.filtersSelected.push(`${filter.filterId}_${filter.name}_${filter.filterType}`);
            });
        },
        applyFilters() {
            let query = {};
            this.filtersSelected.forEach((filter)=>{
                let currentFilter = filter.split('_');
                //currentFilter = [source, biology];
                if(!query[currentFilter[2]]){
                    query[currentFilter[2]] = [];
                }
                query[currentFilter[2]].push(currentFilter[0]);
            });
            if (this.sort){
                query.sort = this.sort;
            }
            if (this.$route.query.term){
                query.term = this.$route.query.term;
            }
            if (this.$route.query && this.$route.query.Course){
                query.Course = this.$route.query.Course;
            }
            if (this.$route.query && this.$route.query.uni){
                query.uni = this.$route.query.uni;
            }
            if(JSON.stringify(query) !== JSON.stringify(this.$route.query)){
                this.UPDATE_SEARCH_LOADING(true);
            }
            this.$router.push({query});
            this.$emit('input', false);
        },

        resetFilters() {
                // this.initFilters();
                // this.$router.push({query: {term: this.$route.query.term}});
                // this.applyFilters();
                this.$emit('input', false);

        },

        $_backAction() {
            this.sort = this.sortVal;
            this.initFilters(this.filterVal);
            this.$emit('input', false);
        }
    },


    created() {
        this.initFilters( this.filterVal);
    },
}
