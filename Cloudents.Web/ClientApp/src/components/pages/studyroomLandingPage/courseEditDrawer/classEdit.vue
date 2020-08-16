<template>
   <v-expansion-panel @click="goTo" class="editSection mb-4 elevation-0 rounded">
      <v-expansion-panel-header class="pa-3">
         <div class="editHeader d-flex justify-space-between align-center">
            {{$t('class_list')}}
         </div>
      </v-expansion-panel-header>
      <v-expansion-panel-content class="pb-3">
         <v-textarea v-for="(session, index) in sessionsList" :key="index" 
            class="textInputs mt-4" auto-grow color="#4c59ff" rows="1" :value="session.name" @input="editClassName($event,session)">
            <template v-slot:label>
               <div class="inputLabel">{{$t('class_title',[index+1])}}</div>
            </template>
         </v-textarea>
      </v-expansion-panel-content>
   </v-expansion-panel>
</template>

<script>
      
export default {
   data() {
      return {
         editedRef:[]
      }
   },
   computed: {
      sessionsList(){
         return this.$store.getters.getCourseSessionsPreview
      }
   },
   methods: {
      editClassName(newName,session){
         let idx;
         let currentSession = this.editedRef.find((s,i)=>{
            if(s.id === session.id){
               idx = i;
               return true;
            }
         })
         if(currentSession){
            this.editedRef[idx].name = newName;
         }else{
            this.editedRef.push({...session,name:newName})
         }
         this.$store.commit('setEditedDetailsByType',{
            type:'studyRooms',
            val: this.editedRef
         })
      },
      goTo(e){
         if(!e.currentTarget.classList.toString().includes('--active')){
            this.$vuetify.goTo('#courseSessionSection',{
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
