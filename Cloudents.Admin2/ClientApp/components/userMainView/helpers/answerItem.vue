<template>
  <div class="item-wrap" data-app>
   
    <v-card class="elevation-0 mb-2 answerItem">
      <v-card-title class="justify-end grey lighten-2 py-0">
        <v-btn flat v-if="!isOk && !isDeleted" @click="approveAnswer(answer, index)" :disabled="proccessedAnswers.includes(answer.id)">
          <v-icon color="green">check</v-icon>
        </v-btn>
        <v-btn icon v-if="!isDeleted" :disabled="proccessedAnswers.includes(answer.id)" @click="deleteAnswer(answer, index)">
          <v-icon>delete</v-icon>
        </v-btn>
      </v-card-title>
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
import { deleteAnswer } from "../../answer/answerComponents/delete/deleteAnswerService";
import { aproveAnswer } from "../../answer/answerComponents/flaggedAnswers/flaggedAnswersService";
import { mapActions } from "vuex";
export default {
  name: "answerItem",
  data() {
    return {
      proccessedAnswers: []
    };
  },
  props: {
    answer: {},
    filterVal: {
      type: String,
      required: false
    }
  },
  computed: {
    ...mapActions(["deleteAnswerItem"]),
    isOk() {
      return this.filterVal === "ok";
    },
    isPending() {
      return this.filterVal === "pending";
    },
    isFlagged() {
      return this.filterVal === "flagged";
    },
    isDeleted() {
      return this.filterVal === "deleted";
    }
  },
  methods: {
    isVisible(itemState) {
      return itemState.toLowerCase() === this.filterVal.toLowerCase();
    },
    deleteAnswer(answer, index) {
      let id = answer.id;
      let numberArr = [];
      numberArr.push(id);
      let self = this;
      deleteAnswer(numberArr).then(
        resp => {
          self.$toaster.success(`Answer were deleted: ${id}`);
          this.markAsProccessed(numberArr);
          self.deleteAnswerItem;
        },
        error => {
          self.$toaster.error("Something went wrong");
        }
      );
    },
    markAsProccessed(arrIds) {
      for (let i = 0; i < arrIds.length; i++) {
        this.proccessedAnswers.push(arrIds[i]);
      }
      return this.proccessedAnswers;
    },

    approveAnswer(answer, index) {
      aproveAnswer(answer.id).then(
        resp => {
          this.$toaster.success(`Answer ${answer.id} approved`);
          this.markAsProccessed([answer.id]);
        },
        error => {
          this.$toaster.error("Something went wrong");
        }
      );
    }
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