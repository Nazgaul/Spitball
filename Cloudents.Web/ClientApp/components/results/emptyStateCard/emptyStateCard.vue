<template>
  <div class="elevation-1 mb-2 empty-state-container" xs-12>
    <v-layout column class="pa-4 empty-state-top-layout" :class="[isRtl ? 'rtl-background' : '']">
      <v-flex>
        <div class="mb-1 user-search-text-container text-truncate" v-show="userText">
          <span v-language:inner>result_no_result_found_for</span>&nbsp;
          <span class="user-search-text">"{{userText}}"</span>
        </div>
        <div class="mb-1 user-search-text-container" v-show="!userText">
          <span v-language:inner>result_no_result_found</span>
        </div>
        <v-flex :class="['empty-state-content',isCourse? 'visible-hidden':'']">
          <div>
            <ul>
              <li v-language:inner>result_spelling</li>
              <li v-language:inner>result_different_keywords</li>
              <li v-language:inner>result_general_keywords</li>
              <li v-language:inner>result_fewer_keywords</li>
            </ul>
          </div>
        </v-flex>
      </v-flex>
    </v-layout>
    <v-layout column class="pa-4 empty-state-bottom-layout" v-show="helpAction">
      <v-flex>
        <div class="mb-1 user-search-cant-find-text">
          <span v-language:inner>result_still_cant_find</span>
        </div>
        <div class="mb-1 user-search-button-container">
          <button @click="helpAction()" v-language:inner>result_get_help</button>
        </div>
      </v-flex>
    </v-layout>
  </div>
</template>

<script>
export default {
    data() {
        return {
            isRtl: global.isRtl,
            isCourse: false,
        };
    },
  props: {
    userText: {
      type: String,
      default: ""
    },
    helpAction:{
      type: Function,
      default:null
    }
  },
  created() {
    if(this.$route.query && this.$route.query.Course){
      this.isCourse = true
    }
  },
};
</script>

<style lang="less" src="./emptyStateCard.less"></style>
