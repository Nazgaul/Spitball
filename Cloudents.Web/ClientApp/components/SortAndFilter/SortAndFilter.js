import {mapActions} from 'vuex'
export default{
    name:"sort-and-filter",
    props:{sortOptions:Array,sortVal:{},filterOptions:{},filterVal:{}},
    methods:{
        ...mapActions(['setFilteredCourses']),
        updateSort(val){
            this.$router.push({query:{...this.$route.query,sort:val}});
        },
        updateFilter({id,val,type}){
            let query={};
            let isChecked=type.target.checked;
            Object.assign(query,this.$route.query);
            let currentFilter=query[id]?[].concat(query[id]):[];
            query[id]=[].concat([...currentFilter,val]);
            if(!isChecked){
                query[id]=query[id].filter(i=>i!==val);
            }
            if (val === 'inPerson' && isChecked){query.sort="price"}
            if(id==='course'){this.setFilteredCourses(query.course)}
            this.$router.push({query});
        }
    }
}