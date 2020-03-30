<template>
  <div class="conversation-container">
    <v-flex class="avatar-container">
      <template v-if="isSingleUser">
        <user-avatar :size="'46'" :userImageUrl="conversation.users[0].image" :user-name="conversation.users[0].name"/>
        <userOnlineStatus class="user-status" :userId="conversation.userId"></userOnlineStatus>
      </template>
      <template v-else>
      <v-avatar size="46">
        <img src="src">
      </v-avatar>
      </template>
    </v-flex>
    <v-flex class="user-detail-container">
      <v-flex class="top-detail-container">
        <v-flex class="top-detail-container-wrap">
          <div class="conversation-name text-truncate">{{conversationName}}</div>
          <span class="conversation-date">{{date}}</span>
        </v-flex>

        <v-flex class="date-unread-container">
          <template>
            <div class="conversation-desc" v-html="conversation.lastMessage"></div>
          </template>
          <div>
            <span v-show="unreadMessages > 0" class="conversation-unread">{{unreadMessages}}</span>
          </div>
        </v-flex>
        
      </v-flex>
    </v-flex>
  </div>
</template>

<script>
import userOnlineStatus from "../../../helpers/userOnlineStatus/userOnlineStatus.vue";
import timeAgoService from '../../../../services/language/timeAgoService';

export default {
  components: {
    userOnlineStatus
  },
  props: {
    conversation: {
      type: Object
    }
  },
  computed: {
    conversationName(){
      let userNames = this.conversation.users.map(u=>u.name).join(" ,")
      return userNames
    },
    date() {
        return timeAgoService.timeAgoFormat(this.conversation.dateTime)
    },
    isSingleUser(){
      return (this.conversation.users.length == 1)
    },
    unreadMessages(){
      let unreads = this.conversation.users.map(u=>u.unread);
      const reducer = (accumulator, currentValue) => accumulator + currentValue;
      return unreads.reduce(reducer)
    },
  }
};
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.conversation-container {
  display: flex;
  width: 100%;
  align-items: center;
  &:hover{
    background: #f7f7f7;
  }
  .avatar-container {
    position:relative;
    flex-grow: 0;
    margin-left: 12px;
    .user-status{
      bottom: 2px;
      position: absolute;
      right: 0;
    }
  }
  .user-detail-container {
    border-bottom: solid 1px #dfdfe4;
    padding: 6px 0;
    margin: 0 10px;
    .top-detail-container {
          display: flex;
          flex-direction: column;
          justify-content: center;
          height: 100%;
      .top-detail-container-wrap {
        justify-content: space-between;
        display: flex;
        align-items: center;
        .conversation-name{
          max-width: 160px;
          font-size: 14px;
          font-weight: bold;
          color: #43425d;
          align-items: center;
          word-break: break-all;
          text-overflow: ellipsis;
          white-space: nowrap;
          overflow: hidden;
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
          font-size: 12px;
          color: #919095;
        }
      }
      .date-unread-container{
        display: flex;
        justify-content: space-between;
        text-align: right;
        max-height: 50px;
        min-height: 36px;
        align-items: center;
        .conversation-desc{
          .giveMeEllipsis(2, 18);
          display: block;
          text-align: left;
          font-size: 12px;
          color: #919095;
          align-items: center;
          width: 200px;
          @media (max-width: @screen-xs) {
            width: 300px;
          }
          @media (max-width: @screen-xss) {
            width: 220px;
          }
          img {
            margin-right: 4px;
            vertical-align: middle;
          }
        }
        .conversation-unread{
          background: #5158af;
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
