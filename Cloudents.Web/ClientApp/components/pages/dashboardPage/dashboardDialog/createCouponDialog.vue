<template>
  <v-dialog :value="true" max-width="550px" max-height="310px" :fullscreen="$vuetify.breakpoint.xsOnly" persistent>
    <div class="createCouponDialog">
      <v-icon class="close-dialog" v-text="'sbf-close'" v-closeDialog />
      <div class="text-wrap pt-4 pt-sm-0">
        <template v-if="!showSuccess">
          <div class="dialog-title pb-10 pb-sm-11">{{$t('coupon_create_title')}}</div>
          <v-form v-model="validCreateCoupon" ref="validCreateCoupon">
            <v-layout justify-space-between wrap class="inputs-coupon pr-0 pr-sm-4">
                
              <v-flex xs12 sm9 pr-0 pr-sm-4 pb-1 pb-sm-0>
                <v-text-field :error-messages="couponErr" autofocus :rules="[rules.required,rules.notSpaces,rules.minimumChars,rules.maximumChars]"
                    v-model="couponCode" :label="$t('coupon_label_code')" :placeholder="placeHoldersEmpty" autocomplete="nope"
                    dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44"/>
              </v-flex>
              <v-flex sm3 v-if="!$vuetify.breakpoint.xsOnly">
                <v-text-field v-model="couponValue" :label="$t('coupon_label_value')"
                  :placeholder="placeHoldersEmpty" autocomplete="nope" :rules="[rules.required,rules.integer,rules.minimum,rules.maximum]"
                  dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44"/>
              </v-flex>

              <v-flex xs12 sm8 pr-0 pr-sm-0 pb-1 pb-sm-0>
                <v-select v-model="couponType" class="coupon-type" color="#304FFE" :items="couponTypesList"
                    outlined :height="$vuetify.breakpoint.xsOnly?50: 44" item-text="key" :append-icon="'sbf-triangle-arrow-down'" dense
                    :label="$t('coupon_label_type')" :rules="[rules.required]" :placeholder="placeHoldersEmpty">
                    <template slot="item" slot-scope="data">
                     <span class="subtitle-1">{{data.item.key}}</span>
                    </template>
                </v-select>
              </v-flex>

              <v-flex xs6 pr-2 pr-sm-0 v-if="$vuetify.breakpoint.xsOnly">
                <v-text-field v-model="couponValue" :label="$t('coupon_label_value')"
                  :placeholder="placeHoldersEmpty" autocomplete="nope" :rules="[rules.required,rules.integer,rules.minimum,rules.maximum]"
                  dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44"/>
              </v-flex>

              <v-flex xs6 sm4 pl-2 pl-sm-4>
                <v-menu ref="datePickerMenu" v-model="datePickerMenu" :close-on-content-click="false" transition="scale-transition" offset-y max-width="290px" min-width="290px">
                  <template v-slot:activator="{ on }">
                      <v-text-field class="date-input" :rules="[rules.required]" :label="$t('coupon_label_date')" autocomplete="nope"
                      v-model="dateFormatted" prepend-inner-icon="sbf-calendar" @blur="date = parseDate(dateFormatted)"
                      dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44" v-on="on" />
                  </template>                  
                  <v-date-picker color="#4C59FF" class="date-picker-coupon" :next-icon="isRtl?'sbf-arrow-left-carousel':'sbf-arrow-right-carousel'" :prev-icon="isRtl?'sbf-arrow-right-carousel':'sbf-arrow-left-carousel'" v-model="date" no-title @input="datePickerMenu = false">
                    <v-spacer></v-spacer>
                    <v-btn text class="font-weight-bold" color="#4C59FF" @click="datePickerMenu = false">{{$t('coupon_btn_calendar_cancel')}}</v-btn>
                    <v-btn text class="font-weight-bold" color="#4C59FF" @click="$refs.datePickerMenu.save(date)">{{$t('coupon_btn_calendar_ok')}}</v-btn>
                  </v-date-picker>
                </v-menu>
              </v-flex>
            </v-layout>
          </v-form>
        </template>
        <template v-else>
          <div class="succes-title pb-9 pb-sm-12 pt-2 pt-sm-6">{{$t('coupon_create_succes')}}</div>
          <div class="coupon-box">
            <img class="coupon-box_img" src="./images/b.png" alt="">
            <span class="coupon-box_code" v-text="couponCode"/>            
          </div>
        </template>
      </div>
      <div class="btns-wrap">
        <v-btn v-closeDialog class="dialog-btn btn-cancel mr-1 mr-sm-3" color="white" height="40" rounded depressed>
          <span v-t="showSuccess?'coupon_btn_exit':'coupon_btn_cancel'"/>
        </v-btn>
        <v-btn :loading="loadingBtn" @click="showSuccess? copyCode() : createMyCoupon()" class="ml-1 ml-sm-0 dialog-btn white--text" height="40" rounded depressed color="#4c59ff">
          <span v-t="showSuccess?'coupon_btn_copy':'coupon_btn_submit'"/>
        </v-btn>
      </div>
    </div>

    <v-snackbar v-model="snackbar" :timeout="3000" top>
      <div class="text-center flex-grow-1">{{ $t('coupon_toaster_copy') }}</div>
    </v-snackbar>
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
      snackbar:false,
      placeHoldersEmpty:'',
      datePickerMenu:false,
      date: new Date().FormatDateToString(),
      dateFormatted: '',
      couponErr:'',
      isRtl: global.isRtl,
    }
  },
  watch: {
    couponCode(val){
      this.couponCode = val.replace(/\s/g,''); 
      this.couponErr = '';
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
    },
    date () {
      this.dateFormatted = this.formatDate(this.date)
    },
  },
  methods: {
    ...mapActions(['createCoupon']),
    copyCode(){
      let self = this;
      this.loadingBtn = true;
      this.$copyText(this.couponCode).then(() => {
          self.loadingBtn = false;
          self.snackbar = true;
      })
    },
    createMyCoupon() {
      if(this.$refs.validCreateCoupon.validate()){
        this.loadingBtn = true;
        let params = {
          value: this.couponValue,
          code: this.couponCode,
          couponType: this.couponType,
          expiration: new Date(this.date).toISOString()
        }
        let self = this;
        this.createCoupon(params).then(()=>{
          self.loadingBtn = false;
          self.showSuccess = true;
          self.couponErr = '';
        }).catch(()=>{
          self.couponErr = this.$t('coupon_already_exists');
          self.loadingBtn = false;
        })
      }
    },
    formatDate (date) {
      if (!date) return null

      const [year, month, day] = date.split('-')
      return `${month}/${day}/${year}`
    },
    parseDate (date) {
      if (!date) return null
      const [month, day, year] = date.split('/')
      return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`
    },
  },
  mounted() {
    this.placeHoldersEmpty = ' '
    setTimeout(()=>{
      this.dateFormatted = this.formatDate(new Date().FormatDateToString())
    })
  },
    beforeDestroy(){
      if(!this.$store.state.hasOwnProperty('couponStore')) {
        storeService.unregisterModule(this.$store, 'couponStore');
      }
     },
    created() {
      if(!this.$store.state.hasOwnProperty('couponStore')) {
        storeService.registerModule(this.$store, 'couponStore', couponStore);
      }
    },
};
</script>


<style lang="less">
@import "../../../../styles/mixin.less";
.date-picker-coupon{
  .v-date-picker-table{
    .v-btn{
      .v-btn--active{
        color:white;
      } 
    }
  } 
  .v-btn__content{
    .v-icon{
      font-size: 16px;
    }
  }
}
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
    cursor: pointer;
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
    .succes-title{
      font-size: 24px;
      font-weight: 600;
      letter-spacing: -0.45px;
      text-align: center;
      max-width: 340px;
      margin: 0 auto;
      @media (max-width: @screen-xs) {
        font-size: 20px;
        letter-spacing: -0.38px;
      }
    }
    .coupon-box{
      text-align: center;
      position: relative;
      border-radius: 4px;
      border: dashed 2px #b8c0d1;
      padding: 8px 18px 10px;
      @media (max-width: @screen-xs) {
        padding: 10px 18px;
      }
      max-width: 256px;
      margin: 0 auto;
      .coupon-box_img{
          transform: scaleX(1) /*rtl:append:scaleX(-1)*/;
          position: absolute;
          top: -16px;
          left: 8px;
      }
      .coupon-box_code{
        font-size: 24px;
        font-weight: 600;
        text-align: center;
        line-height: 1.2;
        letter-spacing: 1.2px;
      }
    }
    .inputs-coupon {
      margin: 0 auto;
      @media (max-width: @screen-xs) {
        max-width: 292px;
      }
      .date-input{
        // .v-label{
        //   left: initial !important;
        //   right: 14px !important;
        //   @media (max-width: @screen-xs) {
        //     right: 2px !important;
        //   }
        // }
        input[type="text"] {
          padding: 8px 0 0 2px !important;
        }
        .v-input__icon--prepend-inner {
          i {
              font-size: 20px;
              color: #4a4a4a;
            @media (max-width: @screen-xs) {
              margin-top: 14px;
            }
              margin-top: 10px;
          }
        }
      }
      .v-text-field__details{
        padding: 0 !important;
      }
      .coupon-type{
        .v-select__selection--comma{
          line-height: initial;
        }
        .v-list-item__title {
          color: #43425d;
        }
        .v-input__append-inner{
          margin-top: 10px !important;
          @media (max-width: @screen-xs) {
            margin-top: 12px !important;
          }
          i{
              font-size: 6px;
              color: #43425d;
          }
        }
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