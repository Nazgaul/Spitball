<template>
   <v-flex xs12 sm6 md3 class="teacherInfoContainer">
      <div class="teacherInfoHeader pl-4 d-flex flex-grow-0 flex-shrink-0 align-center ">
         <span v-t="'chat_teacher_info'"/>
      </div>
      <div class="teacherInfoContent px-5">
         <div class="teacherAvatar">
            <user-avatar class="pt-2" :size="'107'" :userImageUrl="teacherAvatar" :user-name="teacherName"/>
            <div class="teacherAvatarName text-center pt-2" v-t="{path:'chat_teacher_name',args:{0:teacherName}}"/>
         </div>
         <div class="actionBoxs pt-4">
            <div class="actionBox mb-3">
               <v-icon color="#4c59ff" size="20">sbf-book-calendar</v-icon>
               <div class="actionName" v-t="'chat_teacher_btn_book'"/>
            </div>
            <div class="actionBox mb-3">
               <v-icon color="#41c4bc" size="20">sbf-enter-room</v-icon>
               <div class="actionName" v-t="'chat_teacher_btn_studyroom'"/>
            </div>
            <div class="actionBox mb-3">
               <v-icon color="#de5642" size="16">sbf-email-chat</v-icon>
               <div class="actionName" v-t="'chat_teacher_btn_mail'"/>
            </div>
            <div class="actionBox mb-5">
               <v-icon color="#29d367" size="20">sbf-whatsup-share</v-icon>
               <div class="actionName" v-t="'chat_teacher_btn_whatsapp'"/>
            </div>
         </div>

         <div class="participantList">
            <div class="listHeader px-4 d-flex justify-space-between align-center flex-grow-0 flex-shrink-0">
               <span class="participantsTitle" v-t="'chat_participants'"/>
               <span>
                  <v-icon size="16" color="#7a798c" class="pr-1">sbf-users</v-icon>
                  {{chatCounter}}
               </span>
            </div>
            <div class="list">
               <v-list>
                  <template v-for="(item, index) in chatCounter">
                     <v-divider :key="index+'_'"
                        v-if="index > 0"
                        class="px-5"
                     ></v-divider>
                     <v-list-item :key="index">
                        <div class="d-flex align-center">
                           <user-avatar class="mr-4" :size="'32'" :userImageUrl="teacherAvatar" :user-name="teacherName"/>
                           <span class="listUserTitle">{{teacherName}}</span>
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
import { mapGetters } from 'vuex'
export default {
   computed: {
      ...mapGetters(['accountUser','getActiveConversationObj']),
      teacherAvatar(){
         return this.accountUser?.image;
      },
      teacherName(){
         return this.accountUser?.name
      },
      chatCounter(){
         return this.getActiveConversationObj?.conversationId?.split('_').length;
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
   }
   height: 100%;
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
         border: none;
      }
   }
   .teacherInfoContent{
      .teacherAvatar{
         .teacherAvatarName{
            font-size: 16px;
            font-weight: 600;
            color: #43425d;
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
            align-items: center;
            justify-content: space-evenly;
            .actionName{
               font-size: 14px;
               font-weight: 600;
               color: #69687d;
            }
         }
      }
      .participantList{
         height: 296px;
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
            overflow-y: scroll;
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
</style>