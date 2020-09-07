<template>
  <div class="myStudyRooms">
    <v-skeleton-loader
      v-if="skeleton"
      class="mx-auto"
      max-width="1274"
      type="table"
    ></v-skeleton-loader>
    <v-data-table
      v-else
      calculate-widths
      :page.sync="paginationModel.page"
      :headers="headers"
      :items="studyRoomItems"
      :mobile-breakpoint="0"
      :items-per-page="20"
      sort-by
      :item-key="'itemId'"
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
          <div class="myStudyRooms_title pb-3 pb-sm-0">{{$t("dashboardPage_title_private")}}</div>
          <div v-if="isTutor">
            <v-btn
              @click="openPrivateSession"
              class="link white--text"
              depressed
              rounded
              :block="$vuetify.breakpoint.xsOnly"
              color="#5360FC"
            >
              <v-icon size="24" left>sbf-plus-circle</v-icon>
              <span>{{$t("dashboardPage_my_studyrooms_create_room")}}</span>
            </v-btn>
          </div>
        </div>
      </template>

      <template v-slot:item.date="{item}">{{$moment(item.date).format('MMM D, YYYY')}}</template>
      <template v-slot:item.name="{item}">{{item.name}}</template>

      <template v-slot:item.type>
        <div
          class="sessionType private"
          v-t="$t('dashboardPage_type_private')"
        ></div>
      </template>

      <template v-slot:item.students="{item}">
        <v-tooltip :value="currentItemId === item.id" top transition="fade-transition">
          <template v-slot:activator="{on}">
            <div
              v-on="item.userNames && item.userNames.length ? on : null"
              class="amountStudents white--text"
            >
              {{item.amountStudent || 0}}
            </div>
          </template>
          <div v-for="(user, index) in item.userNames" :key="index">{{user}}</div>          
        </v-tooltip>
      </template>

      <template v-slot:item.lastSession="{item}">
        <template v-if="item.lastSession">{{$moment(item.lastSession).format('MMM D, YYYY')}}</template>
      </template>

      <template v-slot:item.price="{item}">{{$price(item.price,item.currency,true)}}</template>

      <template v-slot:item.action="{item}">
        <div class="actionsWrapper d-flex align-center justify-center">
          <div class="me-9">
            <v-btn icon @click="sendMessage(item)" :title="$t('schoolBlock_SendMessageTooltip')">
              <iconChat fill="#4c59ff" />
            </v-btn>
            <div v-t="'schoolBlock_SendMessageTooltip'"></div>
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
          
          <div class="dotsIcon me-2 ms-2 pb-5">
          </div>
        </div>
      </template>

      <slot slot="no-data" name="tableEmptyState" />
    </v-data-table>
  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
import * as routeNames from "../../../../routes/routeNames";
import * as componentConsts from '../../global/toasterInjection/componentConsts.js';

import iconChat from "./images/icon-chat.svg";
import enterRoom from "./images/enterRoomGreen.svg";

export default {
  name: "myStudyRooms",
  components: {
    iconChat,
    enterRoom,
  },
  data() {
    return {
      skeleton: true,
      currentItemId: null,
      paginationModel: {
        page: 1
      }
    };
  },
  computed: {
    ...mapGetters(["getStudyRoomItems"]),
    headers() {
      let headersBuilder = [
        {
          text: this.$t("studyRoom_created"),
          align: "",
          sortable: true,
          value: "date"
        },
        {
          text: this.$t("dashboardPage_name"),
          align: "",
          sortable: true,
          value: "name"
        },
        {
          text: this.$t("dashboardPage_type"),
          align: "",
          sortable: true,
          value: "type"
        }
      ];
      headersBuilder = headersBuilder.concat([
        {
          text: this.$t("dashboardPage_students"),
          align: "",
          sortable: true,
          value: "students"
        },
        {
          text: this.$t("dashboardPage_last_date"),
          align: "",
          sortable: true,
          value: "lastSession"
        },
        {
          text: this.$t("study room price"),
          align: "",
          sortable: true,
          value: "price"
        },
        { text: "", align: "center", sortable: false, value: "action" }
      ]);
      return headersBuilder;
    },
    isTutor() {
      return this.$store.getters.accountUser?.isTutor;
    },
    
    studyRoomItems() {
        // avoiding duplicate key becuase we have id that are the same,
        // vuetify default key is "id", making new key "itemId" for unique index table items
        return this.getStudyRoomItems && this.getStudyRoomItems.map((item, index) => {
            return {
               itemId: index,
               ...item
            }
         })
    }
  },
  methods: {
    ...mapActions([
      "updateStudyRoomItems",
      "dashboard_sort",
      "setActiveConversationObj",
    ]),
    openPrivateSession() {
      this.$store.commit("addComponent", componentConsts.SESSION_CREATE_DIALOG);
    },
    sendMessage(item) {
      let currentConversationObj = {
        userId: item.userId,
        conversationId: item.conversationId,
        name: item.name,
        image: item.image || null
      };
      this.setActiveConversationObj(currentConversationObj);
      this.$router.push({
        name: routeNames.MessageCenter,
        params: { id: currentConversationObj.conversationId }
      });
    },
    enterRoom(id) {
      let routeData = this.$router.resolve({
        name: routeNames.StudyRoom,
        params: { id }
      });
      global.open(routeData.href, "_self");
    },
  },
  created() {
    let self = this;
    self.updateStudyRoomItems({ type: 'private' }).then(() => {
        self.skeleton = false
      })
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@import "../../../../styles/colors.less";

.myStudyRooms {
  padding: 30px;
  max-width: 1080px;
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
        border: 1px solid #5360fc;
        color: #5360fc;
      }
    }
  }
  .sessionType {
    position: relative;
    &::before {
      content: "";
      position: absolute;
      background: #68d568;
      width: 10px;
      height: 10px;
      top: 3px;
      left: -20px;
      border-radius: 50%;
    }
    &.private {
      &::before {
        content: "";
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
  tr {
    .actionsWrapper {
      font-size: 12px;
      color: #69687d;
    }
  }
  td {
    border: none !important;
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
      height: 54px;
      th {
        color: #43425d !important;
        font-size: 14px;
        padding-top: 14px;
        padding-bottom: 14px;
        font-weight: normal;
        border-top: thin solid rgba(0, 0, 0, 0.12);

        &:nth-child(2) {
          width: 120px;
        }
      }
    }
    color: #43425d !important;
  }
  .gap {
    margin-left: 22px;
  }
  .sbf-arrow-right-carousel,
  .sbf-arrow-left-carousel {
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
