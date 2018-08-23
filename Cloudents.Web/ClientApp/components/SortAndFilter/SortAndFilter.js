import { mapActions, mapMutations } from 'vuex'
export default {
    name: "sort-and-filter",
    props: {
        sortOptions: { type: Array, default: () => [] },
        sortVal: {},
        filterOptions: { type: Array, default: () => [] },
        filterVal: { type: Array, default: () => [] }
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
        updateFilter({ id, val, type }) {
            this.UPDATE_SEARCH_LOADING(true);
            let query = {};
            let isChecked = type.target.checked;
            Object.assign(query, this.$route.query);
            let currentFilter = query[id] ? [].concat(query[id]) : [];
            query[id] = [].concat([...currentFilter, val]);
            if (!isChecked) {
                query[id] = query[id].filter(i => i !== val);
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