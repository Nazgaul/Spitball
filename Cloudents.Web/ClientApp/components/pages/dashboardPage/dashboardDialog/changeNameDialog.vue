<template>
<v-form v-model="validationName" class="changeNameDialog">
   <v-card class="name-change-wrap">
      <v-flex align-center justify-center class="relative-pos">
         <div class="title-wrap">
            <span class="change-title pr-1" v-language:inner="'dashboardPage_rename'"></span>
         </div>
         <div class="input-wrap d-flex row align-center justify-center">
            <div :class="['name-wrap']">
            <v-text-field class="sb-input-upload-name" v-model="editedName" :rules="[rules.required,rules.minimumChars,rules.maximumChars]"/>
            </div>
         </div>
      </v-flex>
      <div class="change-name-actions">
         <button @click="$emit('closeDialog')" class="cancel mr-2">
            <span v-language:inner="'resultNote_action_cancel'"/>
         </button>
         <button @click.prevent="submitNewName()" class="change-name">
            <span v-language:inner="'resultNote_action_apply_name'"/>
         </button>
      </div>
   </v-card>
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
@import '../../../../styles/mixin.less';
@placeholderGrey: rgba(74, 74, 74, 0.25);
@colorName: rgba(74, 74, 74, 0.87);
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
.name-change-wrap {
    padding: 12px 0 0 0;
    position: relative;
    .name-wrap {
      background: @color-white;
      display: flex;
      flex-direction: row;
      align-items: center;
      max-width: 300px;
      min-width: 300px;
      border-radius: 4px;
      border: 1px solid rgba(0, 0, 0, .25);
      justify-content: center;
      &:focus-within {
        .glowingBorderFocused();
      }

      .v-text-field {
         margin-top: 0px !important;
         padding-top: 8px !important;
         .v-input__slot:before {
            border: none;
         }
         .v-input__slot:after {
            border: none;
         }
      }
      .sb-input-upload-name {
        height: 44px;
        order: 1;
        outline: none;
        .input-field {
          .placeholder-color(@placeholderGrey, 1);
          font-size: 16px;
          color: @colorName;
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
    .change-name-actions {
      display: flex;
      flex-direction: row;
      align-items: center;
      justify-content: center;
      padding: 32px 0 16px 0;
      background: #f7f7f7;
      .change-name {
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