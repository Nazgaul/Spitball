<template>
   <v-expansion-panel @click="goTo" class="editSection mb-4 elevation-0 rounded teacherEdit">
      <v-expansion-panel-header class="pa-3">
         <div class="editHeader d-flex justify-space-between align-center">
            {{$t('teacher_edit')}}
         </div>
         </v-expansion-panel-header>
      <v-expansion-panel-content>

         <v-text-field class="textInputs my-4" color="#4c59ff" v-model="teacherTitle">
            <template v-slot:label>
               <div class="inputLabel" v-t="'teacher_title'"/>
            </template>
         </v-text-field>

         <v-text-field class="textInputs" color="#4c59ff" v-model="teacherName">
            <template v-slot:label>
               <div class="inputLabel" v-t="'teacher_name'"/>
            </template>
         </v-text-field>

         <v-textarea class="textInputs my-4" auto-grow color="#4c59ff" rows="1" v-model="teacherBio">
            <template v-slot:label>
               <div class="inputLabel" v-t="'teacher_text'"/>
            </template>
         </v-textarea>
      </v-expansion-panel-content>
   </v-expansion-panel>
</template>

<script>
export default {
   computed: {
      teacherName:{
         get(){
            return this.$store.getters.getCourseTeacherNamePreview;
         },
         set(val){
            this.$store.commit('setEditedDetailsByType',{
               type:'tutorName',
               val
            })
         }
      },
      teacherBio:{
         get(){
            return this.$store.getters.getCourseTeacherBioPreview;
         },
         set(val){
            this.$store.commit('setEditedDetailsByType',{
               type:'tutorBio',
               val
            })
         }
      },
      teacherTitle:{
         get(){
            return this.$store.getters.getCourseTeacherTitlePreview || this.$t('about_host');
         },
         set(val){
            this.$store.commit('setEditedDetailsByType',{
               type:'teacherTitle',
               val
            })
         }
      },
   },
   methods: {
      goTo(e){
         if(!e.currentTarget.classList.toString().includes('--active')){
            this.$vuetify.goTo('#courseTeacherSection',{
               duration: 1000,
               offset: 10,
               easing:'easeInOutCubic',
            })
         }
      },
   },
   mounted() {
      let title = this.teacherTitle;
      this.teacherTitle = title;
   }
}
</script>
<style lang="less" src="./editCards.less"></style>