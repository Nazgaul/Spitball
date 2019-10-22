<template>
 <v-data-table
    :headers="itemHeaders"
    :items="userSoldDocItems"
    hide-actions
    class="elevation-1">
    <template slot="items" slot-scope="props">
      <td v-for="(item, index) in props.item" :key="index">{{formatItem(item)}}</td>
    </template>
  </v-data-table>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
  export default {
    props: {
      userId: {},
      needScroll: {}
    },
    data () {
      return {
        scrollFunc: {
          page: 0,
          doingStuff: false
        }
      }
    },
    computed: {
      ...mapGetters(["userSoldDocItems"]),
      itemHeaders(){
        return Object.keys(this.userSoldDocItems[0]).map(item=>{
          return {text:this.formatTitleName(item),value:item}
        })
      }
    },
    watch: {
      needScroll(val, oldval) {
        if (val && val != oldval) {
          this.getUserSolds();
        }
      }
    },
    methods: {
      ...mapActions(['getUserSoldItems']),
      formatTitleName(string){
        return string.replace(/([A-Z])/g, ' $1').replace(/^./,(str => str.toUpperCase()))
      },
      nextPage() {
        this.scrollFunc.page++;
      },
      getUserSolds(){
        if (this.scrollFunc.doingStuff)return;
        let id = this.userId;
        let self = this;
        let page = this.scrollFunc.page;
        this.scrollFunc.doingStuff = true;

        this.getUserSoldItems({ id, page }).then(isComplete => {
          self.scrollFunc.doingStuff = !isComplete;
          self.nextPage();
        });
      },
      formatItem(item){
        if(typeof item === 'number' && item < 0){
          return Math.abs(item)
        }
        let rgx = /\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d+([+-][0-2]\d:[0-5]\d|Z)/;
        if(typeof item === 'string' && item.search(rgx) === 0){
          let date = new Date(item)
          return `${date.getUTCMonth() + 1}/${date.getUTCDate()}/${date.getUTCFullYear({})}`;
        } else{
          return item;
        }
      }
    },
    created() {
      this.getUserSolds()
    }
  }
</script>