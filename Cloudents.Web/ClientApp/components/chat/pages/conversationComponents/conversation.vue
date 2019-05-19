<template>
  <div class="conversation-container">
    <v-flex class="avatar-container">
      <user-avatar :userImageUrl="conversation.image" :user-name="conversation.name"/>
      <userOnlineStatus class="user-status" :userId="conversation.userId"></userOnlineStatus>
    </v-flex>
    <v-flex class="user-detail-container">
      <v-flex class="top-detail-container">
        <v-flex>
          <span class="conversation-name" v-html="conversation.name"></span>
          <span class="conversation-desc text-truncate" v-html="conversation.lastMessage"></span>
      </v-flex>
        <v-flex class="date-unread-container">
          <span class="conversation-date">{{date}}</span>
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
import userOnlineStatus from "../../../helpers/userOnlineStatus/userOnlineStatus.vue"
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
      return utilitiesService.dateFormater(this.conversation.dateTime);
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
      bottom: 0;
      position: absolute;
      right: -2px;
    }
  }
  .user-detail-container {
    padding: 12px;
    border-bottom: solid 2px rgba(67, 66, 93, 0.17);
    padding: 16px 12px 16px 12px;
    
    .top-detail-container {
      display: flex;
      justify-content: space-between;
      .conversation-name{
        display: flex;
        font-size: 13px;
        font-weight: bold;
        color: #43425d;
        align-items: center;
        word-break: break-all;
        text-overflow: ellipsis;
        width: 180px;
        white-space: nowrap;
        overflow: hidden;
      }
      .conversation-desc{
        font-size: 12px;
        width: 180px;
        display: block;
      }
      .date-unread-container{
        display: flex;
        flex-direction: column;
        text-align: right;
        .conversation-date{
          font-size: 11px;
          color: rgba(0, 0, 0, 0.38);
          
        }
        .conversation-unread{
          background: #33cea9;
          color: #fff;
          border-radius: 50%;
          height: 16px;
          width: 16px;
          line-height: 16px;
          display: inline-block;
          text-align: center;
          vertical-align: middle;
          font-size: 11px;
          margin-top: 3px;
        }
      }
      
    }
  }
}
</style>
