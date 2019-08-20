<template>
  <div class="conversation-container">
    <v-flex class="avatar-container">
      <user-avatar :size="'46'" :userImageUrl="userImg" :user-name="conversation.name"/>
      <userOnlineStatus class="user-status" :userId="conversation.userId"></userOnlineStatus>
    </v-flex>
    <v-flex class="user-detail-container">
      <v-flex class="top-detail-container">
        <v-flex class="top-detail-container-wrap">
          <div class="conversation-name pb-2 text-truncate">{{conversation.name}}</div>
          <template>
            <span class="conversation-desc text-truncate" v-html="conversation.lastMessage"></span>
          </template>
        </v-flex>
        <v-flex class="date-unread-container">
          <span class="conversation-date pb-2">{{date}}</span>
          <div>
            <span v-show="conversation.unread > 0" class="conversation-unread">{{conversation.unread}}</span>
          </div>
        </v-flex>
      </v-flex>
    </v-flex>
  </div>
</template>

<script>
import UserAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import utilitiesService from "../../../../services/utilities/utilitiesService";
import userOnlineStatus from "../../../helpers/userOnlineStatus/userOnlineStatus.vue";
import timeAgoService from '../../../../services/language/timeAgoService';

export default {
  components: {
    UserAvatar,
    userOnlineStatus
  },
  props: {
    conversation: {
      type: Object
    }
  },
  computed: {
    date() {
        return timeAgoService.timeAgoFormat(this.conversation.dateTime)
    },
    userImg() {
      return utilitiesService.proccessImageURL(this.conversation.image, 46, 46);
    }
  }
};
</script>

<style lang="less">
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
    padding: 12px 0;
    margin: 0 10px;
    .top-detail-container {
      display: flex;
      justify-content: space-between;
      .top-detail-container-wrap {
        max-width: 148px;
      }
      .conversation-name{
        font-size: 14px;
        font-weight: bold;
        color: #43425d;
        align-items: center;
        word-break: break-all;
        text-overflow: ellipsis;
        white-space: nowrap;
        overflow: hidden;
      }
      .conversation-desc{
        font-size: 12px;
        width: 180px;
        color: #919095;
        display: flex;
        align-items: center;
        img {
          margin-right: 8px;
        }
      }
      .date-unread-container{
        display: flex;
        flex-direction: column;
        text-align: right;
        .conversation-date{
          font-size: 12px;
          color: #919095;
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
          font-size: 11px;
        }
      }
    }
  }
}
</style>
