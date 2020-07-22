<template>
  <div class="item-wrap" data-app>
    <v-card class="elevation-0 mb-2 answerItem">
      <v-card-text>
        <div>
          <b>Answer Text:</b>
          <div class="mb-1">{{answer.text}}</div>
        </div>
        <div>
          <b>Question Text:</b>
          <div class="mb-1">{{answer.questionText}}</div>
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  name: "answerItem",
  props: {
    answer: {}
  }
};
</script>

<style lang="less">
.item-wrap{
  .answerItem{
    border: 1px solid #ccc !important;
  }
}
</style>