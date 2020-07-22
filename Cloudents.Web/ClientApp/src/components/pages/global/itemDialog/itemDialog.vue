<template>
  <v-dialog :value="true" content-class="itemDialog" :fullscreen="$vuetify.breakpoint.xsOnly"
    persistent :overlay="false" max-width="1000px" transition="dialog-transition">

      <v-toolbar color="#4452fc" flat>
        <h1 class="ps-sm-4 text-center text-sm-left text-truncate titleText">{{getDocumentName}}</h1>
        <v-spacer></v-spacer>
        <v-icon absolute right color="white" @click="$emit('close')" size="14" v-text="'sbf-close'"/> 
      </v-toolbar>

      <item2 class="pa-5" :id="id"/>
  </v-dialog>
</template>

<script>
import item2 from '../../itemPage/item2.vue';
import storeService from "../../../../services/store/storeService";
import studyDocumentsStore from "../../../../store/studyDocuments_store";
import { mapGetters } from 'vuex';

export default {
  components:{
    item2
  },
  props:{
    id:{}
  },
  computed: {
    ...mapGetters(['getDocumentName'])
  },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.itemDialog{
  background: white;
  position: relative;
  width: 100%;
  .titleText{
    font-size: 18px;
    font-weight: 600;
    color: #FFF;
  }
}
</style>