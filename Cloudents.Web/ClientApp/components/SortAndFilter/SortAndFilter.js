import { mapActions, mapMutations } from 'vuex'
export default {
    name: "sort-and-filter",
    props: {
        sortOptions: { type: Array, default: () => [] },
        sortVal: {},
        filterOptions: { type: Array, default: () => [] },
        filterVal: { type: Array, default: () => [] },
       
    },
    computed:{
        filterList(){
            return this.filterOptions;
        }
    },
    methods: {
        ...mapActions(['setFilteredCourses']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),
        updateSort(val) {
            this.$router.push({ query: { ...this.$route.query, sort: val } });
        },
        isChecked(singleFilter, filterItem){
            return this.filterVal.find((item)=>{
               return item.key===singleFilter.id && item.value === ( filterItem.id ? filterItem.id.toString() : filterItem.toString())});
        },
        updateFilter({ id, val, event }) {
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
            if (val === 'inPerson' && isChecked) { query.sort = "price" }
            if (id === 'course') { this.setFilteredCourses(query.course) }
            this.$router.push({ query });
        }
    },
    created(){
        console.log('options filter::',this.filterOptions,   'val filter::',  this.filterVal)
    }
}