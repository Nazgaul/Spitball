<template>
   <v-dialog :value="true" persistent max-width="640px" :fullscreen="$vuetify.breakpoint.xsOnly">
      <div class="createStudyRoomDialog">
         <v-icon class="close-dialog" v-text="'sbf-close'" v-closeDialog />
         <div class="createStudyRoomDialog-title pb-4">{{$t('dashboardPage_create_room_title')}}</div>
         <v-form class="d-flex justify-space-between input-room-name" ref="createRoomValidation">
            <v-text-field :rules="[rules.required]" v-model="roomName" height="44" dense outlined :label="$t('dashboardPage_create_room_placeholder')" :placeholder="$t(roomNamePlaceholder)"/>
            <v-text-field class="px-4" outlined  height="44" dense :rules="[rules.required,rules.integer,rules.minimum]"
               v-model="price" type="number"
               :label="$t('becomeTutor_placeholder_price', {'0' : getSymbol})" :placeholder="$t('becomeTutor_placeholder_price', {'0' : getSymbol})">
            </v-text-field>
            <v-select
               v-model="studyRoomType"
               class="roomType"
               append-icon="sbf-menu-down"
               :items="items"
               :label="$t('dashboardPage_placeholder_studyRoom_type')"
               return-object
               height="44"
               outlined
            ></v-select>
         </v-form>

         <div class="createStudyRoomDialog-list">
            <v-list flat class="list-followers" v-if="studyRoomType.value === 'Private'">
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

            <div class="dateTimeWrapper d-flex justify-center" v-else>
               <v-menu ref="datePickerMenu" v-model="datePickerMenu" :close-on-content-click="false" transition="scale-transition" offset-y max-width="290" min-width="290px">
                  <template v-slot:activator="{ on }">
                        <v-text-field 
                           v-on="on"
                           v-model="date"
                           class="date-input"
                           :rules="[rules.required]"
                           :label="$t('dashboardPage_label_date')"
                           autocomplete="nope"
                           prepend-inner-icon="sbf-calendar"
                           dense
                           color="#304FFE"
                           outlined
                           type="text"
                           readonly
                           :height="$vuetify.breakpoint.xsOnly ? 50 : 44"
                        />
                  </template>
                  <v-date-picker :allowed-dates="allowedDates" color="#4C59FF" class="date-picker" :next-icon="isRtl?'sbf-arrow-left-carousel':'sbf-arrow-right-carousel'" :prev-icon="isRtl?'sbf-arrow-right-carousel':'sbf-arrow-left-carousel'" v-model="date" no-title @input="datePickerMenu = false">
                     <v-spacer></v-spacer>
                     <v-btn text class="font-weight-bold" color="#4C59FF" @click="datePickerMenu = false">{{$t('coupon_btn_calendar_cancel')}}</v-btn>
                     <v-btn text class="font-weight-bold" color="#4C59FF" @click="$refs.datePickerMenu.save(date)">{{$t('coupon_btn_calendar_ok')}}</v-btn>
                  </v-date-picker>
               </v-menu>

                  <!-- TIME PICKER TEXT FIELD -->
                  <v-select
                     v-model="hour"
                     class="roomType mx-5"
                     append-icon="sbf-menu-down"
                     :items="timeHoursList"
                     :menu-props="{
                        maxHeight: 200
                     }"
                     :label="$t('dashboardPage_labe_hours')"
                     placeholder=" "
                     outlined
                  ></v-select>

                  <v-select
                     v-model="minutes"
                     class="roomType"
                     append-icon="sbf-menu-down"
                     :items="timeMinutes"
                     :label="$t('dashboardPage_label_minutes')"
                     placeholder=" "
                     outlined
                  ></v-select>


               <!-- TIME PICKER vuetify ui -->
               <!-- <v-menu 
                  v-model="timePickerMenu" 
                  ref="timePickerMenu" 
                  :close-on-content-click="false" 
                  transition="scale-transition" 
                  offset-y 
                  max-width="290" 
                  min-width="290px"
               >
                  <template v-slot:activator="{ on }">
                        <v-text-field
                           v-on="on"
                           v-model="time"
                           class="time-input"
                           :rules="[rules.required]"
                           :label="$t('dashboardPage_label_time')"
                           autocomplete="nope"
                           prepend-inner-icon=""
                           dense
                           color="#304FFE"
                           outlined
                           scrollable
                           type="text"
                           readonly
                           :height="$vuetify.breakpoint.xsOnly ? 50 : 44"
                        />
                  </template>                  
                  <v-time-picker 
                     v-model="time"
                     class="timePicker" 
                     color="#4C59FF" 
                     :next-icon="isRtl?'sbf-arrow-left-carousel':'sbf-arrow-right-carousel'" 
                     :prev-icon="isRtl?'sbf-arrow-right-carousel':'sbf-arrow-left-carousel'" 
                  >
                     <v-spacer></v-spacer>
                     <v-btn text class="font-weight-bold" color="#4C59FF" @click="timePickerMenu = false">{{$t('coupon_btn_calendar_cancel')}}</v-btn>
                     <v-btn text class="font-weight-bold" color="#4C59FF" @click="$refs.timePickerMenu.save(time)">{{$t('coupon_btn_calendar_ok')}}</v-btn>
                  </v-time-picker>
               </v-menu> -->
            </div>

         </div>
         <div class="d-flex flex-column align-center pt-4">
            <div class="mb-4">
               <span v-if="showErrorEmpty" class="error--text" v-t="'dashboardPage_create_room_empty_error'"></span>
               <span v-if="showErrorAlreadyCreated" class="error--text" v-t="'dashboardPage_create_room_created_error'"></span>
               <span v-if="showErrorMaxUsers" class="error--text" v-t="'dashboardPage_create_room_max_error'"></span>
               <span v-if="showErrorWrongTime" class="error--text" v-t="'dashboardPage_pick_time_error'"></span>
            </div>
            <v-btn :loading="isLoading" @click="createStudyRoom" width="150" depressed height="40" color="#4452fc" class="white--text" rounded >{{$t('dashboardPage_create_room_create_btn')}}</v-btn>
         </div>
      </div>
   </v-dialog>
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules.js'

export default {
   name:'createStudyRoom',
   data() {
      return {
         date: new Date().toISOString().substr(0, 10),
         time: '',
         hour: '00',
         minutes: '00',
         roomName:'',
         price: 0,
         isLoading:false,
         datePickerMenu:false,
         showErrorEmpty:false,
         showErrorMaxUsers:false,
         showErrorWrongTime: false,
         showErrorAlreadyCreated:false,
         selected:[],
         myFollowers:[],
         MAX_PARTICIPANT: 49,
         isRtl: global.isRtl,
         rules: {
            required: (value) => validationRules.required(value),
            minimum: (value) => validationRules.minVal(value,0),
         },
         studyRoomType: {text: this.$t('dashboardPage_type_private'), value: 'Private'},
         items: [
            { text: this.$t('dashboardPage_type_private'), value: 'Private' },
            { text: this.$t('dashboardPage_type_broadcast'), value: 'Broadcast' }
         ],
      }
   },
   watch: {
      selected(){
         this.resetErrors()
      },
      studyRoomType() {
         this.resetErrors()
      },
      hour() {
         this.resetErrors()
      },
      minutes() {
         this.resetErrors()
      }
   },
   computed: {
      getSymbol() {
         let v =   this.$n(1,'currency');
         return v.replace(/\d|[.,]/g,'').trim();
      },
      roomNamePlaceholder() {
         let roomNamePlaceholder = {
            private: 'dashboardPage_create_room_label',
            broadcast: 'dashboardPage_create_room_label_broadcast'
         }
         return roomNamePlaceholder[this.studyRoomType.value.toLowerCase()]
      },
      timeMinutes() {
         let arr = []
         let jump = 15
         for (let i = 0; i < 60; i++) {
            if(i % jump === 0) {
               arr.push(i.toString().padStart(2, '0'));
            }
         }
         return arr
      },
      timeHoursList() {
         let arr = []
         for (let i = 0; i < 24; i++) {
            arr.push(i.toString().padStart(2, '0'));
         }
         return arr
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
            if(this.selected.length < this.MAX_PARTICIPANT){
               this.selected.push(user)
            }else{
               this.showErrorMaxUsers = true;
            }
         }
      },
      createStudyRoom(){
         if(!this.$refs.createRoomValidation.validate()) return
         if(!this.isLoading && !this.showErrorAlreadyCreated && !this.showErrorEmpty && !this.showErrorMaxUsers){
            let isBroadcast = this.studyRoomType.value === 'Broadcast'
            
            if(!this.selected.length && !isBroadcast){
               this.showErrorEmpty = true;
               return 
            }

            if(isBroadcast) {
               let today = new Date()
               if(this.date === today.toISOString().substr(0, 10)) {

                  let hour = Number(this.hour);
                  let isWrongMinutes = today.getMinutes() < Number(this.minutes.padStart(0))
                  if(hour < today.getHours()) {
                     this.showErrorWrongTime = true
                     return
                  }

                  if(hour === today.getHours()) {
                     if(!isWrongMinutes) {
                        this.showErrorWrongTime = true
                        return
                     }
                  }
               }
            }
               
            let paramsObj = {
               name: this.roomName,
               userId: Array.from(this.selected.map(user=> user.userId)),
               price: this.price || 0,
               type: this.studyRoomType.value,
               date: new Date(`${this.date} ${this.hour}:${this.minutes}`)
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
      },
      allowedDates(date) {
         let today = new Date().toISOString().substr(0, 10)
         return date >= today
      },
      resetErrors() {
         this.showErrorEmpty = false;
         this.showErrorAlreadyCreated = false;
         this.showErrorMaxUsers = false;
         this.showErrorWrongTime = false
      }
   },
   created() {
      this.$store.dispatch('updateFollowersItems').then(()=>{
         this.myFollowers = this.$store.getters.getFollowersItems
      })
      this.price = this.$store.getters.accountUser.price;
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.createStudyRoomDialog{
   background: white;
   position: relative;
   padding: 10px;
   // height: 520px;
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
      width: 95%;
      // width: 216px;
      .v-text-field__details{
         margin-bottom: 0;
      }
   }
   .roomType {
      .v-input__slot {
         min-height: 44px !important;

         .v-input__append-inner {
            margin-top: 10px;
         }
      }
   }
   .dateTimeWrapper {
      width: 95%;
      margin: 0 auto;
      .menuHour {
         max-height: 200px;
      }
   }
   .createStudyRoomDialog-list{
      width: 100%;
      height: 320px;
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