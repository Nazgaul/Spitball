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
                     <v-icon v-if="header.sortable" v-html="sortedBy !== header.value?'sbf-arrow-down':'sbf-arrow-up'" />
                  </span>
               </th>
            </tr>
         </template>
            <template v-slot:items="props">
               <td class="myContent_td_img">
                  <router-link :to="globalFunctions.router(props.item)" class="myContent_td_img_img">
                     <img width="80" height="80" :src="globalFunctions.formatImg(props.item)">
                  </router-link>
               </td>
               
               <td class="text-xs-left myContent_td_course text-truncate">
                  <router-link :to="globalFunctions.router(props.item)">
                     <template v-if="checkIsQuestion(props.item.type)">
                        <div class="text-truncate">
                           <span class="font-weight-bold" v-language:inner="'dashboardPage_question'"/>
                           <span class="text-truncate">{{props.item.text}}</span>
                        </div>
                        <div class="text-truncate" v-if="props.item.answerText">
                           <span class="font-weight-bold" v-language:inner="'dashboardPage_answer'"/>
                           <span>{{props.item.answerText}}</span>
                        </div>
                     </template>

                     <template v-else>
                        <span>{{props.item.name}}</span>
                     </template>
                     <div class="text-truncate" v-if="props.item.course">
                        <span class="font-weight-bold" v-language:inner="'dashboardPage_course'"></span>
                        <span>{{props.item.course}}</span>
                     </div>
                  </router-link>
               </td>
               <td class="text-xs-left" v-html="dictionary.types[props.item.type]"/>
               <td class="text-xs-left">{{props.item.likes}}</td>
               <td class="text-xs-left">{{props.item.views}}</td>
               <td class="text-xs-left">{{props.item.downloads}}</td>
               <td class="text-xs-left">{{props.item.purchased}}</td>
               <td class="text-xs-left" v-html="globalFunctions.formatPrice(props.item.price,props.item.type)"/>
               <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
               <td class="text-xs-center">
                  <v-menu lazy bottom right v-model="showMenu" v-if="!checkIsQuestion(props.item.type)" >
                     <v-icon @click="currentItemIndex = props.index" slot="activator" small icon>sbf-3-dot</v-icon>

                     <v-list v-if="props.index == currentItemIndex">
                        <v-list-tile style="cursor:pointer;" @click="globalFunctions.openDialog('rename',props.item)">{{rename}}</v-list-tile>
                        <v-list-tile style="cursor:pointer;" @click="globalFunctions.openDialog('changePrice',props.item)">{{changePrice}}</v-list-tile>
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
   props:{
      globalFunctions: {
         type: Object,
      },
      dictionary:{
         type: Object,
         required: true
      }
   },
   data() {
      return {
         sortedBy:'',
         currentItemIndex:'',
         showMenu:false,
         headers:[
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
         ],
         changePrice:LanguageService.getValueByKey("resultNote_change_price"),
         rename:LanguageService.getValueByKey("dashboardPage_rename"),
      }
   },
   computed: {
      ...mapGetters(['getContentItems']),
      contentItems(){
         return this.getContentItems
      },
   },
   methods: {
      ...mapActions(['updateContentItems','dashboard_sort']),
      checkIsQuestion(prop){
         return prop === 'Question' || prop === 'Answer';
      },
      changeSort(sortBy){
         if(sortBy === 'info') return;

         let sortObj = {
            listName: 'contentItems',
            sortBy,
            sortedBy: this.sortedBy
         }
         this.dashboard_sort(sortObj)
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
               border: 1px solid #d8d8d8;
            }

         }
      }
      .myContent_td_course {
         a{
            color: #43425d !important;
            line-height: 1.6;
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