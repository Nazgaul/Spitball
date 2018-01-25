import DialogToolbar from '../dialog-toolbar/DialogToolbar.vue'
import {mapActions} from 'vuex'
export default {
    model: {
        prop: "value",
        event: "input"
    },
    data() {
        return { filters: {source:[],course:[],jobType:[],filter:[]},
            sort: this.sortVal?this.sortVal:this.sortOptions?this.sortOptions[0].id:""}
    },
    components: {  DialogToolbar },
    props: { value: { type: Boolean }, sortOptions: {}, filterOptions: {}, filterVal: {}, sortVal: {}},
    methods: {
        ...mapActions(['setFilteredCourses']),

        initFilters(val=[]){
            this.filters={source:[],course:[],jobType:[],filter:[]};
            [].concat(val).forEach(({key,value})=>{
                this.filters[key]=this.filters[key].concat(value);
            });
        },
        applyFilters() {
            //if filters have courses and courses has been changed save the changes
            if(this.filterOptions.find(i=>i.modelId==='course')&&
                this.filters.course&&this.filters.course.filter(val=>this.filterVal.find(t=>t.value===val)).length!==this.filters.course.length
            ){
                this.setFilteredCourses(this.filters.course);
            }
            if(this.filters.filter.includes('inPerson')){this.sort="price"}
            this.$router.push({query: {q: this.$route.query.q, sort: this.sort,...this.filters}});
            this.$emit('input', false);
        },
        resetFilters() {
            if(this.filterOptions.find(i=>i.modelId==='course')){
                this.setFilteredCourses([]);
            }
            this.initFilters();
            this.$router.push({query: {q: this.$route.query.q}});
            this.$emit('input', false);
        },
        $_backAction() {
            this.$emit('input', false)
        }
    },
    watch: {
        filterVal(val) {
            this.initFilters(val);
            this.sort=this.sortVal?this.sortVal:this.sortOptions?this.sortOptions[0].id:"";
        }
    },
    created() {
        this.initFilters(this.filterVal);
    }
}