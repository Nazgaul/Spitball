<template>
  <v-dialog
    :value="true"
    persistent
    :maxWidth="'450'"
    :content-class="'registerToJoinDialog'">
    <div>
       <v-icon @click="onClose" size="14" class="closeIcon">sbf-close</v-icon>
       <p class="py-5 px-5 content">{{$t('register_to_join')}}</p>
       <v-btn @click="onRegister"
         color="#4c59ff" depressed rounded class="white--text">
         {{$t('register')}}
      </v-btn>
    </div>
  </v-dialog>
</template>

<script>
import * as componentConts from '../../../pages/global/toasterInjection/componentConsts.js';
export default {
   methods: {
      onClose(){
         this.$emit('closeRegisterToJoin')
      },
      onRegister(){
         this.onClose()
         this.$store.commit('setComponent',componentConts.LOGIN)
      }
   },
};
</script>

<style lang="less">
.registerToJoinDialog {
   background: white;
   padding: 14px;
   text-align: center;
   position: relative;
   .closeIcon{
      position: absolute;
      right: 10px;
      top: 10px;
   }
   .content{
      font-size: 16px;
      color: #43425d;
   }
}
</style>