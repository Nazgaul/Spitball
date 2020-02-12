<template>
  <v-dialog :value="true" max-width="480px" :fullscreen="$vuetify.breakpoint.xsOnly" persistent>
    <div class="exitRegisterDialog">
        <div class="text-wrap">
            <div class="dialog-title">{{$t('login_are_you_sure_you_want_to_exit')}}</div>
            <div class="dialog-text">{{$t('login_exiting_information1')}}</div>
        </div>
        <div class="btns-wrap">
            <v-btn @click="exit" class="dialog-btn white--text" height="40" rounded depressed color="#4c59ff">{{$t('login_Exit')}}</v-btn>
            <v-btn @click="$closeDialog()" class="dialog-btn btn-cancel" color="white" height="40" rounded depressed>{{$t('login_text')}}</v-btn>
        </div>
    </div>
   </v-dialog>
</template>

<script>
export default {
  methods: {
    exit() {
      this.$store.dispatch('exit')
      this.$router.push({path: '/'}).catch(() => {})
    }
  }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.exitRegisterDialog {
  background-color: white;
  border-radius: 6px;
  padding: 40px 0 14px;
  min-height: 248px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  @media (max-width: @screen-xs) {
    height: 100%;
    padding: 48px 14px 14px;
  }
  .text-wrap {
    color: #4d4b69;
    text-align: center;
    .dialog-title{
      font-size: 26px;
      font-weight: 600;
      @media (max-width: @screen-xs) {
        font-size: 22px;
      }
      @media (max-width: @screen-xss) {
        padding: 0 20px;
      }

    }
    .dialog-text{
      @media (max-width: @screen-xs) {
        padding-top: 20px;
      }
      padding-top: 14px;
      font-size: 16px;
      line-height: 1.5;
      max-width: 350px;
      margin: 0 auto;
    }
  }
  .btns-wrap{
    text-align: center;
    .dialog-btn{
      min-width: 140px;
      text-transform: capitalize;
      font-size: 14px;
      font-weight: 600;
      &.btn-cancel{
        margin-left: 8px;
        color: #4c59ff;
        border: 1px solid #4c59ff !important;
      }
    }
  }
}

</style>