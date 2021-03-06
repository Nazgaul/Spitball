<template>
   <v-dialog :value="true" persistent max-width="620px" :fullscreen="$vuetify.breakpoint.xsOnly" content-class="createStudyRoomDialog privateHeight">
      <div class="createRoomWrapper px-sm-7 px-4 py-4 d-sm-block d-flex flex-column justify-space-between">

         <v-form class="justify-space-between input-room-name mb-3" ref="createRoomValidation">
            <v-icon class="close-dialog" v-text="'sbf-close'" @click="closeDialog()" />
            <div class="createStudyRoomDialog-title text-center mb-7 pb-3">{{$t('dashboardPage_create_room_private_title')}}</div>

            <Private :price="price"
               :currentError="currentError"
               @updateError="updateError"
               @resetErrors="resetErrors"
               @updatePrice="val => price = val"
               ref="childComponent"/>
         </v-form>

         <div class="d-flex flex-column align-center pt-4">
            <div class="mb-4">
               <span v-if="currentError" class="error--text" v-t="errorsResource[currentError]"></span>
            </div>
            <v-btn :loading="isLoading" @click="createStudyRoom" width="200" depressed height="40" color="#4c59ff" class="white--text createBtn" rounded >{{$t('dashboardPage_create_private')}}</v-btn>
         </div>
      </div>
   </v-dialog>
</template>

<script>
import { CREATE_BROADCAST_ERROR ,SESSION_CREATE_DIALOG } from '../../global/toasterInjection/componentConsts'
const Private = () => import('./privateSession/privateSession.vue');

export default {
   name:'createStudyRoom',
   components: {
      Private
   },
   props: {
      params: {}
   },
   data() {
      return {
         isLoading: false,
         errors: {
            showErrorEmpty: false,
            showErrorMaxUsers: false,
            showErrorWrongTime: false,
            showErrorAlreadyCreated: false,
            showErrorWrongNumber: false
         },

         currentError: '',
         price: 0
      }
   },
   computed: {
      errorsResource() {
         return {
            showErrorEmpty: this.$t('dashboardPage_create_room_empty_error'),
            showErrorAlreadyCreated: this.$t('dashboardPage_create_room_created_error'),
            showErrorMaxUsers: this.$t('dashboardPage_create_room_max_error'),
            showErrorWrongTime: this.$t('dashboardPage_pick_time_error'),
            showErrorWrongNumber: this.$t('not number')
         }
      },
      isNoErrors() {
         return !this.errors.showErrorAlreadyCreated && !this.errors.showErrorEmpty &&
                !this.errors.showErrorMaxUsers && !this.errors.showErrorWrongTime && !this.errors.showErrorWrongNumber
      }
   },
   methods: {
      closeDialog(){
         this.$store.commit('removeComponent',SESSION_CREATE_DIALOG);  
      },
      createStudyRoom(){
         let form = this.$refs.createRoomValidation
         if(!form.validate()) return

         if(!this.isLoading && this.isNoErrors){
            this.isLoading = true
            this.createPrivateSession()
         }
      },
      createPrivateSession() {
         let childComponent = this.$refs.childComponent
         if(!childComponent.selected.length) {
            this.errors.showErrorEmpty = true
            this.currentError = 'showErrorEmpty'
            this.isLoading = false
            return
         }

         let privateObj = {
            userId: Array.from(childComponent.selected.map(user=> user.userId)),
            name: childComponent.roomName,
            price: childComponent.price,
            currency: this.$store.getters.accountUser.currencySymbol,
         }

         let self = this
         this.$store.dispatch('updateCreateStudyRoomPrivate', privateObj)
            .then(()=>{
               self.closeDialog();           
            })
            .catch((error) => {
               self.handleCreateError(error);
            })
            .finally(() => {
               self.isLoading = false;
            })
      },
      handleCreateError(error) {
         if(error.response?.status == 409){
            this.errors.showErrorAlreadyCreated = true
            this.currentError = 'showErrorAlreadyCreated'
            return
         }
         this.$store.commit('setComponent', CREATE_BROADCAST_ERROR)
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
         this.errors.showErrorWrongNumber = false
         this.currentError = ''
      }
   },
   created() {
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

   &::-webkit-scrollbar-track {
     box-shadow: inset 0 0 6px rgba(0,0,0,0.3); 
    border-radius: 10px;
   }
   &::-webkit-scrollbar {
      width: 12px;
   }
   &::-webkit-scrollbar-thumb {
      border-radius: 10px;
      box-shadow: inset 0 0 6px rgba(0,0,0,0.5); 
   }
   &.privateHeight {
      .createRoomWrapper {
         @media (max-width: @screen-xs) {
            height: 100%;
         }  
      }
   }
   .close-dialog {
      position: absolute;
      right: 12px;
      font-size: 12px;
   }
   .createStudyRoomDialog-title {
      color: @global-purple;
      font-size: 22px;
      font-weight: 600;
      // padding-bottom: 34px;
      border-bottom: 1px solid #dddddd;
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
   .createBtn {
      font-size: 16px;
   }
}
   .v-picker__title {
      .v-time-picker-title__time {
         direction: ltr /*rtl: ltr*/ !important;
      }
   }
</style>