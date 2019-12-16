<template>
   <div class="dashboardPage">
      <component :sortFunction="sortFunction" @openDialog="openDialog" :is="currentComponentByRoute"></component>
      <sb-dialog 
         :showDialog="isDialog"
         :popUpType="'dashboardDialog'"
         :onclosefn="closeDialog"
         :activateOverlay="true"
         :max-width="'550px'"
         :content-class="'pop-dashboard-container'">
            <changeNameDialog v-if="currentDialog === 'rename'" :dialogData="dialogData" @closeDialog="closeDialog"/>
            <changePriceDialog v-if="currentDialog === 'changePrice'" :dialogData="dialogData" @closeDialog="closeDialog"/>
      </sb-dialog>
   </div>
</template>

<script>
import mySales from './mySales/mySales.vue';
import myContent from './myContent/myContent.vue';

import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue';
import changeNameDialog from './dashboardDialog/changeNameDialog.vue';
import changePriceDialog from './dashboardDialog/changePriceDialog.vue';

export default {
   name:'dashboardPage',
   data() {
      return {
         currentDialog:'',
         isDialog:false,
         dialogData:'',
      }
   },
   components:{
      mySales,
      myContent,
      changeNameDialog,
      changePriceDialog,
      sbDialog
   },
   computed:{
      currentComponentByRoute(){
         return this.$route.path.slice(1);
      }
   },
   methods: {
      closeDialog() {
         this.currentDialog = '';
         this.dialogData = ''
         this.isDialog = false;
      },
      openDialog(args){
         this.currentDialog = args[0];
         this.dialogData = args[1]
         this.isDialog = true;
      },
      sortFunction(list,sortBy,sortedBy){
         if(sortBy == 'date'){
            if(sortedBy === sortBy){
              return list.reverse()
            }else{
               return list.sort((a,b)=> new Date(b[sortBy]) - new Date(a[sortBy]))
            }
         }
         if(sortedBy === sortBy){
            return list.reverse()
         }else{
            return list = list.sort((a,b)=> {
               if(a[sortBy] == undefined) return 1;
               if(b[sortBy] == undefined) return -1;

               if(a[sortBy] > b[sortBy])return -1;
               if(b[sortBy] > a[sortBy])return 1;
               return 0;
            })
         }
      }
   }

}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
.dashboardPage{
	padding-left: 30px;
	padding-top: 30px;
   max-width: 1150px;
	@media (max-width: @screen-xs) {
		padding-left: 0;
      width: 100%;
      height: 100%;
   }
}
.pop-dashboard-container {
   background: #fff;
}

</style>
