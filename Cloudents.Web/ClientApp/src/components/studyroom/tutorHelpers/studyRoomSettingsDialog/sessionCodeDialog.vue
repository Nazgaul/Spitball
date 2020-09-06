<template>
  <v-dialog :value="true" max-width="550px" :fullscreen="$vuetify.breakpoint.xsOnly" persistent>
    <div class="createCouponDialog">
      <v-icon class="close-dialog" v-text="'sbf-close'" @click="onClose"></v-icon>
      <div class="text-wrap pt-4 pt-sm-0">
        <template>
          <div class="dialog-title pb-10 pb-sm-11">Welcome to Tailor-ed interactive class</div>
          <v-form v-model="seesionCode" ref="seesionCode">
            <v-layout justify-space-between wrap class="inputs-coupon">
              <v-flex xs12 pb-1 pb-sm-0>
                <v-text-field 
                :error-messages="codeErr"
                v-model="sessionCodeValue"
                autofocus 
                :rules="[rules.required,rules.notSpaces,rules.minimumChars,rules.maximumChars]"
                :label="'Enter your 8 digit code'" 
                placeholder=" " 
                autocomplete="nope"
                dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44"/>
              </v-flex>
            </v-layout>
          </v-form>
        </template>
      </div>
      <div class="btns-wrap">
        <v-btn @click="onClose" class="dialog-btn btn-cancel me-1 me-sm-3" color="white" height="40" rounded depressed>
          <span> {{$t('coupon_btn_cancel')}} </span>
        </v-btn>
        <v-btn @click="enterSessionCode" :loading="loadingBtn" class="ms-1 ms-sm-0 dialog-btn white--text" height="40" rounded depressed color="#4c59ff">
          <span> {{$t('coupon_btn_submit')}} </span>
        </v-btn>
      </div>
    </div>
  </v-dialog>
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules';
import * as routeName from '../../../../routes/routeNames.js';

export default {
  data() {
    return {
      sessionCodeValue:'',
      seesionCode:false,
      rules: {
        required: (value) => validationRules.required(value),
        minimumChars: (value) => validationRules.minimumChars(value, 8),
        maximumChars: (value) => validationRules.maximumChars(value, 8),
        notSpaces: (value) => validationRules.notSpaces(value),
      },
      loadingBtn:false,
      codeErr:''
    }
  },
  methods: {
    onClose(){
      this.$emit('closeSessionCode')
    },
    enterSessionCode() {
      if(this.$refs.seesionCode.validate()){
        this.loadingBtn = true;
        let self = this;
        let params = {
          roomId: this.$route.params.id,
          code:this.sessionCodeValue
        }
        self.codeErr = '';
        this.$store.dispatch('updateTailorEd',params)
          .then(()=>{
            let x = self.$router.resolve({
              name:routeName.StudyRoom,
              params:{
                id:params.roomId
              }
            });
            location.href = x.href
          })
          .catch((err=>{
            self.codeErr = 'text text error';
          }))
          .finally(()=>{
          self.loadingBtn = false;
        })
      }
    },
  },
};
</script>


<style lang="less">
@import "../../../../styles/mixin.less";
.createCouponDialog {
  background-color: white;
  border-radius: 6px;
  padding: 10px 16px 14px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
  @media (max-width: @screen-xs) {
    height: 100%;
    padding: 10px 14px 14px;
  }
  .close-dialog {
    cursor: pointer;
    position: absolute;
    right: 0;
    font-size: 12px;
    padding-top: 6px;
    padding-right: 16px;
    color: #b8c0d1;
      @media (max-width: @screen-xs) {
        padding-top: 0;
        padding-right: 10px;
      }
  }
  .text-wrap {
    color: #43425d;
    height: inherit;
    .dialog-title {
      font-size: 18px;
      font-weight: 600;
      @media (max-width: @screen-xs) {
        text-align: center;
        letter-spacing: -0.34px;
      }
    }
    .inputs-coupon {
      margin: 0 auto;
      @media (max-width: @screen-xs) {
        max-width: 292px;
      }
      .v-text-field__details{
        padding: 0 !important;
      }
      .input-fields {
        width: 100%;
      }
    }
  }
  .btns-wrap {
    text-align: center;
    .dialog-btn {
      min-width: 140px;
      @media (max-width: @screen-xs) {
        min-width: 120px;
      }
      text-transform: capitalize;
      font-size: 14px;
      font-weight: 600;
      &.btn-cancel {
        color: #4c59ff;
        border: 1px solid #4c59ff !important;
      }
    }
  }
}
</style>