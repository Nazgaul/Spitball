<template>
<div id="profileItemsBox">
   <div class="profileItemsBox_title text-truncate" 
   v-text="$Ph($vuetify.breakpoint.xsOnly? 'profile_study_materials_mobile':'profile_study_materials',userName)"/>   
   <div class="profileItemsBox_filters">
      <v-flex xs2 sm4 class="pr-0 pr-sm-4 d-flex d-sm-block" :class="{'filterbox':$vuetify.breakpoint.xsOnly}" justify-end>
         <v-menu offset-y sel="filter_type">
            <template v-slot:activator="{ on }">
               <v-btn icon v-on="on" class="filters_menu_btn d-block d-sm-none">
                  <v-icon class="icon">sbf-sort</v-icon>
               </v-btn>
            </template>
            <v-list class="px-2">
               <v-list-item v-for="(item, index) in typeItems" :key="index" @click="menuSelect(item.value)">
               <v-list-item-title :class="{'font-weight-bold': selectChecker(item)}">{{ item.name }}</v-list-item-title>
               </v-list-item>
            </v-list>
         </v-menu>

         <v-select class="profileItemsBox_filters_select d-none d-sm-flex"
            sel="filter_type"
            :append-icon="'sbf-arrow-fill'"
            v-model="selectedModel.itemType"
            :items="typeItems"
            item-text="name"
            @change="handleSelects()"
            :height="$vuetify.breakpoint.xsOnly? 42 : 36" hide-details solo>
         </v-select>
      </v-flex>
      <v-flex xs10 sm9 class="pr-4 pr-sm-0" :class="{'filtercourse':$vuetify.breakpoint.xsOnly}">
         <v-select class="profileItemsBox_filters_select"
            sel="filter_course"
            :append-icon="'sbf-arrow-fill'"
            clearable
            :clear-icon="'sbf-close'"
            v-model="selectedModel.itemCourse"
            :items="userCourses"
            @change="handleSelects()"
            :placeholder="selectPlaceholder" :height="$vuetify.breakpoint.xsOnly? 42 : 36" solo>
         </v-select>
      </v-flex>
   </div>
   <template v-if="!!items && items.length">
      <div class="profileItemsBox_content" v-if="$vuetify.breakpoint.smAndUp">
         <itemCard v-for="(item, index) in items" :key="index" :item="item"/>
      </div>
      <div v-if="$vuetify.breakpoint.xsOnly" class="profileItemsBox_content_mobile">
         <resultNote v-for="(item, index) in items" :key="index" :item="item" class="pa-3 mb-2"/>
      </div>
      <div class="profileItemBox_pagination mb-3" v-if="pageCount > 1">
         <v-pagination
            total-visible=7 
            v-model="query.page" 
            :length="pageCount"
            :next-icon="`sbf-arrow-right-carousel`"
            :prev-icon="`sbf-arrow-left-carousel`"/>
      </div>
   </template>
</div>
</template>

<script>
import itemCard from '../../../carouselCards/itemCard.vue';
import resultNote from "../../../results/ResultNote.vue";
import { LanguageService } from "../../../../services/language/languageService";

import { mapGetters } from 'vuex'
export default {
   name:'profileItemsBox',
   components:{
      itemCard,
      resultNote
   },
   props:{
      globalFunctions:{
         type: Object,
         required:true
      }
   },
   data() {
      return {
         selectPlaceholder: LanguageService.getValueByKey('profile_select_course'),
         typeItems:[
            {name: LanguageService.getValueByKey('profile_select_item_type_all'),value:''},
            {name: LanguageService.getValueByKey('profile_select_item_type_docs'),value:0},
            {name: LanguageService.getValueByKey('profile_select_item_type_videos'),value:1},
            // {name:'Answer',value:'answers'},
            // {name:'Question',value:'questions'},
         ],
         selectedModel:{
            itemType:{name: LanguageService.getValueByKey('profile_select_item_type_all'),value:''},
            itemCourse:''
         },
         query:{
            course:'',
            documentType:'',
            page: 1,
            pageSize: this.$vuetify.breakpoint.xsOnly? 3 : 6,
         }
      }
   },
   watch: {
      query:{
         deep:true,
         handler() {
            this.updateItems()
         }
      }
   },
   computed: {
      ...mapGetters(['getProfile']),
      pageCount(){
         return Math.ceil(this.getProfile.documents.count / this.query.pageSize);
      },
      items(){
         return this.getProfile?.documents.result;
      },
      userName(){
         return this.getProfile?.user.firstName? this.getProfile.user.firstName : this.getProfile.user.name;
      },
      userCourses(){
         return this.getProfile?.user.courses;
      }
   },
   methods: {
      handleSelects(){
         if(typeof this.selectedModel.itemType === 'object'){
            this.query.documentType = '';
            this.selectedModel.itemType = '';
         }
         if(this.query.documentType !== this.selectedModel.itemType){
            this.query.documentType = this.selectedModel.itemType;
            this.selectedModel.itemCourse = "";
            this.query.course = '';
         }else{
            this.query.documentType = this.selectedModel.itemType;
            this.query.course = this.selectedModel.itemCourse;
         }
         this.query.page = 1;
      },
      menuSelect(value){
         this.selectedModel.itemType = value;
         this.handleSelects()
      },
      updateItems(){
         let type = 'documents'
         let params = {
            page: this.query.page -1,
            pageSize: this.query.pageSize,
            documentType: this.query.documentType,
            course: this.query.course,
         }
         Object.keys(params).forEach((key) => (params[key] === '') && delete params[key]);
         this.globalFunctions.getItems(type,params).then(()=>{
            let scrollDiv = document.getElementById("profileItemsBox").offsetTop;
            window.scrollTo({ top: scrollDiv, behavior: 'smooth'});
         })

      },
      selectChecker(item){
         if(item.value === this.selectedModel.itemType){
            return true;
         }
         if(typeof this.selectedModel.itemType == 'object' && this.selectedModel.itemType.value === item.value){
            return true;
         }
         return false;
      }

   }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

#profileItemsBox{
   width: 100%;
   color: #43425d;
   .profileItemsBox_title{
      .responsive-property(font-size, 18px, null, 16px);
      font-weight: 600;
      @media (max-width: @screen-xs) {
         padding: 10px 14px;
         background: white;
         font-weight: bold;
      }
   }
   .profileItemsBox_filters{
      display: flex;
      justify-content: space-between;
      padding-top: 12px;
      @media (max-width: @screen-xs) {
         flex-direction: row-reverse;
         padding: 0 12px 8px 14px;
         background: white;
      }
      .filterbox{
         max-width: fit-content;
      }
      .filtercourse{
         max-width: 100%;
         flex-grow: 1;
      }
      .filters_menu_btn{
         max-width: 44px;
         max-height: 42px;
         width: 44px;
         height: 42px;
         border-radius: 8px;
         border: solid 1px #ced0dc;
         background-color: white;
         color: #6f6e82;
         .icon{
            font-size: 16px;
         }
      }
      .profileItemsBox_filters_select{
         color: #4d4b69;
         .responsive-property(height, 36px, null, 42px);
         .v-input__control{
            min-height: auto !important;
            display: unset;
            .responsive-property(height, 36px, null, 42px);
            .v-input__slot{
               border-radius: 8px;
               box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
               margin: 0;
               @media (max-width: @screen-xs) {
                  box-shadow: none;
                  border: solid 1px #ced0dc;
               }
               .v-select__slot{
                  font-size: 14px;
                  .v-select__selections{
                     ::placeholder{
                        font-size: 14px;
                        color: #4d4b69;
                     }
                  }
                  .v-input__append-inner{
                     .v-input__icon{
                        &.v-input__icon--append{
                           i{
                              font-size: 6px;
                              color: #43425d;
                           }
                        }
                        &.v-input__icon--clear{
                           i{
                              font-size: 10px;
                              color: #43425d;
                           }
                        }
                     }
                  }
               }
            }
         }
      }
   }
   .profileItemsBox_content_mobile{
      width: 100%;
      padding-bottom: 0;
      .note-block{
         border-radius: unset;
      }
   }
   .profileItemsBox_content{
      width: 100%;
      display: flex;
      flex-flow: row wrap;
      display: grid;
      box-sizing: border-box;
      grid-gap: 14px;
      padding-bottom: 10px;      
      grid-template-columns: repeat(auto-fill, 230px);
      margin-top: 18px;
      .itemCarouselCard{
         border: 1px solid #e0e1e9;
         flex: 0 0 32%;
         width: 230px;
      }
   }
   .profileItemBox_pagination{
      padding-bottom: 10px;
      text-align: center;
      .v-pagination__item{
         background-color: initial !important;
         box-shadow: none !important;
         outline:none;
         &.v-pagination__item--active{
            background-color: initial !important;
            border: none !important;
            border: 1px solid rgb(68, 82, 252) !important;
            outline:none;
            color: black;
         }
      }
      .v-pagination__navigation{
         background-color: initial !important;
         box-shadow: none !important;
         outline:none;
         i{
            transform: scaleX(1)/*rtl:scaleX(-1)*/; 
            color: rgb(68, 82, 252) !important;
            font-size: 16px;
         }
      }
   }
}
</style>