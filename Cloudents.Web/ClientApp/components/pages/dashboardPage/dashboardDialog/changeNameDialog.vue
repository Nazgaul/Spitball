<template>
  <div class="changeNameDialog">
     <v-layout align-center text-xs-center row wrap pa-2>
        <v-flex xs12>
           <h1>{{title}}</h1>
        </v-flex>
         <v-flex xs12>
           <v-text-field v-model="name"/>
        </v-flex>
        <v-flex xs12>
           <v-btn color="success">done</v-btn>
           <v-btn color="error" @click="$emit('closeDialog')">cancel</v-btn>
        </v-flex>
     </v-layout>
  </div>
</template>
<script>
import { LanguageService } from '../../../../services/language/languageService';

export default {
   name: 'changeNameDialog',
   props:['dialogData'],
   data() {
      return {
         editedName:'',
         title:LanguageService.getValueByKey('dashboardPage_rename'),
      }
   },
   computed: {
      name:{
         get(){
            this.editedName = this.dialogData.name;
            return this.dialogData.name;
         },
         set(val){
            this.editedName = val;
         }
      }
   },
}
</script>

<style lang="less">
.changeNameDialog{
   width: 100%;
}
</style>