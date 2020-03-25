<template>
   <v-dialog :value="true" persistent max-width="640px" :fullscreen="$vuetify.breakpoint.xsOnly">
      <div class="createStudyRoomDialog">
         <v-icon class="close-dialog" v-text="'sbf-close'" v-closeDialog />
         <div class="createStudyRoomDialog-title pb-4">Choose a student</div>
         <div class="createStudyRoomDialog-select">
            <v-select
               :append-icon="'sbf-arrow-down'"
               outlined
               :items="myFollowers"
               placeholder="Please choose a student"
               v-model="selectedStudent"
               clearable
               :clear-icon="'sbf-close'">
               <template slot="selection" slot-scope="data">
                  <div class="d-flex align-center">
                     <UserAvatar class="mr-4" :size="'34'" :user-name="data.item.name" :user-id="data.item.id" :userImageUrl="data.item.image"/> 
                     <div>{{data.item.name}}</div>
                  </div>
               </template>
               <template slot="item" slot-scope="item">
                  <div class="d-flex align-center">
                     <UserAvatar class="mr-4" :size="'34'" :user-name="item.item.name" :user-id="item.item.id" :userImageUrl="item.item.image"/> 
                     <div>{{item.item.name}}</div>
                  </div>
               </template>
            </v-select>
         </div>
         <v-btn :loading="isLoading" @click="createStudyRoom" width="140" depressed height="40" color="#4452fc" class="white--text" rounded >create</v-btn>
      </div>
   </v-dialog>
</template>

<script>
export default {
   name:'createStudyRoom',
   data() {
      return {
         selectedStudent:null,
         isLoading:false,
      }
   },
   methods: {
      createStudyRoom(){
         if(!this.isLoading){
            this.isLoading = true
            let self = this;
            this.$store.dispatch('updateCreateStudyRoom',this.selectedStudent.userId)
               .then(() => {
                  self.isLoading = false;
                  this.$store.dispatch('updateFollowersItems')
                  self.$closeDialog()
               });
         }
      }
   },
   computed: {
      myFollowers(){
         return this.$store.getters.getFollowersItems;
      },
   },
   created() {
      this.$store.dispatch('updateFollowersItems')
   },

}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.createStudyRoomDialog{
   background: white;
   position: relative;
   padding: 10px;
   text-align: center;
   .close-dialog {
      cursor: pointer;
      position: absolute;
      right: 0;
      font-size: 12px;
      padding-top: 6px;
      padding-right: 16px;
   }
   .createStudyRoomDialog-title{
      color: @global-purple;
      font-size: 20px;
      font-weight: 600;
   }
   .createStudyRoomDialog-select{
      margin: 0 auto;
      max-width: 400px;
      .v-input__icon--clear{
         button{
            font-size: 10px;
            color: #43425d;
         }
      }
   }
}
</style>