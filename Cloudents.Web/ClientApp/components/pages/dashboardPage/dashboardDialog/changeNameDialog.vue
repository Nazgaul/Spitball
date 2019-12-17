<template>
  <v-form v-model="validationName"  class="changeNameDialog">
     <v-layout align-center text-xs-center row wrap pa-2>
        <v-flex xs12>
           <h1>{{title}}</h1>
        </v-flex>
         <v-flex xs12>
           <v-text-field v-model="editedName" :rules="[rules.required,rules.minimumChars,rules.maximumChars]"/>
        </v-flex>
        <v-flex xs12>
           <v-btn :disabled="!validationName" @click="submitNewName" color="success">done</v-btn>
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
   name: 'changeNameDialog',
   props:['dialogData'],
   data() {
      return {
         validationName:false,
         editedName: this.dialogData.name,
         title:LanguageService.getValueByKey('dashboardPage_rename'),
         rules:{
            required: (value) => validationRules.required(value),
            minimumChars: (value) => validationRules.minimumChars(value, 4),
            maximumChars: (value) => validationRules.maximumChars(value, 140),
         }
      }
   },
   methods: {
      ...mapActions(['dashboard_updateName']),
      submitNewName() {
         if(!this.validationName)return
         let data = { documentId: this.dialogData.id, name: this.editedName };
         let self = this;
         documentService.changeDocumentName(data).then(
            success => {
               self.dashboard_updateName({newName:self.editedName,itemId:self.dialogData.id})
               self.editedName = '';
               self.$emit('closeDialog')
            },
            error => {
               console.error("erros change name", error);
            }
         );
      },
   }, 
}
</script>

<style lang="less">
.changeNameDialog{
   width: 100%;
}
</style>