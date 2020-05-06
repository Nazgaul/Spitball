<template>
   <v-dialog :value="true" persistent max-width="640px" :fullscreen="$vuetify.breakpoint.xsOnly">
      <div class="createStudyRoomDialog pa-4 d-sm-block d-flex flex-column justify-space-between">

         <v-form class="justify-space-between input-room-name mb-3" ref="createRoomValidation">
            <v-icon class="close-dialog" v-text="'sbf-close'" @click="$store.commit('setComponent')" />
            <div class="createStudyRoomDialog-title text-center" v-t="createSessionTitle"></div>

            <component
               :is="studyRoomType"
               :price="price"
               :currentError="currentError"
               @updateError="updateError"
               @resetErrors="resetErrors"
               @updatePrice="val => price = val"
               ref="childComponent"
            >
            </component>
         </v-form>

         <div class="d-flex flex-column align-center pt-4">
            <div class="mb-4">
               <span v-if="currentError" class="error--text" v-t="errorsResource[currentError]"></span>
               <!-- <span v-if="errors.showErrorEmpty" class="error--text" v-t="'dashboardPage_create_room_empty_error'"></span>
               <span v-if="errors.showErrorAlreadyCreated" class="error--text" v-t="'dashboardPage_create_room_created_error'"></span>
               <span v-if="errors.showErrorMaxUsers" class="error--text" v-t="'dashboardPage_create_room_max_error'"></span>
               <span v-if="errors.showErrorWrongTime" class="error--text" v-t="'dashboardPage_pick_time_error'"></span> -->
            </div>
            <v-btn :loading="isLoading" @click="createStudyRoom" width="160" depressed height="40" color="#4452fc" class="white--text" rounded >{{$t(btnCreateText)}}</v-btn>
         </div>
      </div>
   </v-dialog>
</template>

<script>
import Broadcast from './liveSession/liveSession.vue'
import Private from './privateSession/privateSession.vue'

export default {
   name:'createStudyRoom',
   components: {
      Broadcast,
      Private
   },
   props: {
      params: {}
   },
   data() {
      return {
         studyRoomType: '',
         isLoading: false,
         errors: {
            showErrorEmpty: false,
            showErrorMaxUsers: false,
            showErrorWrongTime: false,
            showErrorAlreadyCreated: false
         },
         errorsResource: {
            showErrorEmpty: 'dashboardPage_create_room_empty_error',
            showErrorAlreadyCreated: 'dashboardPage_create_room_created_error',
            showErrorMaxUsers: 'dashboardPage_create_room_max_error',
            showErrorWrongTime: 'dashboardPage_pick_time_error',
         },
         currentError: '',
         price: 0
      }
   },
   computed: {
      isPrivate() {
         return this.studyRoomType === 'private'
      },
      btnCreateText() {
         return this.isPrivate ? 'dashboardPage_create_private' : 'dashboardPage_create_broadcast'
      },
      createSessionTitle() {
         return this.isPrivate ? 'dashboardPage_create_room_private_title' : 'dashboardPage_create_room_live_title'
      },
      isNoErrors() {
         return !this.errors.showErrorAlreadyCreated && !this.errors.showErrorEmpty &&
                !this.errors.showErrorMaxUsers && !this.errors.showErrorWrongTime
      }
   },
   methods: {
      createStudyRoom(){
         let params
         let form = this.$refs.createRoomValidation
         if(!form.validate()) return

         if(!this.isLoading && this.isNoErrors){

            if(this.isPrivate) {
               params = this.createPrivateSession()
            } else {
               params = this.createLiveSession()
            }

            if(params === false) return

            params.type = this.studyRoomType

            this.isLoading = true
            let self = this
            this.$store.dispatch('updateCreateStudyRoom', params)
               .then(() => {
                  self.$store.commit('setComponent')
               }).catch((error) => {
                  console.log(error)
                  if(error.response?.status == 409){
                     self.errors.showErrorAlreadyCreated = true
                     self.currentError = 'showErrorAlreadyCreated'
                  }
               }).finally(() => {
                  self.isLoading = false
               })
         }
      },
      createPrivateSession() {
         let childComponent = this.$refs.childComponent

         if(!childComponent.selected.length) {
            this.errors.showErrorEmpty = true
            this.currentError = 'showErrorEmpty'
            return false
         }

         return {
            userId: Array.from(childComponent.selected.map(user=> user.userId)),
            name: childComponent.roomName,
            price: childComponent.newPrice || 0,
         }
      },
      createLiveSession() {
         // TODO: new date format verify
         
         let childComponent = this.$refs.childComponent
         let userChooseDate = new Date(`${childComponent.date} ${childComponent.hour}`)
         let today = new Date()
         if(childComponent.date === today.FormatDateToString()) {
            
            if(userChooseDate.getHours() < today.getHours()) {
               this.errors.showErrorWrongTime = true
               this.currentError = 'showErrorWrongTime'
               return false
            }

            if(userChooseDate.getHours() === today.getHours()) {
               let isWrongMinutes = today.getMinutes() < Number(userChooseDate.getMinutes())
               if(!isWrongMinutes) {
                  this.errors.showErrorWrongTime = true
                  this.currentError = 'showErrorWrongTime'
                  return false
               }
            }
         }



         return {
            date: userChooseDate,
            name: childComponent.liveSessionTitle || '',
            about: childComponent.sessionAboutText || '',
            price: childComponent.newPrice || 0,
         }
      },
      updateError(error) {
         this.currentError = error
         this.errors[error] = true
      },
      resetErrors() {
         this.errors.showErrorEmpty = false
         this.errors.showErrorAlreadyCreated = false
         this.errors.showErrorMaxUsers = false
         this.errors.showErrorWrongTime = false
         this.currentError = ''
      }
   },
   created() {
      this.studyRoomType = this.params.type
      this.price = this.$store.getters.accountUser.price
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.createStudyRoomDialog{
   background: white;
   position: relative;
   height: 100%;
   .close-dialog {
      position: absolute;
      right: 12px;
      font-size: 12px;
   }
   .createStudyRoomDialog-title {
      color: @global-purple;
      font-size: 20px;
      font-weight: 600;
      padding-bottom: 34px;
   }
   .input-room-name{
      width: 100%;
      .v-text-field__details{
         margin-bottom: 0;
      }
      .roomName {
         ::placeholder {
            color: @global-purple;
         }
         :-ms-input-placeholder { /* Internet Explorer 10-11 */
            color: @global-purple;
         }
      }

   }

   .createStudyRoomDialog-list{
      width: 100%;
      .listTitle {
         font-size: 18px;
         font-weight: 600;
         color: @global-purple;
      }
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
   .v-picker__title {
      .v-time-picker-title__time {
         direction: ltr /*rtl: ltr*/ !important;
      }
   }
</style>