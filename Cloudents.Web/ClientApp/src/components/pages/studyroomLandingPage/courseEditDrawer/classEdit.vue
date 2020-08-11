<template>
   <v-expansion-panel class="editSection mb-4 elevation-0 rounded contentEdit">
      <v-expansion-panel-header class="pa-3">
         <div class="editHeader d-flex justify-space-between align-center">
            {{$t('class_list')}}
         </div>
         </v-expansion-panel-header>
      <v-expansion-panel-content>
         <v-textarea v-for="(session, index) in sessionsList" :key="index" :value="session.name" class="textInputs" auto-grow color="#4c59ff" rows="1" >
            <template v-slot:label>
                  <div class="inputLabel">
                     Class {{index+1}} title
                  </div>
            </template>
         </v-textarea>
      </v-expansion-panel-content>
   </v-expansion-panel>
</template>

<script>
export default {
   computed: {
      sessionsList(){
         return this.$store.getters.getCourseSessions
      }

   },
}
</script>
<style lang="less" src="./editCards.less"></style>
<style lang="less">
   .contentEdit{
      
   }
</style>