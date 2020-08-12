<template>
   <v-expansion-panel @click="goTo" class="editSection mb-4 elevation-0 rounded teacherEdit">
      <v-expansion-panel-header class="pa-3">
         <div class="editHeader d-flex justify-space-between align-center">
            {{$t('teacher_edit')}}
         </div>
         </v-expansion-panel-header>
      <v-expansion-panel-content>

         <v-text-field class="textInputs" color="#4c59ff" value="About the teacher">
            <template v-slot:label>
               <div class="inputLabel">
                  Title
               </div>
            </template>
         </v-text-field>
         <v-text-field class="textInputs" color="#4c59ff" v-model="teacherName">
            <template v-slot:label>
               <div class="inputLabel">
                  Name
               </div>
            </template>
         </v-text-field>

         <v-textarea class="textInputs" auto-grow color="#4c59ff" rows="1" v-model="teacherBio">
            <template v-slot:label>
                  <div class="inputLabel">
                     Text
                  </div>
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
}
</script>
<style lang="less" src="./editCards.less"></style>