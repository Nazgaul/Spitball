<template>
  <div class="elevation-1 mb-2 empty-state-container" xs-12>
    <v-layout column class="pa-3">
      <v-flex>
        <h6 class="mb-1">
          <span v-language:inner>result_no_result_found</span>&nbsp;
          <span class="user-search-text">"{{userText}}"</span>
        </h6>
        <v-flex class="empty-state-content">
          <div>
            <ul>
              <li v-language:inner>result_spelling</li>
              <li v-language:inner>result_different_keywords</li>
              <li v-language:inner>result_general_keywords</li>
              <li v-language:inner>result_fewer_keywords</li>
            </ul>
          </div>
          <div class="empty-state-img-container">
            <img class="empty-state-img" src="./img/binoculars.png" alt>
          </div>
        </v-flex>
      </v-flex>
      <!-- <v-flex mt-4>
                <h6 class="mb-3">
                    <span v-language:inner>result_still_cant_find</span>
                </h6>
      </v-flex>-->
    </v-layout>
  </div>
</template>

<script>
export default {
  props: {
    userText: {
      type: String,
      default: ""
    }
  }
};
</script>

<style lang="less">
.empty-state-container {
    background: #fff;
    border-radius: 4px;
  .empty-state-content {
    display: flex;
    justify-content: space-between;
  }
  .empty-state-img-container {
    display: flex;
    .empty-state-img {
      margin: auto 0;
      pointer-events: none;
    }
  }
}
</style>
