<template>
   <v-card class="priceD-change-wrap">
      <v-flex align-center justify-center class="relative-pos">
         <div class="title-wrap">
            <span class="change-title pr-1" v-language:inner="'resultNote_change_for'"></span>
            <span class="change-title">&nbsp;"{{dialogData.name}}"</span>
         </div>
         <div class="input-wrap d-flex align-center justify-center">
            <div :class="['price-wrap']">
            <vue-numeric
               :currency="currentCurrency"
               class="sb-input-upload-price"
               :minus="false"
               :min="0"
               :precision="2"
               :max="1000"
               :currency-symbol-position="'suffix'"
               separator=","
               v-model="editedPrice"
            ></vue-numeric>
            </div>
         </div>
      </v-flex>
      <div class="change-price-actions">
         <button @click="$emit('closeDialog')" class="cancel mr-2">
            <span v-language:inner="'resultNote_action_cancel'"/>
         </button>
         <button @click="submitNewPrice()" class="change-price">
            <span v-language:inner="'resultNote_action_apply_price'"/>
         </button>
      </div>
   </v-card>
</template>
<script>
import { LanguageService } from '../../../../services/language/languageService';
import documentService from '../../../../services/documentService.js';
import { validationRules } from '../../../../services/utilities/formValidationRules';

import { mapActions } from 'vuex';

export default {
   name: 'changePriceDialog',
   props:['dialogData'],
   data() {
      return {
         editedPrice: this.dialogData.price,
         validationPrice:false,
         currentCurrency: LanguageService.getValueByKey("app_currency_dynamic"),
         title:LanguageService.getValueByKey('resultNote_change_price'),
         rules:{
            integer: (value) => validationRules.integer(value),
            maximum: (value) => validationRules.maxVal(value, 1000),
         }
      }
   },
   methods: {
      ...mapActions(['dashboard_updatePrice']),
      submitNewPrice() {
         // if(!this.validationPrice)return

         let data = { id: this.dialogData.id, price: this.editedPrice };
         let self = this;
         documentService.changeDocumentPrice(data).then(
            () => {
               self.dashboard_updatePrice({newPrice:self.editedPrice,itemId:self.dialogData.id})
               self.editedPrice = '';
               self.$emit('closeDialog')
            }
         );
      },
   },
}
</script>


<style lang="less">
@import '../../../../styles/mixin.less';
@placeholderGrey: rgba(74, 74, 74, 0.25);
@colorPrice: rgba(74, 74, 74, 0.87);
.relative-pos {
  position: relative;
  .input-wrap {
    position: absolute;
    display: flex;
    justify-content: center;
    align-items: center;
    left: 0;
    bottom: -12px;
    right: 0; 
  }
}
//price-change dialog
.priceD-change-wrap {
   width: 438px;
    padding: 12px 0 0 0;
    position: relative;
    .price-wrap {
      background: @color-white;
      display: flex;
      flex-direction: row;
      align-items: center;
      max-width: 164px;
      min-width: 164px;
      border-radius: 4px;
      border: 1px solid rgba(0, 0, 0, .25);
      justify-content: center;
      &:focus-within {
        .glowingBorderFocused();
      }
      .sb-input-upload-price {
        max-width: 76px;
        height: 44px;
        order: 1;
        outline: none;
        .input-field {
          .placeholder-color(@placeholderGrey, 1);
          font-size: 16px;
          color: @colorPrice;
          text-align: center;
          height: 100%;
          width: 100%;
          &:focus {
            outline: none;
          }
        }
      }
    }
  
  
    .title-wrap {
      padding-top: 16px;
      padding-bottom: 48px;
      display: flex;
      flex-direction: row;
      align-items: center;
      justify-content: center;
      .change-title {
        font-size: 16px;
        letter-spacing: -0.4px;
        color: @textColor;
        .lineClamp()
      }
    }
    .change-price-actions {
      display: flex;
      flex-direction: row;
      align-items: center;
      justify-content: center;
      padding: 32px 0 16px 0;
      // background: #f7f7f7;
      .change-price {
        .sb-rounded-btn();
        padding: 8px 20px 8px 20px;
        font-size: 14px;
        font-weight: 400;
      }
      .cancel {
        border-radius: 16px;
        border: 1px solid #979797;
        color: @colorBlackNew;
        display: flex;
        align-items: center;
        padding: 6px 22px 6px 22px;
        outline: none;
        font-size: 14px;
        font-weight: 600;
        letter-spacing: -0.4px;
      }
    }
  }
</style>