<template>
  <div class="item-wrap" data-app>
    <!-- <v-card class="answer-card" v-for="(answer, index) in answers" :key="index" v-if="isVisible(answer.state)">
            <v-toolbar class="answer-toolbar mt-4 back-color-purple">
                <v-toolbar-title class="answer-text-title">
                    <span class="question-text-label">Answer Text</span>

                    {{answer.text}}
                </v-toolbar-title>
                <v-spacer></v-spacer>
                <span title="Fictive Or Original Question ">{{answer.flaggedUserEmail}}</span>
                <v-spacer></v-spacer>
                <v-flex>
                    <v-tooltip left  attach="tooltip-1">
                        <v-btn slot="activator" icon @click="declineAnswer(answer, index)" class="tooltip-1">
                            <v-icon color="red">close</v-icon>
                        </v-btn>
                        <span>Delete</span>
                    </v-tooltip>
                    <v-list-tile-action-text></v-list-tile-action-text>
                    <v-tooltip left  attach="tooltip-2">
                        <v-btn slot="activator" icon @click="aproveA(answer, index)" class="tooltip-2">
                            <v-icon color="green">done</v-icon>
                        </v-btn>
                        <span>Accept</span>
                    </v-tooltip>
                </v-flex>
            </v-toolbar>

            <v-list two-line avatar>
                <template>
                    <v-list-tile class="answers-list-tile">
                        <v-list-tile-content class="answers-content">
                            <v-list-tile-sub-title class="answer-subtitle">
                                <span class="question-text-label">Question Text</span>
                                {{answer.questionText}}
                            </v-list-tile-sub-title>
                        </v-list-tile-content>
                    </v-list-tile>
                </template>
            </v-list>
    </v-card>-->
    <v-card class="elevation-5">
      <v-card-text>
        <span>
          <b>Answer Text:</b>
        </span>
        <br>
        {{answer.text}}
        <br>
        <span>
          <b>Question Text:</b>
        </span>
        <br>
        {{answer.questionText}}
        <v-spacer></v-spacer>
        <v-btn
          flat
          v-if="!isOk && !isDeleted"
          @click="approveAnswer(answer, index)"
          :disabled="proccessedAnswers.includes(answer.id)"
        >
          <v-icon>check</v-icon>Accept
        </v-btn>
        <v-btn
          flat
          v-if="!isDeleted"
          :disabled="proccessedAnswers.includes(answer.id)"
          @click="deleteAnswer(answer, index)"
        >
          <v-icon>delete</v-icon>Delete
        </v-btn>
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

<style lang="scss">
</style>