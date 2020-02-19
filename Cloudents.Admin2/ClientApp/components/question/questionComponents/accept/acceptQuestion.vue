<template>
  <div class="accept-question-container">
    <h1>Accept Question</h1>
    <div class="wrap">
      <div class="info">
        <!-- <h3>To Accept multiple questions, enter the question Id's seperated with a comma (See example).</h3> -->
        <h4 v-if="infoSuccess">{{infoSuccess}}</h4>
        <h4 v-else-if="infoError">{{infoError}}</h4>
      </div>
      <div class="input-wrap">
        <v-text-field
          height="36"
          solo
          class="id-input"
          type="text"
          v-model="questionsIdString"
          placeholder="example: 1245"
        />
        <v-btn color="#97ed82" @click="acceptByIds">Accept</v-btn>
      </div>
    </div>
  </div>
</template>
<script>
import { acceptQuestion } from "./acceptQuestionService";

export default {
  data() {
    return {
      questionsIds: [],
      questionsIdString: "",
      infoSuccess: "",
      infoError: ""
    };
  },
  methods: {
    acceptByIds() {
      if (this.questionsIdString.length > 0) {
        this.questionsIds = this.questionsIdString.split(",");
        let numberArr = [];
        this.questionsIds.forEach(id => {
          return numberArr.push(parseInt(id.trim()));
        });
        acceptQuestion(numberArr).then(
          resp => {
            this.$toaster.success(
              `Question ${this.questionsIdString} were Accepted`
            );
            this.questionsIdString = "";
            this.questionsIds = [];
          },
          error => {
            this.$toaster.error("Something went wrong");
            console.log("component accept error", error);
          }
        );
      }
    }
  }
};
</script>

<style lang="less" scoped>
.accept-question-container {
  margin: 0 auto;
  .wrap {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    .input-wrap {
      padding-top: 16px;
      display: flex;
      flex-direction: row;
      align-items: baseline;
      justify-content: center;
      .id-input {
        width: 345px;
        height: 36px;
        border-radius: 25px;
        padding-left: 10px;
      }
    }
  }
}
</style>