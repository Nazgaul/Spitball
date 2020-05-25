<template>
   <v-flex xs12 sm6 md4 class="conversationsContainer">
      <div class="conversationsHeader d-flex align-center flex-grow-0 flex-shrink-0">
         <span class="cHeaderTitle">
            <v-icon class="mr-2" size="20">sbf-message-icon</v-icon>
            {{$t("chat_messages")}}
         </span>
      </div>
      <v-layout row class="conversationsActions mx-3 justify-space-between">
         <v-flex xs12 class="pt-3">
            <v-text-field class="searchChat" v-model="filter.keyWord"
               solo flat rounded height="38px" dense
               prepend-inner-icon="sbf-search"
               autocomplete="off"
               hide-details
               :label="$t('chat_search_placeholder')"
            ></v-text-field>
         </v-flex>
         <div class="d-flex justify-space-between">            
            <v-flex class="flex-grow-1" :class="[{'pr-3':isTutor}]">
               <v-select class="filterSelect ma-0"
                  :append-icon="'sbf-arrow-fill'" 
                  :items="[{name: $t('chat_show_all'),value:true},{name:$t('chat_show_unread'),value:false}]"
                  item-text="name"
                  v-model="filter.isShowAll"
                  flat hide-details height="38" dense rounded/>
            </v-flex>
            <v-flex v-if="isTutor" class="flex-grow-0 flex-shrink-0">
               <v-btn @click="updateCreateGroupDialogState(true)" class="createBtn pl-1 pr-2" height="40" color="#4452fc" rounded outlined>
                  <v-icon class="pr-1" size="16">sbf-plus-regular</v-icon>
                  <span>{{$t(isMobile?'chat_create_mobile':'chat_create')}}</span>
               </v-btn>
            </v-flex>
         </div>
      </v-layout>
      <v-sheet class="conversationsList d-flex flex-grow-1">
         <conversations :filterOptions="filter"></conversations>
      </v-sheet>
      <createGroupChatDialog v-if="createGroupDialogState" 
            @updateCreateGroupDialogState="updateCreateGroupDialogState" 
            :dialogState="createGroupDialogState"/>
   </v-flex>
</template>

<script>
const conversations = () => import('../../../chat/components/conversations.vue');
const createGroupChatDialog = () => import('./createGroupChatDialog.vue');
export default {
   components:{
      conversations,
      createGroupChatDialog
   },
   data() {
      return {
         createGroupDialogState:false,
         filter:{
            keyWord:'',
            isShowAll:true,
         }
      }
   },
   computed: {
      isMobile(){
         return this.$vuetify.breakpoint.smAndDown
      },
      isTutor(){
         return this.$store.getters.getIsTutorState;
      }
   },
   methods: {
      updateCreateGroupDialogState(val){
         this.createGroupDialogState = val;
      }
   },
}
</script>

<style lang="less">
   @import '../../../../styles/mixin.less';
   @headerHeight: 62px;
   @media(max-width: @screen-xs) {
      @headerHeight: 60px;
   }
   .conversationsContainer{
      height: 100%;
      background: #ffffff;
      .conversationsHeader{
         width: 100%;
         height: @headerHeight;
         @media(max-width: @screen-xs) {
            background-color: #4c59ff;
            border: none;
         }
         background-color: #efefef;
         border-right: 1px solid #e4e4e4;
         border-bottom: 1px solid #e4e4e4;
         .cHeaderTitle{
            font-size: 16px;
            font-weight: 600;
            color: #43425d;
            padding-left: 18px;
            @media(max-width: @screen-xs) {
               color: #ffffff;
               .v-icon{
                  color: #ffffff;
               }
            }
         }
      }
      .conversationsActions{
         // background-color: khaki;
         height: 116px;
         .searchChat{
            border: solid 1px #ced0dc;
            .v-input__slot{
               padding: 0 10px;
               .v-text-field__slot{
                  .v-label{
                     padding-left: 4px;
                     font-size: 14px;
                     color: #43425d;
                  }
               }
               .v-icon{
                  color: #69687d;
                  font-size: 22px;
               }
            }
         }
         .filterSelect{
            border: solid 1px #ced0dc;
            color: #69687d;
            .v-input__slot{
               padding: 0 14px;
               .v-select__selections{
                  font-size: 14px;
                  font-weight: 600;
                  color: #4d4b69;
                  input{
                     &::placeholder{
                        font-size: 14px;
                        font-weight: 600;
                        color: #4d4b69;
                     }
                  }
               }
            }
            .v-icon{
               font-size: 7px;
            }
         }
         .createBtn{
            font-size: 14px;
            font-weight: 600;
         }

      }
      .conversationsList{
         height: calc(~"100% - 178px"); // header || mobile footer height & search and sort height
      }
   }
</style>