<template>
  <div class="conversation-container">
    <v-flex ml-2 class="avatar-container">
      <user-avatar :userImageUrl="conversation.image" :user-name="conversation.name"/>
    </v-flex>
    <v-flex class="user-detail-container">
      <v-flex class="top-detail-container">
        <span class="conversation-name">{{conversation.name}}</span>
        <v-flex class="date-unread-container">
          <span class="conversation-date">{{date}}</span>
          <div>
            <span v-show="conversation.unread > 0" class="conversation-unread">{{conversation.unread}}</span>
          </div>
          
        </v-flex>
        
      </v-flex>
      <!-- <v-flex>
            <span>message {{conversation}}</span>
      </v-flex>-->
    </v-flex>
  </div>
</template>

<script>
import UserAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import utilitiesService from "../../../../services/utilities/utilitiesService"
export default {
  components: {
    UserAvatar
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
  padding: 7px 0;
  border-bottom: solid 1px rgba(67, 66, 93, 0.17);
  .avatar-container {
    flex-grow: 0;
  }
  .user-detail-container {
    padding: 12px;
    .top-detail-container {
      display: flex;
      justify-content: space-between;
      .conversation-name{
        display: flex;
        font-size: 11px;
        font-weight: bold;
        color: #43425d;
        align-items: center;
      }
      .date-unread-container{
        display: flex;
        flex-direction: column;
        text-align: right;
        .conversation-date{
          font-size: 11px;
          color: #43425d;
        }
        .conversation-unread{
          background: #4452fc;
          color: #fff;
          border-radius: 50%;
          height: 16px;
          width: 16px;
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
