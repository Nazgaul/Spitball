<template>
   <v-expansion-panel @click="goTo" class="editSection mb-4 elevation-0 rounded classEdit">
      <v-expansion-panel-header class="pa-3">
         <div class="editHeader d-flex justify-space-between align-center">
            {{$t('content_edit')}}
         </div>
         </v-expansion-panel-header>
      <v-expansion-panel-content>
         <v-textarea class="textInputs" auto-grow color="#4c59ff" rows="1" v-model="contentTitle">
            <template v-slot:label>
               <div class="inputLabel" v-t="'content_title'"/>
            </template>
         </v-textarea>
         <v-textarea class="textInputs" auto-grow color="#4c59ff" rows="1" v-model="contentText">
            <template v-slot:label>
               <div class="inputLabel" v-t="'content_text'"/>
            </template>
         </v-textarea>
      </v-expansion-panel-content>
   </v-expansion-panel>
</template>

<script>
export default {
   computed: {
      itemList(){
         return this.$store.getters.getCourseItems
      },
      isCourseEnrolled(){
         return this.$store.getters.getIsCourseEnrolled;
      },
      contentTitle:{
         get(){
            return this.$store.getters.getCourseItemsContentTitlePreview || this.isCourseEnrolled? this.$t('courseItemsTitle_enrolled') : this.$t('courseItemsTitle');
         },
         set(val){
            this.$store.commit('setEditedDetailsByType',{
               type:'contentTitle',
               val
            })
         }
      },
      contentText:{
         get(){
            return this.$store.getters.getCourseItemsContentTextPreview || this.$t('courseItemsAccsess')
         },
         set(val){
            this.$store.commit('setEditedDetailsByType',{
               type:'contentText',
               val
            })
         }
      },

   },
   methods: {
      goTo(e){
         if(!e.currentTarget.classList.toString().includes('--active')){
            this.$vuetify.goTo('#courseContentSection',{
               duration: 1000,
               offset: 10,
               easing:'easeInOutCubic',
            })
         }
      },
   },
}
</script>
<style lang="less" src="./editCards.less"></style>
<style lang="less">
   .classEdit{
      
   }
</style>