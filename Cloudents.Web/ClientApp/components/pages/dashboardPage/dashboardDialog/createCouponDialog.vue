<template>
  <v-dialog :value="true" max-width="550px" :fullscreen="$vuetify.breakpoint.xsOnly" persistent>
    <div class="createCouponDialog">
    <v-icon class="close-dialog" @click="$closeDialog()">sbf-close</v-icon>
        <div class="text-wrap">
            <div class="dialog-title">Create a special offer</div>
            <v-layout row wrap class="inputs-coupon">
              <v-flex xs12>
                <v-text-field
                  dense
                  color="#304FFE"
                  placeholder=" "
                  label="Coupon code"
                  outlined
                  autocomplete="nope"
                  type="text"
                  height="44"
                ></v-text-field>
              </v-flex>
              <v-flex xs12 sm8>
                <v-text-field
                  dense
                  color="#304FFE"
                  placeholder=" "
                  label="Coupon code"
                  outlined
                  autocomplete="nope"
                  type="text"
                  height="44"
                ></v-text-field>
              </v-flex>
              <v-flex xs3>
                <v-text-field
                  dense
                  color="#304FFE"
                  placeholder=" "
                  label="Coupon code"
                  outlined
                  autocomplete="nope"
                  type="text"
                  height="44"
                ></v-text-field>
              </v-flex>
            </v-layout>

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

    }
  }
}
</script>


<style lang="less">
@import '../../../../styles/mixin.less';

.createCouponDialog {
  background-color: white;
  border-radius: 6px;
  padding: 14px 16px;
  // padding: 40px 0 14px;
  min-height: 248px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
  @media (max-width: @screen-xs) {
    height: 100%;
  }
  .close-dialog{
    position: absolute;
    right: 0;
    font-size: 12px;
    padding-top: 6px;
    padding-right: 16px;
    color: #b8c0d1;
  }
  .text-wrap {
    color: #43425d;
    .dialog-title{
      font-size: 18px;
      font-weight: 600;
    }
  .inputs-coupon{
          input[type="text"] {
        padding: 10px !important;
      }
        .input-fields {
      width: 100%;
    }
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