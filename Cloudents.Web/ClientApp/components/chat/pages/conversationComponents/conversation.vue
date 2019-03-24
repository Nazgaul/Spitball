<template>
  <div class="conversation-container">
    <v-flex ml-2 class="avatar-container">
      <user-avatar :user-name="conversation.name" :user-id="conversation.userId"/>
    </v-flex>
    <v-flex class="user-detail-container">
      <v-flex class="top-detail-container">
        <span class="conversation-name">{{conversation.name}}</span>
        <span class="conversation-date">{{date}}</span>
      </v-flex>
      <!-- <v-flex>
            <span>message {{conversation}}</span>
      </v-flex>-->
    </v-flex>
  </div>
</template>

<script>
import UserAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
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
      let date = new Date(this.conversation.dateTime);
      let options = { year: "numeric", month: "short", day: "2-digit" };
      return new Intl.DateTimeFormat("en-US", options).format(date);
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
        font-size: 11px;
        font-weight: bold;
        color: #43425d;
      }
      .conversation-date{
        font-size: 11px;
        color: #43425d;
      }
    }
  }
}
</style>
