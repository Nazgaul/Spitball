<template>
   <div class="myContent">
      <v-data-table 
            :headers="headers"
            :items="contentItems"
            :items-per-page="5"
            :mobile-breakpoint="0"
            :item-key="'itemId'"
            sort-by
            class="elevation-1 myContent_table"
            :footer-props="{
               showFirstLastPage: false,
               firstIcon: '',
               lastIcon: '',
               prevIcon: 'sbf-arrow-left-carousel',
               nextIcon: 'sbf-arrow-right-carousel',
               itemsPerPageOptions: [5]
            }">

            <template v-slot:top>
               <div class="tableTop d-flex flex-sm-row flex-column align-sm-center justify-space-between">
                  <div class="myStudyRooms_title pb-3 pb-sm-0" v-t="'dashboardPage_my_content_title'"></div>
                  <div>
                     <v-btn
                        @click="$store.commit('setComponent', 'upload')"
                        class="white--text"
                        depressed
                        rounded
                        :block="$vuetify.breakpoint.xsOnly"
                        color="#5360FC"
                        v-t="'dashboardPage_my_content_upload'"
                     ></v-btn>
                  </div>
               </div>
            </template>

            <template v-slot:item.preview="{item}">
                  <img v-if="item.type === 'BuyPoints'" :src="item.image" class="tablePreview_img buyPointsLayoutPreview">
   
                  <router-link v-else :to="item.url" class="tablePreview">
                     <span v-if="item.online" class="tablePreview_online"></span>
                     <img v-if="item.image || item.preview || checkIsQuestion(item.type)" :src="formatImg(item)" class="tablePreview_img" width="80" height="80" />
                     
                     <v-avatar v-else tile tag="v-avatar" :class="'tablePreview_img tablePreview_no_image userColor' + strToACII(item.name)" :style="{width: `80px`, height: `80px`, fontSize: `22px`}">
                        <span class="white--text">{{item.name.slice(0,2).toUpperCase()}}</span>
                     </v-avatar>
                  </router-link>
            </template>

            <template v-slot:item.info="{item}">
               <div class="tableInfo text-xs-left text-truncate py-2">
                  <template v-if="item.type === 'BuyPoints'">
                     <div class="text-truncate">
                        <span>{{$t('dashboardPage_info_buy_points')}}</span>
                     </div>
                  </template>
                  <router-link v-else class="tableInfo_router" :to="item.url">
                     <template v-if="item.type === 'TutoringSession'">
                        <div class="text-truncate">
                           <div v-if="item.roomName" class="text-truncate">
                              <span class="font-weight-bold">{{$t('dashboardPage_room_name')}}</span> 
                              <span>{{item.roomName}}</span>
                           </div>
                           <span>{{$t('dashboardPage_session',[item.name])}}</span>
                           <div class="text-truncate">
                              <span class="font-weight-bold" v-t="'dashboardPage_duration'"></span>
                              <span v-if="item.duration">{{item.duration}}</span>
                              <span v-else v-t="'dashboardPage_session_on'"></span>
                           </div>
                        </div>
                     </template>
                     <template v-if="item.type === 'Document' || item.type === 'Video'">
                        <div class="text-truncate">
                           <span>{{item.name}}</span>
                        </div>
                     </template>
                     <template v-if="checkIsQuestion(item.type)">
                        <div class="text-truncate">
                           <span class="font-weight-bold" v-t="'dashboardPage_question'"></span>
                           <span class="text-truncate">{{item.text}}</span>
                        </div>
                        <div class="text-truncate" v-if="item.answerText">
                           <span class="font-weight-bold" v-t="'dashboardPage_answer'"></span>
                           <span>{{item.answerText}}</span>
                        </div>
                     </template>
                     <template v-if="item.conversationId">
                        <div class="text-truncate">
                           <span>{{item.name}}</span>
                        </div>
                     </template>
                     <div class="text-truncate" v-if="item.course">
                        <span class="font-weight-bold" v-t="'dashboardPage_course'"></span>
                        <span>{{item.course}}</span>
                     </div>
                  </router-link>
               </div>
            </template>

            <template v-slot:item.type="{item}">{{dictionary.types[item.type]}}</template>
            <template v-slot:item.likes="{item}">{{item.likes}}</template>
            <template v-slot:item.views="{item}">{{item.views}}</template>
            <template v-slot:item.downloads="{item}">{{item.downloads}}</template>
            <template v-slot:item.purchased="{item}">{{item.purchased}}</template>
            <template v-slot:item.price="{item}">{{formatPrice(item.price,item.type)}}</template>
            <template v-slot:item.date="{item}">{{ $d(item.date) }}</template>

            <template v-slot:item.action="{item}">
               <v-menu bottom left v-model="showMenu" v-if="!checkIsQuestion(item.type)">
                  <template v-slot:activator="{ on }">
                     <v-icon @click="currentItemIndex = item.itemId" v-on="on" slot="activator" small icon>{{$vuetify.icons.values.dotMenu}}</v-icon>
                  </template>
               
                  <v-list v-if="item.itemId == currentItemIndex">
                     <v-list-item style="cursor:pointer;" @click="openChangeNameDialog(item)" v-t="'dashboardPage_rename'"></v-list-item>
                     <v-list-item style="cursor:pointer;" @click="openChangePriceDialog(item)" v-t="'resultNote_change_price'"></v-list-item>
                  </v-list>
               </v-menu>
            </template>
            <slot slot="no-data" name="tableEmptyState"/>
      </v-data-table>

      <sb-dialog 
         :showDialog="isChangeNameDialog"
         :isPersistent="true"
         :popUpType="'dashboardDialog'"
         :onclosefn="closeDialog"
         :activateOverlay="true"
         :max-width="'fit-content'"
         :content-class="'pop-dashboard-container'">
            <changeNameDialog :dialogData="currentItem" @closeDialog="closeDialog"/>
      </sb-dialog>
      <sb-dialog 
         :showDialog="isChangePriceDialog"
         :isPersistent="true"
         :popUpType="'dashboardDialog'"
         :onclosefn="closeDialog"
         :activateOverlay="true"
         :max-width="'fit-content'"
         :content-class="'pop-dashboard-container'">
            <changePriceDialog :dialogData="currentItem" @closeDialog="closeDialog"/>
      </sb-dialog>
   </div>
</template>

<script>
import { mapGetters } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import changeNameDialog from '../dashboardDialog/changeNameDialog.vue';
import changePriceDialog from '../dashboardDialog/changePriceDialog.vue';

export default {
   name:'myContent',
   components:{sbDialog,changeNameDialog,changePriceDialog},
   props:{
      dictionary:{
         type: Object,
         required: true
      }
   },
   data() {
      return {
         currentItem: '',
         isChangeNameDialog: false,
         isChangePriceDialog: false,
         currentItemIndex: '',
         showMenu: false,
         headers: [
            this.dictionary.headers['preview'],
            this.dictionary.headers['info'],
            this.dictionary.headers['type'],
            this.dictionary.headers['likes'],
            this.dictionary.headers['views'],
            this.dictionary.headers['downloads'],
            this.dictionary.headers['purchased'],
            this.dictionary.headers['price'],
            this.dictionary.headers['date'],
            this.dictionary.headers['action'],
         ]
      }
   },
   computed: {
      ...mapGetters(['getContentItems','accountUser']),
      contentItems(){
         // avoiding duplicate key becuase we have id that are the same,
         // vuetify default key is "id", making new key "itemId" for unique index table items
         return this.getContentItems && this.getContentItems.map((item, index) => {
            return {
               itemId: index,
               ...item
            }
         })
      }
   },
   methods: {
      formatPrice(price,type){
         if(isNaN(price)) return;
         if(price < 0){
            price = Math.abs(price)
         }
         price = Math.round(+price).toLocaleString();
         let currency;
         if(type === 'Document' || type === 'Video' ){
            currency = this.$t('dashboardPage_pts')
         }
         if(type === 'TutoringSession' || type === 'BuyPoints'){
            currency = this.accountUser.currencySymbol
         }
         return `${price} ${currency}`
      },
      openChangeNameDialog(item){
         this.currentItem = item;
         this.isChangeNameDialog = true;
      },
      openChangePriceDialog(item){
         this.currentItem = item;
         this.isChangePriceDialog = true;
      },
      closeDialog(){
         this.isChangeNameDialog = false;
         this.isChangePriceDialog = false;
         this.currentItem = '';
      },
      checkIsQuestion(type){
         return type === 'Question' || type === 'Answer';
      },
      formatImg(item){
         if(item.preview || item.image){
            return this.$proccessImageUrl(item.preview,80,80)
         }
         if(this.checkIsQuestion(item.type)){
            return require('../global/images/qs.png') 
         }
      }
   },
   created() {
      this.$store.dispatch('updateContentItems')
   },
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@import "../../../../styles/colors.less";
.pop-dashboard-container {
   background: #fff;
}
.myContent{
   max-width: 1366px;
   .myContent_title{
      font-size: 22px;
      color: @global-purple;
      font-weight: 600;
      padding: 30px;
      line-height: 1.3px;
   }
   .myContent_table{
      thead {
         tr {
            height: auto;
            th{
               color: @global-purple !important;
               font-size: 14px;
               padding-top: 14px;
               padding-bottom: 14px;
               font-weight: normal;
            }
         }
         color: @global-purple !important;
      }
      .tablePreview{
         line-height: 0;
         padding-right: 0 !important;
         width: 104px;
         position: relative;
         .tablePreview_online{
            position: absolute;
            border-radius: 50%;
            width: 10px;
            height: 10px;
            background-color: #00ff14;
            top: 16px;
            left: 28px;
         }
         .tablePreview_img{
            margin: 10px 0;
            border: 1px solid #d8d8d8;
            &.buyPointsLayoutPreview{
               width: 100%;
               object-fit: cover;
               height: 80px;      
            }
         }
         .tablePreview_no_image {
            position: unset;
            border-radius: 4px;
            font-size: 24px;
         }
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

      tbody tr {
         td:nth-child(2) {
            padding-left: 0;
         }
      }
      .tableInfo{
         width: 400px;
         max-width: 400px;
         min-width: 300px;
         .tableInfo_router{
            color: @global-purple !important;
            line-height: 1.6;
         }
      }

      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         transform: none /*rtl:rotate(180deg)*/;
         color: @global-purple !important;
         height: inherit;
         font-size: 14px;
      }
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         transform: none /*rtl:rotate(180deg)*/;
         color: @global-purple !important;
         height: inherit;
         font-size: 14px;
      }
      .v-data-footer {
         padding: 6px 0;
         .v-data-footer__pagination {
            font-size: 14px;
            color: @global-purple;
         }
      }
   }
}
</style>