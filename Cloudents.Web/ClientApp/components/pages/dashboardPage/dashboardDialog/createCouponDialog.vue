<template>
  <v-dialog :value="true" max-width="550px" max-height="310px" :fullscreen="$vuetify.breakpoint.xsOnly" persistent>
    <div class="createCouponDialog">
      <v-icon class="close-dialog" v-text="'sbf-close'" v-closeDialog />
      <div v-if="!showSuccess" class="text-wrap pt-4 pt-sm-0">
        <div class="dialog-title pb-10 pb-sm-11">Create a special offer</div>
        <v-form v-model="validCreateCoupon" ref="validCreateCoupon">
          <v-layout justify-space-between wrap class="inputs-coupon pr-0 pr-sm-4">
              <v-flex xs12 pb-1 pb-sm-3>
                <v-text-field autofocus :rules="[rules.required,rules.notSpaces,rules.minimumChars,rules.maximumChars]"
                  v-model="couponCode" :label="$t('coupon_label_code')" placeholder=" " autocomplete="nope"
                  dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44"/>
              </v-flex>
              <v-flex xs12 sm9 pr-0 pr-sm-4 pb-1 pb-sm-0> 
                <v-select v-model="couponType" class="coupon-type" color="#304FFE" :items="couponTypesList"
                  outlined :height="$vuetify.breakpoint.xsOnly?50: 44" item-text="key" :append-icon="'sbf-triangle-arrow-down'" dense
                  :label="$t('coupon_label_type')" :rules="[rules.required]" placeholder=" "/>

              </v-flex>
              <v-flex xs5 sm3>
                <v-text-field v-model="couponValue" :label="$t('coupon_label_value')"
                  placeholder=" " autocomplete="nope" :rules="[rules.required,rules.integer,rules.minimum,rules.maximum]"
                  dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44"/>
              </v-flex>
          </v-layout>
        </v-form>
      </div>
      <div class="btns-wrap">
        <v-btn v-closeDialog class="dialog-btn btn-cancel mr-1 mr-sm-3" color="white" height="40" rounded depressed>
          <span v-t="'coupon_btn_cancel'"/>
        </v-btn>
        <v-btn :loading="loadingBtn" @click="createMyCoupon" class="ml-1 ml-sm-0 dialog-btn white--text" height="40" rounded depressed color="#4c59ff">
          <span v-t="'coupon_btn_submit'"/>
        </v-btn>
      </div>
    </div>
  </v-dialog>
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules';
import { mapActions } from 'vuex';
import storeService from '../../../../services/store/storeService';
import couponStore from '../../../../store/couponStore';

export default {
  data() {
    return {
      validCreateCoupon: false,
      couponTypesList:[{key: this.$t('coupon_type_flat'),value: 'flat'},{key: this.$t('coupon_type_percentage'),value: 'percentage'}],
      couponCode:'',
      couponType:'',
      couponValue:'',
      maximumValue: Infinity,
      rules: {
        required: (value) => validationRules.required(value),
        minimumChars: (value) => validationRules.minimumChars(value, 5),
        maximumChars: (value) => validationRules.maximumChars(value, 12),
        integer: (value) => validationRules.integer(value),
        notSpaces: (value) => validationRules.notSpaces(value),
        minimum: (value) => validationRules.minVal(value,1),
        maximum: (value) => validationRules.maxVal(value, this.maximumValue),
      },
      loadingBtn:false,
      showSuccess:false,
    }
  },
  watch: {
    couponCode(val){
      this.couponCode = val.replace(/\s/g,''); 
    },
    couponType(val){
      if(val === 'percentage'){
        this.maximumValue = 100;
      }else{
        this.maximumValue = Infinity;
      }
      if(this.couponValue){
        this.$refs.validCreateCoupon.validate()
      }
    }
  },
  methods: {
    ...mapActions(['createCoupon']),
    createMyCoupon() {
      if(this.$refs.validCreateCoupon.validate()){
        this.loadingBtn = true;
        let params = {
          value: this.couponValue,
          code: this.couponCode,
          couponType: this.couponType
        }
        let self = this;
        this.createCoupon(params).then(()=>{
          self.loadingBtn = false;
          self.showSuccess = true;
        })

      }
    }
  },
    beforeDestroy(){
      storeService.unregisterModule(this.$store, 'couponStore');
     },
    created() {
      storeService.registerModule(this.$store, 'couponStore', couponStore);
    },
};
</script>


<style lang="less">
@import "../../../../styles/mixin.less";

.createCouponDialog {
  background-color: white;
  border-radius: 6px;
  padding: 10px 16px 14px;
  min-height: 248px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
  height: 310px;
  @media (max-width: @screen-xs) {
    height: 100%;
    padding: 10px 14px 14px;
  }
  .close-dialog {
    position: absolute;
    right: 0;
    font-size: 12px;
    padding-top: 6px;
    padding-right: 16px;
    color: #b8c0d1;
      @media (max-width: @screen-xs) {
        padding-top: 0px;
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
      .v-text-field__details{
        padding: 0 !important;
      }
      .coupon-type{
        .v-list-item__title {
                color: #43425d;
                line-height: normal;
            }
            .v-input__append-inner{
              margin-top: 10px !important;
              @media (max-width: @screen-xs) {
                margin-top: 12px !important;
              }
            }
            i {
              @media (max-width: @screen-xs) {
                font-size: 6px;
              }
                font-size: 8px;
                color: #43425d;
            }
      }
      input[type="text"] {
        padding: 10px !important;
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