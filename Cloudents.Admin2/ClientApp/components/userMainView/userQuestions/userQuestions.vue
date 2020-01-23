<template>
  <!-- <v-container  class="item-wrap">
    <v-layout>
        <v-flex xs12>
            <question-item
                    :filterVal="filterValue" :questions="UserQuestions"
            ></question-item>
        </v-flex>
    </v-layout>
  </v-container>-->

  <div>
    <v-container class="pl-2 pr-0" fluid grid-list-sm>
      <v-layout row wrap>
        <v-flex xs12 v-for="(question, index) in filteredQuestions" :key="index">
          <question-item :question="question" :filterVal="filterValue"></question-item>
        </v-flex>
      </v-layout>
    </v-container>
  </div>
  
</template>

<script>
import questionItem from "../helpers/questionIitem.vue";
import { mapGetters, mapActions } from "vuex";

export default {
  name: "userQuestions",
  components: { questionItem },
  data() {
    return {
      scrollFunc: {
        page: 0,
        doingStuff: false
      }
    };
  },
  props: {
    userId: {},
    needScroll: {}
  },
  computed: {
    ...mapGetters(["userQuestions", "filterValue"]),
    filteredQuestions: function() {
      return this.userQuestions.filter(f => f.state === this.filterValue);
    }
  },
  watch: {
    needScroll(val, oldval) {
      if (val && val != oldval) {
        this.getUserQuestionsData();
      }
    },
    userId(){
      this.scrollFunc.page = 0;
      this.getUserQuestionsData();
    },
    
  },
  methods: {
    ...mapActions(["getUserQuestions"]),
    nextPage() {
      this.scrollFunc.page++;
    },
    getUserQuestionsData() {
      let self = this;
      let id = self.userId;
      if (this.scrollFunc.doingStuff) {
        return;
      }
      let page = this.scrollFunc.page;
      this.scrollFunc.doingStuff = true;
      self.getUserQuestions({ id, page }).then(isComplete => {
        self.scrollFunc.doingStuff = !isComplete;
        self.nextPage();
      });
    },
  },
    created() {
      this.getUserQuestionsData();
    }  
};
</script>

<style scoped>
</style>