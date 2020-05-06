<template>
  <div class="myStudyRooms">
    <v-data-table
      calculate-widths
      :page.sync="paginationModel.page"
      :headers="headers"
      :items="studyRoomItems"
      :mobile-breakpoint="0"
      :items-per-page="20"
      sort-by
      :item-key="'date'"
      class="elevation-1"
      :footer-props="{
            showFirstLastPage: false,
            firstIcon: '',
            lastIcon: '',
            prevIcon: 'sbf-arrow-left-carousel',
            nextIcon: 'sbf-arrow-right-carousel',
            itemsPerPageOptions: [20]
         }"
    >
      <template v-slot:top>
        <div class="tableTop d-flex flex-sm-row flex-column align-sm-center justify-space-between">
          <div class="myStudyRooms_title pb-3 pb-sm-0" v-t="'schoolBlock_my_study_rooms'"></div>
          <div v-if="isTutor && isTutorStateOk">
            <v-btn
              @click="openLiveSession"
              class="link white--text mr-0 mr-sm-4 mb-4 mb-sm-0"
              depressed
              rounded
              :block="$vuetify.breakpoint.xsOnly"
              color="#5360FC"
              v-t="'dashboardPage_my_studyrooms_create_live'"
            ></v-btn>
            <v-btn
              @click="openPrivateSession"
              class="link white--text"
              depressed
              rounded
              :block="$vuetify.breakpoint.xsOnly"
              color="#5360FC"
              v-t="'dashboardPage_my_studyrooms_create_room'"
            ></v-btn>
            <!-- <v-btn
              v-if="!$vuetify.breakpoint.xsOnly"
              class="link btnTestStudyRoom"
              :to="{name: routeNames.StudyRoom}"
              depressed
              rounded
              outlined
              v-t="'dashboardPage_link_studyroom'"
            ></v-btn> -->
          </div>
        </div>
      </template>
      <!-- <template v-slot:item.preview="{item}">
            <user-avatar :user-id="item.userId"
                  :user-image-url="item.image" 
                  :size="'40'" 
                  :user-name="item.name" >
               </user-avatar>
      </template>-->
      <template v-slot:item.date="{item}">{{ $d(new Date(item.date)) }}</template>

      <template v-slot:item.name="{item}">{{item.name}}</template>

      <template v-slot:item.type="{item}">
        <div
          class="sessionType"
          :class="{'private': item.type === 'Private'}"
          v-t="item.type === 'Private' ? 'dashboardPage_type_private' : 'dashboardPage_type_broadcast'"
        >
        </div>
      </template>

      <template v-slot:item.scheduled="{item}">
        <div v-if="item.scheduled">{{ $d(new Date(item.scheduled)) }}</div>
      </template>

      <template v-slot:item.students="{item}">
        <div class="amountStudents white--text" v-if="item.students">{{item.students}}</div>
      </template>

      <template v-slot:item.lastSession="{item}">
        <template v-if="item.lastSession">{{ $d(new Date(item.lastSession)) }}</template>
      </template>
      
      <template v-slot:item.action="{item}">

        <div class="actionsWrapper d-flex align-center justify-center">
          
            <div class="studyRoomActions">
                <div v-if="item.showChat" class="mr-9">
                    <v-btn
                      icon
                      @click="sendMessage(item)"
                      :title="$t('schoolBlock_SendMessageTooltip')"
                    >
                        <iconChat fill="#4c59ff" />
                    </v-btn>
                    <div v-t="'schoolBlock_SendMessageTooltip'"></div>
                </div>

                <div v-else class="copyLink mr-8 flex-shrink-0">
                    <v-tooltip :value="currentItemId === item.id" top transition="fade-transition">
                        <template v-slot:activator="{}">
                            <linkSVG
                              style="width:20px;height:36px;"
                              class="option link"
                              @click="copyLink(item)"
                            />
                        </template>
                        <span v-t="'shareContent_copy_tool'"></span>
                    </v-tooltip>
                    <div v-t="'dashboardPage_link_share'"></div>
                </div>

                <div class="flex-shrink-0">
                  <v-btn
                    icon
                    @click="enterRoom(item.id)"
                    :title="$t('schoolBlock_EnterStudyRoomTooltip')"
                  >
                    <enterRoom width="18" />
                  </v-btn>
                  <div v-t="'schoolBlock_EnterStudyRoomTooltip'"></div>
                </div>
            </div>

            <v-menu v-model="showMenu" offset-overflow>
                <template v-slot:activator="{ on }">
                    <div class="dotsIcon mr-2 ml-4 pb-5" v-if="isTutor && item.type === 'Broadcast'">
                        <v-icon color="#bbb" v-on="on" @click="openDeleteMenu(item.id)" slot="activator" small icon>sbf-3-dot</v-icon>
                    </div>
                </template>
                <v-list v-if="menuShowId === item.id">
                  <v-list-item @click="deleteSession(item.id)" v-t="'dashboardPage_link_delete'"></v-list-item>
                </v-list>
            </v-menu>
        </div>

      </template>
      <slot slot="no-data" name="tableEmptyState" />
    </v-data-table>

    <v-snackbar
      v-model="snackbar.value"
      @input="snackbar.value = false"
      :timeout="5000"
      top
    >
      <div class="text-center flex-grow-1" v-t="snackbar.text"></div>
    </v-snackbar>
  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
import * as routeNames from "../../../../routes/routeNames";
import * as dialogNames from "../../global/dialogInjection/dialogNames.js";

import iconChat from "./images/icon-chat.svg";
import enterRoom from "./images/enterRoomGreen.svg";
import linkSVG from "../../global/shareContent/images/link.svg";

export default {
  name: "myStudyRooms",
  components: {
    iconChat,
    enterRoom,
    linkSVG
  },
  data() {
    return {
      snackbar: {
        value: false,
        text: ''
      },
      snackTest: '',
      showMenu: false,
      menuShowId: null,
      currentItemId: null,
      createStudyRoomDialog: dialogNames.CreateStudyRoom,
      routeNames,
      sortedBy: "",
      paginationModel: {
        page: 1
      },
      headers: [
        this.dictionary.headers["created"],
        this.dictionary.headers["name"],
        this.dictionary.headers["type"],
        this.dictionary.headers["scheduled"],
        this.dictionary.headers["students"],
        this.dictionary.headers["last_date"],
        this.dictionary.headers["action"]
      ]
    };
  },
  props: {
    dictionary: {
      type: Object,
      required: true
    }
  },
  computed: {
    ...mapGetters(["getStudyRoomItems"]),
    isTutorStateOk() {
      return this.$store.getters.accountUser?.isTutorState === 'ok';
    },
    isTutor() {
      return this.$store.getters.accountUser?.isTutor;
    },
    studyRoomItems() {
      return this.getStudyRoomItems;
    }
  },
  methods: {
    ...mapActions([
      "updateStudyRoomItems",
      "dashboard_sort",
      "openChatInterface",
      "setActiveConversationObj",
      "deleteStudyRoomSession"
    ]),
    openPrivateSession() {
      this.$store.commit('setComponent', 'createPrivateSession')
    },
    openLiveSession() {
      this.$store.commit('setComponent', 'createLiveSession')
    },
    deleteSession(id) {
      let self = this
      this.deleteStudyRoomSession(id).then(() => {
        let newItems = self.studyRoomItems.filter(item => item.id !== id)
        self.$store.commit('setStudyRoomItems', newItems)
        self.snackbar.text = 'dashboardPage_success_session_removed'
      }).catch(() => {
        self.snackbar.text = 'dashboardPage_error_session_removed'
        self.snackbar.color = "error"
      }).finally(() => {
        self.snackbar.value = true;
      })
    },
    openDeleteMenu(id) {
      this.menuShowId = id
      this.showMenu = true
    },
    sendMessage(item) {
      let currentConversationObj = {
        userId: item.userId,
        conversationId: item.conversationId,
        name: item.name,
        image: item.image || null
      };
      this.setActiveConversationObj(currentConversationObj);
      this.openChatInterface();
    },
    enterRoom(id) {
      let routeData = this.$router.resolve({
        name: routeNames.StudyRoom,
        params: { id }
      });
      global.open(routeData.href, "_self");
    },
    copyLink(item) {
      let link = `${window.origin}/studyroom/${item.id}`
      let self = this
      this.$copyText(link).then(({text}) => {
        self.currentItemId = item.id
        self.$ga.event('Share', 'Link', text);
        setTimeout(() => {
          self.currentItemId = null
        }, 2000);
      });
    }
  },
  created() {
    this.updateStudyRoomItems();
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@import "../../../../styles/colors.less";

.myStudyRooms {
  max-width: 1334px;
  .tableTop {
    padding: 30px;
    color: @global-purple !important;
    .myStudyRooms_title {
      font-size: 22px;
      font-weight: 600;
      line-height: 1.3px;
      @media (max-width: @screen-xs) {
        line-height: initial;
      }
      background: #fff;
    }
    .link {
      color: inherit;
      font-weight: 600;
      &.btnTestStudyRoom {
        border: 1px solid #5360FC;
        color: #5360FC;
      }
    }
  }
  .sessionType {
    position: relative;
    &::before {
      content: '';
      position: absolute;
      background: #68D568;
      width: 12px;
      height: 12px;
      top: 2px;
      left: -20px;
      border-radius: 50%;
    }
    &.private {
      &::before {
        content: '';
        background: #BFE4FF;
      }
    }
  }
  .amountStudents {
    background: #0294FF;
    border-radius: 12px;
    display: inline-block;
    padding: 8px 20px;
  }
  .copyLink {
    width: 100px;
  }
  tr {
    height: 54px;

    .actionsWrapper {
      width: 304px;
      .studyRoomActions {
        display: none;
        opacity: 0;
        transition: opacity 0.5s ease-out;
  
        @media(max-width: @screen-xs) {
          display: flex;
          opacity: 1;
        }
      }
  
    }
    &:hover {
      .studyRoomActions {
        display: flex;
        opacity: 1;
        transition: opacity 0.5s ease-in;
      } 
    }
  }
  td {
    border: none !important;
  }
  td:last-child {
      position: relative;
      .dotsIcon {
        position: absolute;
        right: 12px;
      }
  }
  td:first-child {
    white-space: nowrap;
  }
  tr:nth-of-type(2n) {
    td {
      background-color: #f5f5f5;
    }
  }
  thead {
    tr {
      th {
        color: #43425d !important;
        font-size: 14px;
        padding-top: 14px;
        padding-bottom: 14px;
        font-weight: normal;
        min-width: 100px;
      }
    }
    color: #43425d !important;
  }
  .gap {
    margin-left: 22px;
  }
  .sbf-arrow-right-carousel,
  .sbf-arrow-left-carousel {
    transform: none /*rtl:rotate(180deg)*/;
    color: #43425d !important;
    height: inherit;
    font-size: 14px !important;
  }
  .v-data-footer {
    padding: 6px 0;
    .v-data-footer__pagination {
      font-size: 14px;
      color: #43425d;
    }
  }
  .option {
    vertical-align: middle;
    cursor: pointer;
  }
}
</style>
