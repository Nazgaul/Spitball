<template>
   <v-dialog :value="true" persistent max-width="640px" :fullscreen="$vuetify.breakpoint.xsOnly">
      <div class="createStudyRoomDialog">
         <v-icon class="close-dialog" v-text="'sbf-close'" v-closeDialog />
         <div class="createStudyRoomDialog-title pb-4">{{$t('dashboardPage_create_room_title')}}</div>
         <div class="createStudyRoomDialog-list">
            <v-list flat class="list-followers">
               <v-list-item-group>
                  <v-list-item v-for="(item, index) in myFollowers" :key="index" @click="addSelectedUser(item)">
                     <template v-slot:default="{}">
                        <v-list-item-avatar>
                           <UserAvatar :size="'34'" :user-name="item.name" :user-id="item.id" :userImageUrl="item.image"/> 
                        </v-list-item-avatar>
                        <v-list-item-content>
                           <v-list-item-title>{{item.name}}</v-list-item-title>
                        </v-list-item-content>
                        <v-list-item-action>
                           <v-checkbox @click.prevent multiple v-model="selected" :value="item" off-icon="sbf-check-box-un" on-icon="sbf-check-box-done"></v-checkbox>
                        </v-list-item-action>
                     </template>
                  </v-list-item>
               </v-list-item-group>
            </v-list>
         </div>
         <div class="d-flex flex-column align-center">
            <span v-if="showErrorEmpty" class="error--text">{{$t('dashboardPage_create_room_empty_error')}}</span>
            <span v-if="showErrorAlreadyCreated" class="error--text">{{$t('dashboardPage_create_room_created_error')}}</span>
            <v-btn :loading="isLoading" @click="createStudyRoom" width="140" depressed height="40" color="#4452fc" class="white--text" rounded >{{$t('dashboardPage_create_room_create_btn')}}</v-btn>
         </div>
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
         selected:[],
         showErrorEmpty:false,
         showErrorAlreadyCreated:false,
      }
   },
   methods: {
      addSelectedUser(user){
         let idx;
         let isInList = this.selected.some((u,i)=>{
            idx = i;
            return u.userId === user.userId;
         })
         if(isInList){
            this.selected.splice(idx,1);
         }else{
            this.selected.push(user)
         }
      },
      createStudyRoom(){
         if(!this.isLoading && !this.showErrorAlreadyCreated && !this.showErrorEmpty){
            if(this.selected.length){
               this.isLoading = true
               let self = this;
               this.$store.dispatch('updateCreateStudyRoom',this.selected)
                  .then(() => {
                     self.isLoading = false;
                     self.$closeDialog()
                  }).catch((error)=>{
                     self.isLoading = false;
                     if(error.response?.status == 409){
                        self.showErrorAlreadyCreated = true;
                     }
                  });
            }else{
               this.showErrorEmpty = true;
            }
         }
      }
   },
   watch: {
      selected(){
         this.showErrorEmpty = false;
         this.showErrorAlreadyCreated = false;
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