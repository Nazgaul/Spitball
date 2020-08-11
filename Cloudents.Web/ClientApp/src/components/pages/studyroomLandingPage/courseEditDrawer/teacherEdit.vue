<template>
   <v-expansion-panel class="editSection mb-4 elevation-0 rounded teacherEdit">
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
         <v-text-field class="textInputs" color="#4c59ff" value="Ariel Fineshtain">
            <template v-slot:label>
               <div class="inputLabel">
                  Name
               </div>
            </template>
         </v-text-field>

         <v-textarea class="textInputs" auto-grow color="#4c59ff" rows="1" value="I’m a teacher for many years, learn how to effectively search for a quality online teaching job! Many companies are looking for teachers. Find hidden job boards and awesome online teaching opportunities. We'll also talk about how to network on Linked In to find work and how to search for online teaching jobs using keywords." >
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


   },
}
</script>
<style lang="less" src="./editCards.less"></style>
<style lang="less">
   .teacherEdit{
      
   }
</style>