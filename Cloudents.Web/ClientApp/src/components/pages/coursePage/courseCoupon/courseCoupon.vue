<template>
    <div class="createCouponDialog">
        <div class="text-wrap pt-4 pt-sm-0">
            <div class="dialog-title mb-6" v-t="'coupon_create_title'"></div>
            <div class="inputs-coupon">
                <div class="d-flex">
                  <!-- {{requiredCouponCode}} -->
                  <v-text-field
                      v-model="couponCode"
                      :rules="[requiredCouponCode]"
                      :label="$t('coupon_label_code')"
                      :height="$vuetify.breakpoint.xsOnly ? 50 : 44"
                      class="couponCode me-0 me-sm-4 mb-1 mb-sm-0"
                      type="text"
                      color="#304FFE"
                      placeholder=" "
                      autocomplete="off"
                      dense
                      outlined
                  />
                <!-- </div> -->


                <!-- <v-flex sm3 class="flex-grow-1"> -->
                  <!-- {{requiredCouponAmount}} -->
                    <v-text-field
                      v-model="couponValue"
                      :label="$t('coupon_label_value')"
                      placeholder=" "
                      :rules="[requiredCouponAmount]"
                      :height="$vuetify.breakpoint.xsOnly ? 50 : 44"
                      class="couponValue"
                      autocomplete="off"
                      type="text"
                      color="#304FFE"
                      dense
                      outlined
                    />
                <!-- </v-flex> -->
                </div>

                <div class="d-flex">
                    <v-select
                      v-model="couponType"
                      class="coupon-type me-0 me-sm-4 mb-1 mb-sm-0"
                      :items="couponTypesList"
                      :label="$t('coupon_label_type')"
                      placeholder=" "
                      :append-icon="'sbf-triangle-arrow-down'"
                      :height="$vuetify.breakpoint.xsOnly ? 50 : 44"
                      color="#304FFE"
                      item-text="key"
                      outlined
                      dense
                    >
                      <template slot="item" slot-scope="data">
                        <span class="subtitle-1">{{data.item.key}}</span>
                      </template>
                    </v-select>
                <!-- </div> -->
                
                <!-- <v-flex xs6 pe-2 pe-sm-0 v-if="$vuetify.breakpoint.xsOnly">
                    <v-text-field v-model="couponType" :label="$t('coupon_label_value')"
                    :placeholder="placeHoldersEmpty" autocomplete="nope" :rules="[rules.required,rules.integer,rules.minimum,rules.maximum]"
                    dense color="#304FFE" outlined type="text" :height="$vuetify.breakpoint.xsOnly?50: 44"/>
                </v-flex> -->

                <!-- <v-flex xs6 sm4 ps-2 ps-sm-4> -->
                    <v-menu ref="datePickerMenu" v-model="datePickerMenu" :close-on-content-click="false" transition="scale-transition" offset-y max-width="290px" min-width="290px">
                      <template v-slot:activator="{ on }">
                        <v-text-field
                          v-model="dateFormatted"
                          v-on="on"
                          class="date-input"
                          :label="$t('coupon_label_date')"
                          autocomplete="nope"
                          prepend-inner-icon="sbf-calendar"
                          @blur="date = parseDate(dateFormatted)"
                          dense
                          color="#304FFE"
                          outlined
                          type="text"
                          :height="$vuetify.breakpoint.xsOnly ? 50 : 44"
                        />
                      </template>                  
                      <v-date-picker color="#4C59FF" class="date-picker-coupon" v-model="date" no-title @input="datePickerMenu = false">
                          <v-spacer></v-spacer>
                          <v-btn text class="font-weight-bold" color="#4C59FF" @click="datePickerMenu = false">{{$t('coupon_btn_calendar_cancel')}}</v-btn>
                          <v-btn text class="font-weight-bold" color="#4C59FF" @click="$refs.datePickerMenu.save(date)">{{$t('coupon_btn_calendar_ok')}}</v-btn>
                      </v-date-picker>
                    </v-menu>
                </div>

                <!-- </v-flex> -->
            </div>
        </div>
    </div>
    <!-- <v-snackbar v-model="snackbar" :timeout="3000" top>
      <div class="text-center flex-grow-1">{{ $t('coupon_toaster_copy') }}</div>
    </v-snackbar> -->
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules';

export default {
  data() {
    return {
      couponTypesList:[
        { key: this.$t('coupon_type_flat'),value: 'flat' },
        { key: this.$t('coupon_type_percentage'),value: 'percentage' }
      ],
      maximumValue: Infinity,
      datePickerMenu: false,
      dateFormatted: '',
      rules: {
        required: (value) => validationRules.required(value),
        minimumChars: (value) => validationRules.minimumChars(value, 5),
        maximumChars: (value) => validationRules.maximumChars(value, 12),
        integer: (value) => validationRules.integer(value),
        notSpaces: (value) => validationRules.notSpaces(value),
        minimum: (value) => validationRules.minVal(value,1),
        maximum: (value) => validationRules.maxVal(value, this.maximumValue),
      },
      // snackbar:false,
      // validCreateCoupon: false,
      // couponCode:'',
      // couponType:'',
      // couponValue:'',
      // loadingBtn:false,
      // showSuccess:false,
      // date: new Date().FormatDateToString(),
      // couponErr:''
    }
  },
  watch: {
    couponCode(val){
      let code = val.replace(/\s/g,''); 
      this.$store.commit('setCouponCode', code)
      // this.couponErr = '';
    },
    couponType(val){
      if(val === 'percentage') {
        this.maximumValue = 100;
      } else {
        this.maximumValue = Infinity;
      }
      // if(this.couponValue){
      //   this.$refs.validCreateCoupon.validate()
      // }
    },
    date () {
      this.dateFormatted = this.formatDate(this.date)
    },
  },
  computed: {
    requiredCouponCode() {
      return (val) => {
        if(val || this.$store.getters.getCouponAmount) {
          let rules = [
            validationRules.required(val),
            validationRules.minimumChars(val, 5),
            validationRules.maximumChars(val, 12),
            validationRules.notSpaces(val),
          ]
          let x = rules.filter((r) => r !== true)[0]
          return x || true
        }
        return true
      }
    },
    requiredCouponAmount() {
      return val => {
        if(val || this.$store.getters.getCouponCode) {
          let rules = [
            validationRules.integer(val),
            validationRules.minVal(val, 1),
            validationRules.maxVal(val, this.maximumValue),
          ]
          let x = rules.filter((r) => r !== true)[0]
          return x || true
        }
        return true
      }
    },
    couponCode: {
      get() {
        return this.$store.getters.getCouponCode
      },
      set(code) {
        this.$store.commit('setCouponCode', code)
      }
    },
    couponValue: {
      get() {
        return this.$store.getters.getCouponAmount
      },
      set(amount) {
        this.$store.commit('setCouponAmount', amount)
      }
    },
    couponType: {
      get() {
        return this.$store.getters.getCouponType
      },
      set(type) {
        this.$store.commit('setCouponType', type)
      }
    },
    date: {
      get() {
        return this.$store.getters.getCouponDate
      },
      set(date) {
        this.$store.commit('setCouponDate', date)
      }
    }
  },
  methods: {
    // ...mapActions(['createCoupon']),
    // copyCode(){
    //   let self = this;
    //   this.loadingBtn = true;
    //   this.$copyText(this.couponCode).then(() => {
    //       self.loadingBtn = false;
    //       self.snackbar = true;
    //   })
    // },
    // createMyCoupon() {
    //   if(this.$refs.validCreateCoupon.validate()){
    //     this.loadingBtn = true;
    //     let params = {
    //       value: this.couponValue,
    //       code: this.couponCode,
    //       couponType: this.couponType,
    //       expiration: new Date(this.date).toISOString()
    //     }
    //     let self = this;
    //     this.createCoupon(params).then(()=>{
    //       self.loadingBtn = false;
    //       self.showSuccess = true;
    //       self.couponErr = '';
    //     }).catch(()=>{
    //       self.couponErr = this.$t('coupon_already_exists');
    //       self.loadingBtn = false;
    //     })
    //   }
    // },
    formatDate (date) {
      if (!date) return null
      const [year, month, day] = date.split('-')
      return `${month}/${day}/${year}`
      // return this.$moment(date).format('YYYY/MM/DD')
    },
    parseDate (date) {
      if (!date) return null
      const [month, day, year] = date.split('/')
      return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`
      // let parseDate = new Date(date)
      // return this.$moment(parseDate).format("YYYY-MM-DD")
    },
  },
  mounted() {
    setTimeout(()=>{
      this.dateFormatted = this.formatDate(new Date().FormatDateToString())
    })
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
  .text-wrap {
    color: #43425d;
    height: inherit;
    .dialog-title {
      font-size: 18px;
      font-weight: 600;
    }
    // .succes-title {
    //   font-size: 24px;
    //   font-weight: 600;
    //   letter-spacing: -0.45px;
    //   text-align: center;
    //   max-width: 340px;
    //   margin: 0 auto;
    //   @media (max-width: @screen-xs) {
    //     font-size: 20px;
    //     letter-spacing: -0.38px;
    //   }
    // }
    // .coupon-box {
    //   text-align: center;
    //   position: relative;
    //   border-radius: 4px;
    //   border: dashed 2px #b8c0d1;
    //   padding: 8px 18px 10px;
    //   @media (max-width: @screen-xs) {
    //     padding: 10px 18px;
    //   }
    //   max-width: 256px;
    //   margin: 0 auto;
    //   .coupon-box_img{
    //       transform: scaleX(1) /*rtl:append:scaleX(-1)*/;
    //       position: absolute;
    //       top: -16px;
    //       left: 8px;
    //   }
    //   .coupon-box_code{
    //     font-size: 24px;
    //     font-weight: 600;
    //     text-align: center;
    //     line-height: 1.2;
    //     letter-spacing: 1.2px;
    //   }
    // }
    .inputs-coupon {
      margin: 0 auto;
      @media (max-width: @screen-xs) {
        max-width: 292px;
      }
      .couponCode {
        max-width: 180px;
      }
      .couponValue, .date-input {
        max-width: 160px;
      }
      .date-input {
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
      .coupon-type {
        max-width: 180px;
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
    //   .input-fields {
    //     width: 100%;
    //   }
    }
  }
//   .btns-wrap {
//     text-align: center;
//     .dialog-btn {
//       min-width: 140px;
//       @media (max-width: @screen-xs) {
//         min-width: 120px;
//       }
//       text-transform: capitalize;
//       font-size: 14px;
//       font-weight: 600;
//       &.btn-cancel {
//         color: #4c59ff;
//         border: 1px solid #4c59ff !important;
//       }
//     }
//   }
}
</style>