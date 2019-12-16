<template>
   <div class="myContent">
      <div class="myContent_title" v-language:inner="'dashboardPage_my_content_title'"/>
      <v-data-table v-if="contentItems.length"
            :headers="headers"
            :items="contentItems"
            disable-initial-sort
            :item-key="'date'"
            :rows-per-page-items="['5']"
            class="elevation-1 myContent_table"
            :prev-icon="'sbf-arrow-left-carousel'"
            :sort-icon="'sbf-arrow-down'"
            :next-icon="'sbf-arrow-right-carousel'">
         <template slot="headers" slot-scope="props">
            <tr>
               <th class="text-xs-left"
                  v-for="header in props.headers"
                  :key="header.text"
                  :class="['column',{'sortable':header.sortable}]"
                  @click="changeSort(header.value)">
                  <span class="text-xs-left">{{ header.text }}
                     <v-icon v-if="header.sortable" small>sbf-arrow-down</v-icon>
                  </span>
               </th>
            </tr>
         </template>
            <template v-slot:items="props">
               <td class="myContent_td_img">
                  <router-link :to="dynamicRouter(props.item)" class="myContent_td_img_img">
                     <img width="80" height="80" :src="formatItemImg(props.item)" :class="{'imgPreview_content':props.item.preview}">
                  </router-link>
               </td>
               
               <td class="text-xs-left myContent_td_course text-truncate">
                  <router-link :to="dynamicRouter(props.item)">
                     <template v-if="checkIsQuestuin(props.item.type)">
                        <div class="text-truncate">
                           <span v-language:inner="'dashboardPage_questuin'"/>
                           <span class="text-truncate">{{props.item.text}}</span>
                        </div>
                        <div class="text-truncate">
                           <span v-language:inner="'dashboardPage_answer'"/>
                           <span>{{props.item.answerText}}</span>
                        </div>
                     </template>

                     <template v-else>
                        <span>{{props.item.name}}</span>
                     </template>
                        <div class="text-truncate">
                           <span v-language:inner="'dashboardPage_course'"></span>
                           <span>{{props.item.course}}</span>
                        </div>
                  </router-link>
               </td>
               <td class="text-xs-left">{{formatItemType(props.item.type)}}</td>
               <td class="text-xs-left">{{props.item.likes}}</td>
               <td class="text-xs-left">{{props.item.views}}</td>
               <td class="text-xs-left">{{props.item.downloads}}</td>
               <td class="text-xs-left">{{props.item.purchased}}</td>
               <td class="text-xs-left">{{ formatItemPrice(props.item.price,props.item.type)}}</td>
               <!-- <td class="text-xs-left" v-html="formatItemStatus(props.item.state)"/> -->
               <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
               <td v-if="!checkIsQuestuin(props.item.type)" class="text-xs-center">
                  <v-menu lazy bottom right v-model="showMenu">
                     <v-icon @click="currentItemIndex = props.index" slot="activator" small icon>sbf-3-dot</v-icon>

                     <v-list v-if="props.index == currentItemIndex">
                        <v-list-tile style="cursor:pointer;" @click="$emit('openDialog',['rename',props.item])">{{rename}}</v-list-tile>
                        <v-list-tile style="cursor:pointer;" @click="$emit('openDialog',['changePrice',props.item])">{{changePrice}}</v-list-tile>
                     </v-list>
                  </v-menu>
               </td>
            </template>

         <template slot="pageText" slot-scope="item">
            <span class="myContent_footer">
            {{item.pageStop}} <span v-language:inner="'dashboardPage_of'"/> {{item.itemsLength}}
            </span>
         </template>

      </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from '../../../../services/language/languageService';

export default {
   name:'myContent',
   data() {
      return {
         itemList:[],
         sortedBy:'',
         currentItemIndex:'',
         showMenu:false,
         headers:[
            {text: LanguageService.getValueByKey('dashboardPage_preview'), align:'left', sortable: false, value:'preview'},
            {text: LanguageService.getValueByKey('dashboardPage_info'), align:'left', sortable: false, value:'info'},
            {text: LanguageService.getValueByKey('dashboardPage_type'), align:'left', sortable: true, value:'type'},
            {text:LanguageService.getValueByKey('dashboardPage_likes'), align:'left', sortable: true, value:'likes'},
            {text:LanguageService.getValueByKey('dashboardPage_views'), align:'left', sortable: true, value:'views'},
            {text:LanguageService.getValueByKey('dashboardPage_downloads'), align:'left', sortable: true, value:'downloads'},
            {text:LanguageService.getValueByKey('dashboardPage_purchased'), align:'left', sortable: true, value:'purchased'},
            {text:LanguageService.getValueByKey('dashboardPage_price'), align:'left', sortable: true, value:'price'},
            // {text: LanguageService.getValueByKey('dashboardPage_status'), align:'left', sortable: true, value:'status'},
            {text: LanguageService.getValueByKey('dashboardPage_date'), align:'left', sortable: true, value:'date'},
            {text: LanguageService.getValueByKey('dashboardPage_action'), align:'center', sortable: false, value:'action'},
         ],
         changePrice:LanguageService.getValueByKey("resultNote_change_price"),
         rename:LanguageService.getValueByKey("dashboardPage_rename"),
      }
   },
   computed: {
      ...mapGetters(['getContentItems']),
      contentItems:{
         get(){
            this.itemList = this.getContentItems
            return this.itemList;
         },
         set(val){
            this.itemList = val
         }
      },
   },
   methods: {
      ...mapActions(['updateContentItems']),
      dynamicRouter(item){
         if(item.url){
            return item.url;
         }
         if(item.type === 'Question'){
            return {path:'/question/'+item.id}
         }
      },
      formatItemImg(item){
         if(item.preview){
            return this.$proccessImageUrl(item.preview,140,140,"crop&anchorPosition=top")
         }
         if(item.type === 'Question'){
            return require(`../images/qs.png`) 
         }
      },
      checkIsQuestuin(prop){
         return prop === 'Question';
      },
      formatItemType(type){
         if(type === 'Question'){
            return LanguageService.getValueByKey('dashboardPage_qa')
         }
         if(type === 'Document'){
            return LanguageService.getValueByKey('dashboardPage_document')
         }
         if(type === 'Video'){
            return LanguageService.getValueByKey('dashboardPage_video')
         }
      },
      // formatItemStatus(state){
      //    if(state === 'Ok'){
      //       return LanguageService.getValueByKey('dashboardPage_ok')
      //    }
      //    if(state === 'Deleted'){
      //       return LanguageService.getValueByKey('dashboardPage_deleted')
      //    }
      //    if(state === 'Flagged'){
      //       return LanguageService.getValueByKey('dashboardPage_flagged')
      //    }
      //    if(state === 'Pending'){
      //       return LanguageService.getValueByKey('dashboardPage_pending ')
      //    }
      // },
      formatItemPrice(price,type){
         if(type !== 'Question'){
            return `${Math.round(+price)} ${LanguageService.getValueByKey('dashboardPage_pts')}`
         }
      },
      changeSort(sortBy){
         if(sortBy == 'date'){
            if(this.sortedBy === sortBy){
               this.itemList = this.itemList.sort((a,b)=> new Date(a[sortBy]) - new Date(b[sortBy]))
            }else{
               this.itemList = this.itemList.sort((a,b)=> new Date(b[sortBy]) - new Date(a[sortBy]))
            }
         }
         if(sortBy == 'type'){
            if(this.sortedBy === sortBy){
               this.itemList = this.itemList.sort((a,b)=> {
                  if(a[sortBy] > b[sortBy]){
                     return -1;
                  }
                  if(b[sortBy] > a[sortBy]){
                     return 1;
                  }
                  return 0;
               })
            }else{
               this.itemList = this.itemList.sort((a,b)=> {
                  if(b[sortBy] > a[sortBy]){
                     return -1;
                  }
                  if(a[sortBy] > b[sortBy]){
                     return 1;
                  }
                  return 0;
               })
            }
         }
         this.sortedBy = this.sortedBy === sortBy ? '' : sortBy;
      }
   },
   created() {
      this.updateContentItems()
   },
}
</script>

<style lang="less">
.myContent{
   .myContent_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 0 0 10px 2px;
   }
   .myContent_table{
      .v-datatable{
         tr{
            height: auto;
            th{
               color: #43425d !important;
               font-size: 14px;
               padding-top: 14px;
               padding-bottom: 14px;
            }
            
         }
         color: #43425d !important;
      }
      .myContent_footer{
         font-size: 14px;
         color: #43425d !important;
      }
      .myContent_td_img{
         line-height: 0;
         padding-right: 0 !important;
         .myContent_td_img_img{
            img{
               margin: 10px 0;
               &.imgPreview_content{
                  object-fit: none;
                  object-position: top;
               }
            }

         }
      }
      .myContent_td_course {
         a{
            color: #43425d !important;
         }
         width: 300px;
         max-width: 300px;
         min-width: 300px;
      }
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         transform: none /*rtl:rotate(180deg)*/;
         color: #43425d !important;
         height: inherit;
         font-size: 14px;
      }

   }
}
</style>