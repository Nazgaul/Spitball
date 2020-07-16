<template>
   <v-dialog :value="true" persistent max-width="620px" :fullscreen="$vuetify.breakpoint.xsOnly" :content-class="[ isPrivate ? 'createStudyRoomDialog privateHeight' : 'createStudyRoomDialog liveHeight']">
      <div class="createRoomWrapper px-sm-7 px-4 py-4 d-sm-block d-flex flex-column justify-space-between">

         <v-form class="justify-space-between input-room-name mb-3" ref="createRoomValidation">
            <v-icon class="close-dialog" v-text="'sbf-close'" @click="$store.commit('setComponent')" />
            <div class="createStudyRoomDialog-title text-center mb-7 pb-3">{{createSessionTitle}}</div>

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
            </div>
            <v-btn :loading="isLoading" @click="createStudyRoom" width="200" depressed height="40" color="#4c59ff" class="white--text createBtn" rounded >{{btnCreateText}}</v-btn>
         </div>
      </div>
   </v-dialog>
</template>

<script>
const Broadcast = () => import('./liveSession/liveSession.vue');
const Private = () => import('./privateSession/privateSession.vue');

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
      isPrivate() {
         return this.studyRoomType === 'private'
      },
      btnCreateText() {
         return this.isPrivate ? this.$t('dashboardPage_create_private') : this.$t('dashboardPage_create_broadcast')
      },
      createSessionTitle() {
         return this.isPrivate ? this.$t('dashboardPage_create_room_private_title') : this.$t('dashboardPage_create_room_live_title')
      },
      isNoErrors() {
         return !this.errors.showErrorAlreadyCreated && !this.errors.showErrorEmpty &&
                !this.errors.showErrorMaxUsers && !this.errors.showErrorWrongTime && !this.errors.showErrorWrongNumber
      }
   },
   methods: {
      createStudyRoom(){
         let form = this.$refs.createRoomValidation
         if(!form.validate()) return

         if(!this.isLoading && this.isNoErrors){
            this.isLoading = true
            if(this.isPrivate) {
               this.createPrivateSession()
            } else {
               this.createLiveSession()
            }
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
         this.$store.dispatch('updateCreateStudyRoomPrivate', privateObj).catch((error) => {
               self.handleCreateError(error)
            }).finally(() => {
               self.loisLoading = false;
               self.$store.commit('setComponent')               
            })
      },
      createLiveSession() {
         let childComponent = this.$refs.childComponent;
         let userChooseDate =  this.$moment(`${childComponent.date}T${childComponent.hour}:00`);         
         let isToday = userChooseDate.isSame(this.$moment(), 'day');
         if(isToday) {
            let isValidDateToday = userChooseDate.isAfter(this.$moment().format())
            if(!isValidDateToday) {
               this.errors.showErrorWrongTime = true
               this.currentError = 'showErrorWrongTime'
               this.isLoading = false
               return
            } 
         }

         if(childComponent.radioEnd === 'after') {
            if(isNaN(childComponent.endAfterOccurrences)) {
               this.errors.showErrorWrongNumber = true
               this.currentError = 'showErrorWrongNumber'
               this.isLoading = false
               return
            }
         }

         let liveObj = {
            name: childComponent.liveSessionTitle,
            price: childComponent.currentVisitorPriceSelect.value === 'free' ? 0 : childComponent.price,
            date: userChooseDate,
            description: childComponent.sessionAboutText,
            repeat: childComponent.currentRepeatItem.value !== 'none' ? childComponent.currentRepeatItem.value : undefined,
            endDate: childComponent.radioEnd === 'on' ? this.$moment(childComponent.dateOcurrence) : undefined,
            endAfterOccurrences: childComponent.radioEnd === 'after' ? childComponent.endAfterOccurrences : undefined,
            repeatOn: childComponent.currentRepeatItem.value === 'custom' ? childComponent.repeatCheckbox : undefined,
            image: childComponent.newLiveImage
         }
         
         let self = this
         this.$store.dispatch('updateCreateStudyRoomLive', liveObj)
            .catch((error) => {
               self.handleCreateError(error)
            }).finally(() => {
               self.isLoading = false;
               self.$store.commit('setComponent')
            })
      },
      handleCreateError(error) {
         console.log(error)
         if(error.response?.status == 409){
            self.errors.showErrorAlreadyCreated = true
            self.currentError = 'showErrorAlreadyCreated'
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
         this.errors.showErrorWrongNumber = false
         this.currentError = ''
      }
   },
   created() {
      this.studyRoomType = this.params?.type
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
   &.liveHeight {
      height: 700px;
      @media (max-width: @screen-xs) {
         height: 100%;
      }
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