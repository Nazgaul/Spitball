<template>
   <v-dialog :value="true" persistent max-width="640px" :fullscreen="$vuetify.breakpoint.xsOnly">
      <div class="createStudyRoomDialog">
         <v-icon class="close-dialog" v-text="'sbf-close'" v-closeDialog />
         <div class="createStudyRoomDialog-title pb-4">{{$t('dashboardPage_create_room_title')}}</div>
         <v-form class="d-flex justify-space-between input-room-name" ref="createRoomValidation">
            <v-text-field class="pr-5" :rules="[rules.required]" v-model="roomName" height="44" dense outlined :label="$t('dashboardPage_create_room_placeholder')" :placeholder="$t('dashboardPage_create_room_label')"/>
            <v-text-field class="pl-5" outlined  height="44" dense :rules="[rules.required,rules.integer,rules.minimum]"
               v-model="price" type="number"
               :label="$t('becomeTutor_placeholder_price', {'0' : getSymbol})" :placeholder="$t('becomeTutor_placeholder_price', {'0' : getSymbol})">
            </v-text-field>
         </v-form>
         <div class="createStudyRoomDialog-list">
            <v-list flat class="list-followers">
               <v-list-item-group>
                  <v-list-item v-for="(item, index) in myFollowers" :key="index" @click="addSelectedUser(item)" :class="[{'dark-line': index % 2}]">
                     <template v-slot:default="{}">
                        <v-list-item-avatar>
                           <UserAvatar :size="'40'" :user-name="item.name" :user-id="item.id" :userImageUrl="item.image"/> 
                        </v-list-item-avatar>
                        <v-list-item-content>
                           <v-list-item-title>{{item.name}}</v-list-item-title>
                        </v-list-item-content>
                        <v-list-item-action>
                           <v-checkbox @click.prevent multiple v-model="selected" :value="item" color="#4c59ff" off-icon="sbf-check-box-un" on-icon="sbf-check-box-done"></v-checkbox>
                        </v-list-item-action>
                     </template>
                  </v-list-item>
               </v-list-item-group>
            </v-list>
         </div>
         <div class="d-flex flex-column align-center pt-4">
            <span v-if="showErrorEmpty" class="error--text">{{$t('dashboardPage_create_room_empty_error')}}</span>
            <span v-if="showErrorAlreadyCreated" class="error--text">{{$t('dashboardPage_create_room_created_error')}}</span>
            <span v-if="showErrorMaxUsers" class="error--text">{{$t('dashboardPage_create_room_max_error')}}</span>
            <v-btn :loading="isLoading" @click="createStudyRoom" width="150" depressed height="40" color="#4452fc" class="white--text" rounded >{{$t('dashboardPage_create_room_create_btn')}}</v-btn>
         </div>
      </div>
   </v-dialog>
</template>

<script>
import {validationRules} from '../../../../services/utilities/formValidationRules.js'
export default {
   name:'createStudyRoom',
   data() {
      return {
         showErrorMaxUsers:false,
         isLoading:false,
         myFollowers:[],
         selected:[],
         showErrorEmpty:false,
         showErrorAlreadyCreated:false,
         roomName:'',
         price: '0',
         // date:'',
         rules: {
            required: (value) => validationRules.required(value),
            minimum: (value) => validationRules.minVal(value,0),
         },
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
            if(this.selected.length < 4){
               this.selected.push(user)
            }else{
               this.showErrorMaxUsers = true;
            }
         }
      },
      createStudyRoom(){
         if(!this.$refs.createRoomValidation.validate()) return
         if(!this.isLoading && !this.showErrorAlreadyCreated && !this.showErrorEmpty && !this.showErrorMaxUsers){
            if(this.selected.length){
               let paramsObj = {
                  name: this.roomName,
                  userId: Array.from(this.selected.map(user=> user.userId)),
                  price: this.price || 0,
                  // date: this.date || new Date().toISOString(),
                  // date: '2020-04-06T14:01:54.339Z'
               }
               this.isLoading = true
               let self = this;
               this.$store.dispatch('updateCreateStudyRoom',paramsObj)
                  .then(() => {
                     self.isLoading = false;
                     self.$closeDialog()
                  }).catch((error)=>{
                     self.isLoading = false;
                     if(error.response?.status == 409){
                        self.showErrorAlreadyCreated = true;
                     }
                  });
            }
            else{
               this.showErrorEmpty = true;
            }
         }
      }
   },
   watch: {
      selected(){
         this.showErrorEmpty = false;
         this.showErrorAlreadyCreated = false;
         this.showErrorMaxUsers = false;
      }
   },
   computed: {
      getSymbol() {
         let v =   this.$n(1,'currency');
         return v.replace(/\d|[.,]/g,'').trim();
      },
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
   height: 520px;
   display: flex;
   flex-direction: column;
   align-items: center;
   justify-content: space-between;
   padding-left: 0;
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
   .input-room-name{
      width: 500px;
      // width: 216px;
      .v-text-field__details{
         margin-bottom: 0;
      }
   }
   .createStudyRoomDialog-list{
      width: 100%;
      .list-followers{
         max-height: 320px;
         overflow-y: scroll;
         .v-item-group {
            padding-right: 6px;
         }
         .dark-line{
            background: #f5f5f5;
         }
      }
   }
}
</style>