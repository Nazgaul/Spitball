<template>
  <v-dialog overlay-opacity="0.9" :value="true" max-width="550px" :fullscreen="$vuetify.breakpoint.xsOnly" persistent>
    <div class="createCouponDialog">
      <div class="text-wrap pt-4 pt-sm-0">
        <template>
          <div class="dialog-title pb-10 pb-sm-11">{{$t('sessionCode_title')}}</div>
          <v-form v-model="seesionCode" ref="seesionCode">
            <v-layout justify-space-between wrap class="inputs-coupon">
              <v-flex xs12 pb-1 pb-sm-0>
                <v-text-field 
                :error-messages="codeErr"
                v-model="sessionCodeValue"
                autofocus 
                :rules="[rules.required,rules.notSpaces,rules.minimumChars,rules.maximumChars]"
                :label="$t('sessionCode_label')" 
                placeholder=" " 
                autocomplete="nope"
                dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44"/>
              </v-flex>
            </v-layout>
          </v-form>
        </template>
      </div>
      <div class="btns-wrap">
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
          .catch(()=>{
            self.codeErr = self.$t('sessionCode_err');
          })
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
    }
  }
}
</style>