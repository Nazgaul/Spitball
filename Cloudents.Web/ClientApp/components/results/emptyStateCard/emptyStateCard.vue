<template>
  <div class="elevation-1 mb-2 empty-state-container" xs-12>
    <v-layout column class="pa-4 empty-state-top-layout">
      <v-flex>
        <div class="mb-1 user-search-text-container text-truncate" v-show="userText">
          <span v-t>result_no_result_found_for</span>&nbsp;
          <span class="user-search-text">"{{userText}}"</span>
        </div>
        <div class="mb-1 user-search-text-container" v-show="!userText">
          <span v-t>result_no_result_found</span>
        </div>
        <v-flex :class="['empty-state-content',isCourse? 'visible-hidden':'']">
          <div>
            <ul>
              <li v-t>result_spelling</li>
              <li v-t>result_different_keywords</li>
              <li v-t>result_general_keywords</li>
              <li v-t>result_fewer_keywords</li>
            </ul>
          </div>
        </v-flex>
      </v-flex>
    </v-layout>
    <v-layout column class="pa-4 empty-state-bottom-layout" v-show="helpAction">
      <v-flex>
        <div class="mb-1 user-search-cant-find-text">
          <span v-t>result_still_cant_find</span>
        </div>
        <div class="mb-1 user-search-button-container">
          <button @click="helpAction()" v-t>result_get_help</button>
        </div>
      </v-flex>
    </v-layout>
  </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
export default {
    data() {
        return {
            isCourse: false,
        };
    },
  created() {
    if(this.$route.query && this.$route.query.Course){
      this.isCourse = true
    }
  },
  computed: {
    ...mapGetters(['accountUser']),
    userText() {
      return this.$route.query.term;
    },  
  },
  methods: {
    ...mapActions(['updateNewQuestionDialogState']),
    helpAction(){
      (this.accountUser == null)? this.$openDialog('login') : this.updateNewQuestionDialogState(true);
    }
  },
};
</script>

<style lang="less" src="./emptyStateCard.less"></style>
