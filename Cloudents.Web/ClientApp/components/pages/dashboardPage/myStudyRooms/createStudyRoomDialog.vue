<template>
   <v-dialog :value="true" persistent max-width="640px" :fullscreen="$vuetify.breakpoint.xsOnly">
      <div class="createStudyRoomDialog">
         <v-icon class="close-dialog" v-text="'sbf-close'" v-closeDialog />
         <div class="createStudyRoomDialog-title pb-4">{{$t('dashboardPage_create_room_title')}}</div>
         <div class="createStudyRoomDialog-list">
            <v-list flat class="list-followers">
               <v-list-item-group>
                  <v-list-item v-for="(item, index) in myFollowers" :key="index"  @click="selected = item">
                     <template v-slot:default="{}">
                        <v-list-item-avatar>
                           <UserAvatar :size="'34'" :user-name="item.name" :user-id="item.id" :userImageUrl="item.image"/> 
                        </v-list-item-avatar>
                        <v-list-item-content>
                           <v-list-item-title>{{item.name}}</v-list-item-title>
                        </v-list-item-content>
                        <v-list-item-action>
                           <v-checkbox v-model="selected" :value="item" off-icon="sbf-check-box-un" on-icon="sbf-check-box-done"></v-checkbox>
                        </v-list-item-action>
                     </template>
                  </v-list-item>
               </v-list-item-group>
            </v-list>
         </div>
         <v-btn :loading="isLoading" @click="createStudyRoom" width="140" depressed height="40" color="#4452fc" class="white--text" rounded >{{$t('dashboardPage_create_room_create_btn')}}</v-btn>
      </div>
   </v-dialog>
</template>

<script>
export default {
   name:'createStudyRoom',
   data() {
      return {
         isLoading:false,
         myFollowers:[],
         selected:'',
      }
   },
   methods: {
      createStudyRoom(){
         if(!this.isLoading && this.selected){
            this.isLoading = true
            let self = this;
            this.$store.dispatch('updateCreateStudyRoom',this.selected)
               .then(() => {
                  self.isLoading = false;
                  self.$closeDialog()
               }).catch(()=>{
                  self.isLoading = false;
               });
         }
      }
   },
   created() {
      this.$store.dispatch('updateFollowersItems').then(()=>{
         this.myFollowers = this.$store.getters.getFollowersItems
      })
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.createStudyRoomDialog{
   background: white;
   position: relative;
   padding: 10px;
   height: 450px;
   display: flex;
   flex-direction: column;
   align-items: center;
   justify-content: space-between;
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
   .createStudyRoomDialog-list{
      width: 100%;
      .list-followers{
         max-height: 300px;
         overflow-y: scroll;
      }
   }
}
</style>