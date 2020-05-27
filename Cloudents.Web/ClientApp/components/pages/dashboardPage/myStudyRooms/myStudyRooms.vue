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
              class="link white--text"
              v-if="isStudyroomLive"
              depressed
              rounded
              :block="$vuetify.breakpoint.xsOnly"
              color="#5360FC"
              v-t="'dashboardPage_my_studyrooms_create_live'"
            ></v-btn>
            <v-btn
              @click="openPrivateSession"
              class="link white--text"
              v-else
              depressed
              rounded
              :block="$vuetify.breakpoint.xsOnly"
              color="#5360FC"
              v-t="'dashboardPage_my_studyrooms_create_room'"
            ></v-btn>
          </div>
        </div>
      </template>

      <template v-slot:item.date="{item}">{{ $d(new Date(item.date)) }}</template>
      <template v-slot:item.name="{item}">{{item.name}}</template>

      <template v-slot:item.type="{item}">
        <div
          class="sessionType"
          :class="{'private': !isStudyroomLive}"
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

            <v-menu v-model="showMenu" offset-overflow>
                <template v-slot:activator="{ on }">
                    <div class="dotsIcon mr-2 ml-4 pb-5 pr-5 pr-sm-0" v-if="isTutor && isStudyroomLive">
                        <v-icon color="#bbb" v-on="on" @click="openDeleteMenu(item.id)" slot="activator" small icon>sbf-3-dot</v-icon>
                    </div>
                    <div class="dotsIcon mr-2 ml-2 pb-5" v-else>
                        <!-- <v-icon color="#bbb" v-on="on" @click="openDeleteMenu(item.id)" slot="activator" small icon>sbf-3-dot</v-icon> -->
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
      studyRoomType: '',
      snackbar: {
        value: false,
        text: ''
      },
      showMenu: false,
      menuShowId: null,
      currentItemId: null,
      paginationModel: {
        page: 1
      },
      headers: [
        {text: this.$t('studyRoom_created'), align:'left', sortable: true, value:'date'},
        {text: this.$t('dashboardPage_name'), align:'left', sortable: true, value:'name'},
        {text: this.$t('dashboardPage_type'), align:'left', sortable: true, value:'type'},
        {text: this.$t('dashboardPage_scheduled'), align:'left', sortable: true, value:'scheduled'},
        {text: this.$t('dashboardPage_students'), align:'left', sortable: true, value:'students'},
        {text: this.$t('dashboardPage_last_date'), align:'left', sortable: true, value:'lastSession'},
        {text: '', align:'center', sortable: false, value:'action'},
      ]
    };
  },
  computed: {
    ...mapGetters(["getStudyRoomItems"]),
    isStudyroomLive() {
      return this.studyRoomType === 'live'
    },
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
  watch: {
    "$route.meta.type"(val) {
      this.getSessions(val)
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
      }).catch(error => {
        let { response: {data}} = error
        self.snackbar.text = data["error"][0]
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
    },
    getSessions(type) {
      this.studyRoomType = type
      this.updateStudyRoomItems({type: this.studyRoomType});
    }
  },
  created() {
    this.getSessions(this.$route.meta.type)
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@import "../../../../styles/colors.less";

.myStudyRooms {
  padding: 30px;
  max-width: 1334px;
  @media (max-width: @screen-xs) {
    padding: 30px 0;
    width: 100%;
    height: 100%;
  }
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
      width: 10px;
      height: 10px;
      top: 3px;
      left: -20px;
      border-radius: 50%;
    }
    &.private {
      &::before {
        content: '';
        background: #5360fc;
      }
    }
  }
  .amountStudents {
    background: #0294ff;
    border-radius: 9px;
    display: inline-block;
    padding: 4px 10px;
  }
  .copyLink {
    width: 100px;

    @media(max-width: @screen-xs) {
      width: 86px;
    }
  }
  tr {
    height: 70px;

    .actionsWrapper {
      transition: opacity 0.5s ease-out;
      opacity: 0;
      display: none;
        
      @media(max-width: @screen-xs) {
        display: flex;
        opacity: 1;
      }
    }
    &:hover {
      .actionsWrapper {
        display: flex;
        opacity: 1;
        transition: opacity 0.5s ease-in;
      } 
    }
  }
  td {
    border: none !important;
  }
  // td:last-child {
  //     position: relative;
  //     .dotsIcon {
  //       position: absolute;
  //       right: 12px;
  //     }
  // }
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
      height: 54px;
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
