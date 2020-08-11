<template>
   <v-expansion-panel class="editSection mb-4 elevation-0 rounded classEdit">
      <v-expansion-panel-header class="pa-3">
         <div class="editHeader d-flex justify-space-between align-center">
            {{$t('content_edit')}}
         </div>
         </v-expansion-panel-header>
      <v-expansion-panel-content>
         <v-textarea class="textInputs" auto-grow color="#4c59ff" rows="1" value="Take a deep dive and try our list of over 40 uniq" >
            <template v-slot:label>
                  <div class="inputLabel">
                     Title
                  </div>
            </template>
         </v-textarea>
         <v-textarea class="textInputs" auto-grow color="#4c59ff" rows="1" value="Lorem ipsum dolor sit amet, consectetur piscing elit, sed do eiusmod tempor incididunt ut labo." >
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
      itemList(){
         return this.$store.getters.getCourseItems
      }

   },
}
</script>
<style lang="less" src="./editCards.less"></style>
<style lang="less">
   .classEdit{
      
   }
</style>