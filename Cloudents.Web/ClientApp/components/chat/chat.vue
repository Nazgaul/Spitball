<template>
  <div class="sb-chat-container px-0 py-0" :class="{'minimized': isMinimized}">
    <v-layout @click="toggleMinimizeChat" class="chat-header" :class="{'new-messages': hasUnread}">
      <v-icon
        class="mr-2"
        size="18"
        color="#fff"
        @click.stop="OriginalChatState"
        v-html="inConversationState ? 'sbf-message-icon' : 'sbf-arrow-back-chat'"
      />
      <template v-if="state === 'messages'">
        <user-avatar
          :size="'32'"
          :user-name="activeConversationObj.name"
          :user-id="activeConversationObj.userId"
          :userImageUrl="activeConversationObj.image"
        />
        <div class="chat-header-name text-truncate pl-4">{{chatTitle}}</div>
      </template>
      <template v-else>
        <span class="chat-header-text">{{getIsSignalRConnected ? headerTitle : errorTitle}}</span>
      </template>

      <span class="other-side">
        <v-icon
          class="minimizeIcon"
          v-show="!isMobile"
          @click.stop="toggleMinimizeChat"
          size="18"
          color="#fff"
        >{{isMinimized ? 'sbf-toggle-enlarge' : 'sbf-minimize'}}</v-icon>
        <v-icon
          size="18"
          color="#fff"
          class="closeIcon"
          v-if="!isLocked"
          @click.stop="closeChatWindow"
        >sbf-close-chat</v-icon>
      </span>
    </v-layout>

    <v-layout v-if="!isMinimized" class="general-chat-style">
      <component :is="`chat-${state}`"></component>
    </v-layout>
  </div>
</template>


<script>
const chatConversation = () => import("./components/conversations.vue");
const chatMessages = () => import("./components/messages.vue");
import { mapGetters, mapActions } from "vuex";
import { LanguageService } from "../../services/language/languageService";
export default {
  components: {
    chatConversation,
    chatMessages
  },
  data() {
    return {
      enumChatState: this.getEnumChatState(),
      mobileHeaderHeight: 39
    };
  },
  computed: {
    ...mapGetters([
      "getChatState",
      "getIsChatMinimized",
      "getActiveConversationObj",
      "getIsChatLocked",
      "accountUser",
      "getTotalUnread",
      "getIsSignalRConnected"
    ]),
    isLocked() {
      return this.getIsChatLocked;
    },
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    state() {
      return this.getChatState;
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
      return "";
    },
    chatTitle() {
      if (this.$store.getters.getRoomIsBroadcast) {
        return this.$store.getters.getRoomName;
      } else {
        return this.getActiveConversationObj.name;
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
    activeConversationObj() {
      return this.getActiveConversationObj;
    }
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
  position: fixed;
  bottom: 0;
  right: 130px;
  width: 320px;
  height: 520px;
  z-index: 99;
  background: #fff;
  border-radius: 10px 10px 0 0;
  box-shadow: 0 3px 16px 0 rgba(0, 0, 0, 0.3);
  max-height: ~"calc( 100vh - 100px)";

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
    background-color: @global-purple;
    align-items: center;
    border-radius: 4px 4px 0 0;
    padding: 6px;
    color: #fff;
    z-index: 1;
    .heightMinMax(48px);
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
      font-size: 14px;
      color: #ffffff;
      word-break: break-all;
      text-overflow: ellipsis;
      width: 200px;
      white-space: nowrap;
      overflow: hidden;
    }
    .sbf-message-icon {
      margin: 4px 10px 0 4px;
      z-index: 2;
    }
    .sbf-arrow-back-chat {
      transform: scaleX(1) /*rtl:append:scaleX(-1)*/;
      display: flex;
    }
    .chat-header-name,
    .other-side {
      align-self: center;
    }
    .other-side {
      display: flex;
      margin-left: auto;
      .minimizeIcon,
      .closeIcon {
        margin-right: 0;
        margin-left: 14px;
      }
      .theme--light.v-btn-toggle {
        background: #393850 !important;
      }
    }
  }
  .general-chat-style {
    height: ~"calc( 100% - 48px)";
    width: 100%;
    @media (max-width: @screen-xs) {
      flex: 2;
    }
  }
}
.tutoring-page + .sb-chat-container {
  right: 0;
  left: unset;
  z-index: 201;
  @media (max-width: @screen-sm) and (orientation: portrait) {
    // width: 100%;
    // height: 100%;
    // max-height: unset;
    height: unset;
    top: 50%;
    left: 0;
    bottom: 0;
    right: 0;
  }
  @media (max-width: @screen-sm) and (orientation: landscape) {
    // width: 100%;
    // height: 100%;
    // max-height: unset;
    display: none;
  }
}
</style>
