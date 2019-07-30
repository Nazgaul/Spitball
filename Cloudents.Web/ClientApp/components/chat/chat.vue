<template>
  <v-container
    v-if="visible"
    py-0
    px-0
    class="sb-chat-container"
    :class="[ $route.name == 'tutoring' ?  'chat-studyRoom': '', {'minimized': isMinimized}]"
  >

    <v-layout @click="toggleMinimizeChat" class="chat-header" :class="{'new-messages': hasUnread}">
      <v-icon
        :class="{'rtl':isRtl}"
        @click.stop="OriginalChatState"
      >{{inConversationState ? 'sbf-message-icon' : 'sbf-arrow-back-chat'}}</v-icon>

      <template v-if="state === 'messages'">
        <user-avatar :size="'32'" :user-name="activeConversationObj.name" :user-id="activeConversationObj.userId" :userImageUrl="activeConversationObj.image"/> 
        <div class="chat-header-name pl-3">{{activeConversationObj.name}}</div>
        <span class="other-side">
          <v-btn-toggle class="chat-header-btn" active-class="chat-btns">
            <v-btn text>
              <v-icon
                v-show="!isMobile"
                @click.stop="toggleMinimizeChat"
              >{{isMinimized ? 'sbf-toggle-enlarge' : 'sbf-minimize'}}
              </v-icon>
            </v-btn>
            <v-btn text>
              <v-icon v-if="!isLocked" @click.stop="closeChatWindow">sbf-close-chat</v-icon>
            </v-btn>
          </v-btn-toggle>
        </span>
      </template>

      <template v-else>
        <span class="chat-header-text">{{getIsSignalRConnected ? headerTitle : errorTitle}}</span>
        <span class="other-side">
          <v-icon
            v-show="!isMobile"
            @click.stop="toggleMinimizeChat"
          >{{isMinimized ? 'sbf-toggle-enlarge' : 'sbf-minimize'}}</v-icon>
          <v-icon v-if="!isLocked" @click.stop="closeChatWindow">sbf-close-chat</v-icon>
        </span>
      </template>

    </v-layout>

    <v-layout v-show="!isMinimized" class="general-chat-style">
      <component :is="`chat-${state}`"></component>
    </v-layout>
  </v-container>
</template>


<script>
import chatConversation from "./pages/conversations.vue";
import chatMessages from "./pages/messages.vue";
import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
import { mapGetters, mapActions } from "vuex";
import { LanguageService } from "../../services/language/languageService";
export default {
  components: {
    chatConversation,
    chatMessages,
    UserAvatar
  },
  data() {
    return {
      enumChatState: this.getEnumChatState(),
      mobileHeaderHeight: 39,
      isRtl: global.isRtl
    };
  },
  watch: {
    visible: function(val) {
      if (!this.isMobile) {
        return;
      }
      if (val) {
        document.body.classList.add("noscroll");
      } else {
        document.body.classList.remove("noscroll");
      }
    }
  },
  computed: {
    ...mapGetters([
      "getChatState",
      "getIsChatVisible",
      "getIsChatMinimized",
      "getActiveConversationObj",
      "getIsChatLocked",
      "accountUser",
      "getTotalUnread",
      "getIsSignalRConnected"
    ]),
    isLocked() {
      // return this.getIsChatLocked;
      return false;
    },
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    state() {
      return this.getChatState;
    },
    visible() {
      if (this.accountUser === null) {
        return false;
      } else {
        return this.getIsChatVisible;
      }
    },
    isMinimized() {
      if (this.isMobile) {
        return false;
      } else {
        return this.getIsChatMinimized;
      }
    },
    headerTitle() {
      if (this.state === this.enumChatState.conversation) {
        return LanguageService.getValueByKey("chat_messages");
      } else {
        if (!!this.getActiveConversationObj) {
          return this.getActiveConversationObj.name;
        }
      }
    },
    errorTitle() {
      return LanguageService.getValueByKey("chat_error_messages");
    },
    inConversationState() {
      if (this.isLocked) {
        return true;
      }
      return this.state === this.enumChatState.conversation;
    },
    hasUnread() {
      return this.getTotalUnread > 0;
    },
    activeConversationObj(){
      return this.getActiveConversationObj;
    },
  },
  methods: {
    ...mapActions([
      "updateChatState",
      "toggleChatMinimize",
      "closeChat",
      "openChatInterface"
    ]),
    ...mapGetters(["getEnumChatState"]),
    OriginalChatState() {
      console.log("vslkjnavesjklnvjklragvnklj");
      if (!this.isLocked) {
        this.updateChatState(this.enumChatState.conversation);
        if (this.isMinimized) {
          this.openChatInterface();
        }
      }
    },
    expandChat() {
      this.openChatInterface();
    },
    toggleMinimizeChat() {
      this.toggleChatMinimize();
    },
    closeChatWindow() {
      this.OriginalChatState();
      this.closeChat();
    }
  }
};
</script>
<style lang="less">
@import "../../styles/mixin.less";
.sb-chat-container {
  .scrollBarStyle(6px, #43425d);
  position: fixed;
  bottom: 0;
  right: 130px;
  width: 320px;
  height: 520px;
  z-index: 3;
  background: #fff;
  border-radius: 10px 10px 0 0;
  box-shadow: 0 3px 16px 0 rgba(0, 0, 0, 0.3);
  max-height: ~"calc( 100vh - 100px)";
  &.chat-studyRoom {
    right: 0 ;
    left: unset;
  }
  @media (max-width: @screen-xs) {
    width: 100%;
    height: 100%;
    max-height: unset;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    z-index: 999;
  }
  &.minimized {
    height: unset;
  }
  .chat-header {
    background-color: #43425d;
    border-radius: 4px 4px 0 0;
    padding: 10px;
    color: #fff;
    z-index: 1;
    transition: background-color 0.2s ease-in-out;
    -moz-transition: background-color 0.2s ease-in-out;
    -webkit-transition: background-color 0.2s ease-in-out;
    -o-transition: background-color 0.2s ease-in-out;
    &.new-messages {
      background-color: #33cea9;
    }
    @media (max-width: @screen-xs) {
      border-radius: unset;
    }
    .chat-header-text {
      font-family: @fontOpenSans;
      font-size: 12px;
      font-weight: bold;
      color: #ffffff;
      word-break: break-all;
      text-overflow: ellipsis;
      width: 200px;
      white-space: nowrap;
      overflow: hidden;
    }
    i {
      color: #ffffff;
      font-size: 16px;
      margin-right: 14px;
      z-index: 2;
      &.sbf-arrow-back-chat {
        &.rtl {
          transform: rotate(180deg);
        }
      }
    }
    .chat-header-name, .other-side {
      align-self: center
    }
    .chat-header-btn {
      button {
        background: #393850 !important; // vuetify
        height: 20px;
        vertical-align: text-top;
        width: 30px;
        align-self: center;
        color: #fff;
        opacity: 1;
        padding-right: 22px;
        box-shadow: none;
      }
      :hover {
          background: transparent !important;
        }
    }
    .other-side {
      margin-left: auto;
      i {
        margin-right: 0;
        margin-left: 14px;
      }
      .theme--light.v-btn-toggle {
        background: transparent !important;
      }
    }
  }
  .general-chat-style {
    height: 92%;
    width: 100%;
    @media (max-width: @screen-xs) {
     //Not sure why we need height in mobile but if not ios gets fucked up
      flex:2;
    }
  }
}
</style>
