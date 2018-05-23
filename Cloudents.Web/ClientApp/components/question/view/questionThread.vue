<template>
    <div>
        <div class="user-question">
            <question-card :cardData="questionData"></question-card>
        </div>
        <div v-for="answer in questionData.answers" class="user-question">
            <question-card :isAnswer="true" :cardData="answer"></question-card>
        </div>
        <question-text-area v-model="textAreaValue"  class="small" ></question-text-area> <!--:collapsed="addAnswerBtnDisplayed" -->
        <v-btn block color="primary" @click="answer()" :disabled="!validForm" class="add_answer">Add your answer</v-btn>
    </div>
</template>
<script>
import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionCard from "./../helpers/question-card/question-card.vue";
import questionService from "../../../services/questionService";
import { mapGetters } from "vuex";

export default {
  components: { questionTextArea, questionCard },
  props: {
    questionId: { Number }
  },
  data() {
    return {
      textAreaValue: "",
      files: [],
      questionData: null,
      addAnswerBtnDisplayed: true
    };
  },
  computed: {
    validForm() {
      return this.textAreaValue.length;
    }
  },
  methods: {
    answer() {
      var self = this;
      questionService
        .answerQuestion(this.questionId, this.textAreaValue, this.files)
        .then(function() {
          self.textAreaValue = "";
          //TODO: do this on client side
          self.getData();
        });
    },
    showAddAnswer() {
      //TODO: IRENA fix that
      this.addAnswerBtnDisplayed = true;
    },
    getData() {
      var self = this;
      questionService.getQuestion(this.questionId).then(function(response) {
        self.questionData = response.data;
        //TODO: IRENA fix that
        self.addAnswerBtnDisplayed = Boolean(response.data.answers.length);
        self.buildChat();
      });
    },
    buildChat() {
      if (this.talkSession && this.questionData) {
        const otherUser = this.questionData.user;
        if (this.accountUser.id === otherUser.id) {
          return;
        }
        var other1 = new Talk.User({
          id: otherUser.id,
          name: otherUser.name
        });

        var conversation = this.talkSession.getOrCreateConversation(
          `question_${this.questionId}_${this.accountUser.id}_${otherUser.id}`
        );
        conversation.setParticipant(this.chatAccount);
        conversation.setParticipant(other1);
        var chatbox = this.talkSession.createChatbox(conversation);
        this.$nextTick(() => {
          chatbox.mount(this.$refs["chat-area"]);
        });
      }
    }
  },
  watch: {
    talkSession: function(newVal, oldVal) {
      if (newVal) {
        this.buildChat();
      }
    }
  },
  computed: {
    ...mapGetters(["talkSession", "accountUser", "chatAccount"]),
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    },

    validForm() {
      return this.textAreaValue.length;
    }
  },
  created() {
    this.getData();
  }
};
</script>
