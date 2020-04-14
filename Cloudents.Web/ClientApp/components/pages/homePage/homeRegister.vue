<template>
    <div class="homeRegister d-sm-flex d-block text-center px-4 pa-sm-0">
        <div class="boxStudent">
            <h3 class="boxHeader" v-t="'homePage_student'"></h3>
            <h4 class="boxDesc mt-3 mb-6" v-t="'homePage_student_text'"></h4>
            <button class="boxBtns btnsLearn mb-3" @click="studentRegister" v-t="'homePage_student_btn_register'"></button>
            <div class="boxNoCredit" v-t="'homePage_credit'"></div>
        </div>
        <div>
            <h3 class="boxHeader" v-t="'homePage_teacher'"></h3>
            <h4 class="boxDesc mt-3 mb-6" v-t="'homePage_teacher_text'"></h4>
            <button class="boxBtns btnsTeach mb-3" @click="teacherRegister" v-t="'homePage_teacher_btn_register'"></button>
            <div class="boxNoCredit" v-t="'homePage_credit'"></div>
        </div>
    </div>
</template>

<script>
export default {
    methods: {
        studentRegister() {
            this.$store.commit('setComponent', 'register')
        },
        teacherRegister() {
            this.$store.commit('setComponent', 'registerTeacher')
        },
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";

.homeRegister {
    .responsive-property(margin-bottom, 170px, null, 54px);
    color: @global-purple;
    display: flex;
    justify-content: space-evenly;

    .boxStudent {
      @media (max-width: @screen-xs) {
        margin: 30px 0;
      }
    }
    .boxHeader {
      font-size: 32px;
      font-weight: normal;
    }
    .boxDesc {
      font-weight: normal;
      line-height: 1.6;
      font-size: 18px;
      max-width: 366px;
      width: 100%;
      margin: 0 auto;
      padding: 0 10px;
      @media (max-width: @screen-sm) {
        font-size: 16px;
      }
    }
    .boxBtns {
      border-radius: 28px;
      border: solid 1px #4c59ff;
      width: 230px;
      height: 44px;
      font-size: 16px;
      font-weight: 600;
      outline: none;
      .responsive-property(max-width, 230px, null, 260px);

      &.btnsLearn {
        color: #4c59ff;
        border: solid 1px #4c59ff;
      }
      &.btnsTeach {
        color: #41c4bc;
        border: solid 1px #41c4bc;
      }
    }
    .boxNoCredit {
      color: #a8a8ac;
    }
}
</style>