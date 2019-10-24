<template>
 <v-data-table
    :headers="itemHeaders"
    :items="userSoldDocItems"
    hide-actions
    class="elevation-1">
    <template slot="items" slot-scope="props">
      <td v-for="(item, name) in props.item" :key="name">
        <span v-if="name !== 'url'">
          <a v-if="name === 'itemId'" :href="userSoldDocItems[props.index].url" v-html="item"/>
          <span v-else>{{item}}</span>
        </span>
      </td>
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
        let titles = Object.keys(this.userSoldDocItems[0])
        titles.splice(titles.indexOf('url'),1)
        return titles.map(item=>{
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
      }
    },
    created() {
      this.getUserSolds()
    }
  }
</script>