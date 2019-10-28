<template>
  <div class="uni-class-container">
    <h1 v-language:inner>setUniClass_find_relevant</h1>
    <div class="uni-class-button-cntainer">
      <button @click="chooseUni()" v-language:inner>setUniClass_choose_now</button>
    </div>
    <building-image class="building-img"></building-image>
  </div>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import buildingImage from "./img/Building_pinned_card.svg";
export default {
  components: {
    buildingImage
  },
  methods: {
    ...mapActions([
      "changeSelectUniState",
      "updateCurrentStep",
      "updateLoginDialogState"
    ]),
    ...mapGetters(["getAllSteps", "accountUser"]),
    chooseUni() {
      let user = this.accountUser();
      if (user == null) {
        this.updateLoginDialogState(true);
      } else {
        let universitySteps = this.getAllSteps();
        this.$router.push({name: 'addUniversity'});
      }
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.uni-class-container {
  border-radius: 4px;
  // box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
  padding: 34px !important;
  display: flex;
  flex-direction: column;
  max-width: @cellWidth;
  h1 {
    font-size: 22px;
    font-weight: 600;
    line-height: 1.64;
    color: @color-blue-new;
  }
  .uni-class-button-cntainer {
    margin: 0 auto;
    display: flex;
    margin-top: 29px;
    button {
      width: 165px;
      height: 32px;
      border-radius: 16px;
      background-color: @color-blue-new;
      color: #fff;
    }
  }
  .building-img {
    position: absolute;
    bottom: 0;
    right: 25px;
  }
}
</style>
