<template>
  <v-form v-model="validationPrice" class="changePriceDialog">
     <v-layout align-center justify-center text-xs-center row wrap pa-2>
        <v-flex xs12>
           <h1>{{title}}</h1>
        </v-flex>
         <v-flex xs4>       
           <v-text-field v-model="editedPrice" :rules="[rules.integer,rules.maximum]"/>
        </v-flex>
        <v-flex xs12>
           <v-btn :disabled="!validationPrice" @click="submitNewPrice" color="success">done</v-btn>
           <v-btn color="error" @click="$emit('closeDialog')">cancel</v-btn>
        </v-flex>
     </v-layout>
  </v-form>
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
         if(!this.validationPrice)return

         let data = { id: this.dialogData.id, price: this.editedPrice };
         let self = this;
         documentService.changeDocumentPrice(data).then(
            success => {
               self.dashboard_updatePrice({newPrice:self.editedPrice,itemId:self.dialogData.id})
               self.editedPrice = '';
               self.$emit('closeDialog')
            },
            error => {
               console.error("erros change price", error);
            }
         );
      },
   },
}
</script>

<style lang="less">
.changePriceDialog{
   width: 100%;
}
</style>