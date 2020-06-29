<template>
  <v-dialog
    :value="true"
    persistent
    :maxWidth="'296'"
    :content-class="'registerToJoinDialog'">
    <div>
       <v-icon @click="onClose" size="12" class="closeIcon">sbf-close</v-icon>
       <p class="px-0 pb-6 ma-0 content">{{$t('register_to_join')}}</p>
       <v-btn @click="onRegister" block
         color="#4c59ff" height="44" depressed rounded class="white--text">
         {{$t('register_free')}}
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
         this.$store.commit('setComponent',componentConts.REGISTER)
      }
   },
};
</script>

<style lang="less">
.registerToJoinDialog {
   border-radius: 6px;
   background: white;
   box-shadow: 0 13px 21px 0 rgba(0, 0, 0, 0.51);
   padding: 16px;
   padding-top: 36px;
   text-align: center;
   position: relative;
   .closeIcon{
      position: absolute;
      right: 10px;
      top: 10px;
   }
   .content{
      color: #43425d;
      font-size: 18px;
      font-weight: 600;
      line-height: 1.44;
   }
}
</style>