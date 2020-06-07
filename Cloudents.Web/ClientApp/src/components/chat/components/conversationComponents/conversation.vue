<template>
  <div class="conversation-container">
    <v-flex class="avatar-container">
        <user-avatar :size="'49'" :userImageUrl="userImg" :user-name="conversation.name"/>
        <div v-if="conversation.online" class="onlineDot"></div>
    </v-flex>
    <v-flex class="user-detail-container text-truncate">
      <v-flex class="top-detail-container">

        <v-flex class="top-detail-container-wrap">
          <div :class="['conversation-name','text-truncate',{'font-weight-bold':isConversationUnread}]" class="">{{conversation.name}}</div>
          <span class="conversation-date">{{date}}</span>
        </v-flex>

        <v-flex class="date-unread-container">
          <template>
            <div class="conversation-desc text-truncate" v-html="conversation.lastMessage"></div>
          </template>
          <div>
            <span v-show="conversation.unread > 0" class="conversation-unread">{{conversation.unread}}</span>
          </div>
        </v-flex>
        
      </v-flex>
    </v-flex>
  </div>
</template>

<script>
import utilitiesService from "../../../../services/utilities/utilitiesService";

export default {
  props: {
    conversation: {
      type: Object
    }
  },
  computed: {
    date() {
      let momentDate = this.$moment(this.conversation.dateTime);
      let isToday = momentDate.isSame(this.$moment(), 'day');
      if(isToday){
            return momentDate.format('LT');
      }else{
        if (this.$moment().diff(momentDate, 'days') >= 1) {
          return momentDate.format('l');
        }else{
          return momentDate.calendar().split(' ')[0];
        }
      }
    },
    userImg() {
      return utilitiesService.proccessImageURL(this.conversation.image, 46, 46);
    },
    isConversationUnread(){
      return this.conversation.unread > 0;
    }
  }
};
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.conversation-container {
  padding: 12px 0px 12px 16px;
  border-bottom: solid 1px rgba(238, 238, 238, 0.8);
  height: 76px;
  display: flex;
  width: 100%;
  align-items: center;
  &:hover{
    background: #f0f3f6;
  }
  .avatar-container {
    position:relative;
    flex-grow: 0;
    .onlineDot{
      position: absolute;
      width: 12px;
      height: 12px;
      background-color: #2dfe14;
      border-radius: 50%;
      bottom: 0px;
      right: 0;
    }
  }
  .user-detail-container {
    margin-left: 16px;
    padding-right: 8px ;
    height: 100%;
    .top-detail-container {
          display: flex;
          flex-direction: column;
          justify-content: center;
          height: 100%;
      .top-detail-container-wrap {
        justify-content: space-between;
        display: flex;
        align-items: center;
        padding-top: 2px;
        padding-bottom: 2px;
        .conversation-name{
          max-width: 160px;
          font-size: 16px;
          color: #43425d;
          align-items: center;
          font-family: sans-serif;
          @media (max-width: 425px) {
            max-width: 200px;
          }
          @media (max-width: 375px) {
            max-width: 180px;
          }
          @media (max-width: 320px) {
            max-width: 150px;
          }
        }
        .conversation-date{
          font-size: 13px;
          color: #69687d;
        }
      }
      .date-unread-container{
        display: flex;
        justify-content: space-between;
        align-items: center;
        .conversation-desc{
          .giveMeEllipsis(1, 18);
          display: block;
          text-align: left;
          font-size: 14px;
          color: #69687d;
          align-items: center;
          width: 86%;
          img {
            margin-right: 4px;
          }
        }
        .conversation-unread{
          background: #4c59ff;
          color: #fff;
          border-radius: 50%;
          height: 20px;
          width: 20px;
          line-height: 20px;
          display: inline-block;
          text-align: center;
          vertical-align: middle;
          font-size: 12px;
        }
      }
    }
  }
}
</style>
