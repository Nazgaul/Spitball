import { sortAndFilterMixin } from './mixins'
import ResultBook from './ResultBook.vue';
const sortOptions=[{id:'buy',name:'BUY/RENT'},{id:'sell',name:'SELL'}];
export default {
    mixins: [sortAndFilterMixin],
    created() {
        this.filter = 'all';
        this.UPDATE_LOADING(true);
        this.$store.dispatch("bookDetails", { pageName: "bookDetails", isbn13: this.params.id,type:"buy" }).then((res) => {
            this.pageData = res;
            this.UPDATE_LOADING(false);
        });
    },
    data() {
        return { pageData: '',sortVal:"buy",
            sortOptions}
    },
    components: {  ResultBook },
    computed: {
        filteredList: function () {
            return !this.pageData.data?[]:this.filter === 'all' ? this.pageData.data.sort((a, b) => a.price - b.price) : this.pageData.data.filter(item => item.condition === this.filter);
        }
    },

    methods: {
    $_updateFilter({val,type}){
        console.log(val);
        this.filter=(type.target.checked||val==='all')?val:'all';
    },
        $_updateSort(val){
            console.log(val);
            this.sortVal=val;
            this.UPDATE_LOADING(true);
            this.$store.dispatch("bookDetails", { pageName: "bookDetails", isbn13: this.params.id,type:val }).then((res) => {
                this.pageData = res;
                this.UPDATE_LOADING(false);
            });
        }
    },
    props:{filterOptions:{type:Array}}
}