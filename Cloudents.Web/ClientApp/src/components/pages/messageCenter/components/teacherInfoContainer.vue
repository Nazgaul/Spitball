<template>
   <v-flex xs12 sm6 md3 class="teacherInfoContainer">
      <div class="teacherInfoHeader ps-4 d-flex flex-grow-0 flex-shrink-0 align-center ">
         <v-icon @click="$emit('toggleTeacherInfo')" class="me-5 me-sm-3 d-flex d-md-none" size="16" :color="isMobile? '#ffffff' : '#69687d'">{{isRtl?'sbf-arrow-right-carousel':'sbf-arrow-left-carousel'}}</v-icon>
         <span v-t="'chat_teacher_info'"/>
      </div>
      <div class="teacherInfoContent px-5 py-3">
         <div class="content">
            <div class="teacherAvatar text-center">
               <user-avatar class="pt-2" :size="'107'" :userImageUrl="tutorAvatar" :user-name="tutorName"/>
               <div class="teacherAvatarName text-center pt-2" v-t="{path:'chat_teacher_name',args:{0:tutorName}}"/>
            </div>
            <div class="actionBoxs pt-4">
               <v-btn v-if="isBookSession" block depressed text class="actionBox mb-3 cursor-pointer" 
                  :to="{name: profileRoute,params: {id: currentTutor.id,name: currentTutor.name}}">
                  <v-icon color="#4c59ff" size="20">sbf-book-calendar</v-icon>
                  <div class="actionName" v-t="'chat_teacher_btn_book'"/>
               </v-btn>
               <v-btn v-if="showStudyRoomBtn" block depressed text class="actionBox mb-3 cursor-pointer" @click="isStudyRoom ? goToStudyRoom() : createStudyRoom()">
                  <v-icon color="#41c4bc" size="20">sbf-enter-room</v-icon>
                  <div class="actionName">{{chatText}}</div>
               </v-btn>
            </div>

            <div v-if="participants" class="participantList">
               <div class="listHeader px-4 d-flex justify-space-between align-center flex-grow-0 flex-shrink-0">
                  <span class="participantsTitle" v-t="'chat_participants'"/>
                  <span>
                     <v-icon size="14" color="#7a798c" class="pe-1">sbf-users</v-icon>
                     {{participantsCount}}
                  </span>
               </div>
               <v-list class="list flex-grow-1">
                  <template v-for="(item, index) in participants">
                     <v-divider :key="index+'_'" v-if="index > 0" class="dividerList"></v-divider>
                     <v-list-item :key="index">
                        <div class="d-flex align-center">
                           <user-avatar class="me-4" :size="'32'" :userImageUrl="item.image" :user-name="item.name"/>
                           <span class="listUserTitle">{{item.name}}</span>
                        </div>
                     </v-list-item>
                  </template>
               </v-list>
            </div>
         </div>
      </div>
   </v-flex>
</template>

<script>
import { mapGetters } from 'vuex';
import * as routeNames from '../../../../routes/routeNames.js';

export default {
   data() {
      return {
         isRtl:global.isRtl,
         profileRoute: routeNames.Profile,
      }
   },
   computed: {
      ...mapGetters(['accountUser','getActiveConversationObj']),
      currentConversation(){
         return this.getActiveConversationObj;
      },
      currentTutor(){
         return this.$store.getters.getActiveConversationTutor;
      },
      tutorName(){
         return this.currentTutor?.name
      },
      tutorAvatar(){
         return this.currentTutor?.image;
      },
      participants(){
         return this.currentConversation?.users
      },
      isStudyRoom(){ 
         return this.currentTutor?.studyRoomId
      },
      showStudyRoomBtn(){
         if(this.isStudyRoom) return true;
         return this.currentTutor?.id == this.$store.getters.accountUser?.id;
      },
      participantsCount(){
         return this.participants?.length || 0;
      },
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      },
      isBookSession(){
         return this.currentTutor?.calendar;
      },
      chatText() {
         if (this.isStudyRoom) {
            return this.$t('chat_teacher_btn_studyroom')
         }
         return this.$t('chat_teacher_btn_studyroom_create');
      }
   },
   methods: {
      goToStudyRoom(){
         let routeParams = {
            name: routeNames.StudyRoom,
            params: {
               id: this.currentTutor?.studyRoomId
            }
         }
         let routeData = this.$router.resolve(routeParams);
         global.open(routeData.href, "_blank");
      },
      createStudyRoom(){
         this.$router.push({name:routeNames.MyStudyRooms})
      }
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.teacherInfoContainer{
   background: #ffffff;
   @headerHeight:62px;
   @media(max-width: @screen-xs) {
      @headerHeight: 60px;
      height: 100vh;
   }
   height: 100%;
   border-left: 1px solid #e4e4e4;
   @media(max-width: @screen-xs) {
      border: none;
   }
   .teacherInfoHeader{
      max-width: 100%;
      height: @headerHeight;
      background-color: #efefef;
      font-size: 16px;
      font-weight: 600;
      color: #43425d;
      @media(max-width: @screen-xs) {
         background-color: #4c59ff;
         color: #ffffff;
      }
   }
   .teacherInfoContent{
      height: calc(~"100% - 62px");
      overflow-y: auto;
      .content{
         max-width: 320px;
         margin: 0 auto;
         .teacherAvatar{
            color: #43425d;
            font-weight: 600;

            .teacherAvatarName{
               font-size: 16px;
               text-transform: capitalize;
            }
         }
         .actionBoxs{
            .actionBox{
               padding: 4px 0;
               height: 64px;
               border-radius: 6px;
               border: solid 1.5px #ced0dc75;
               display: flex;
               flex-direction: column;
               .v-btn__content{
                  flex-direction: column;
                  align-items: center;
                  justify-content: space-evenly;
               }
               .actionName{
                  font-size: 14px;
                  font-weight: 600;
                  color: #69687d;
                  text-transform: none;
               }
            }
         }
         .participantList{
            ::-webkit-scrollbar-track {
               background:none; 
            }
            max-height: 296px;
            // height: 296px;
            border-radius: 6px;
            border: solid 1.5px #ced0dc75;
            display: flex;
            flex-direction: column;
            .listHeader{
               height: 42px;
               background-color: #f5f5f5;
               color: #43425d;
               .participantsTitle{
                  font-size: 14px;
                  font-weight: 600;
               }
            }
            .list{
               background: none;
               padding: 0;
               overflow-y: auto;
               .dividerList{
                  margin: 0 auto;
                  width: calc(~"100% - 40px");
               }
               .listUserTitle{
                  font-size: 14px;
                  font-weight: 600;
                  color: #43425d;
                  text-transform: capitalize;
               }
            }
         }
      }
   }
}
</style>