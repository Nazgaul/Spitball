<template>
   <v-dialog :value="true" max-width="840px" content-class="createStudyRoomDialog" :fullscreen="$vuetify.breakpoint.xsOnly">
         choose a student:
         <v-select
            :items="myFollowers"
            v-model="selectedStudent">
            <template slot="selection" slot-scope="data">{{data.item.name}}</template>
            <template slot="item" slot-scope="item">{{item.item.name}}</template>
         </v-select>
         <v-btn :loading="isLoading" @click="createStudyRoom" color="success">create</v-btn>
   </v-dialog>
</template>

<script>
export default {
   name:'createStudyRoom',
   data() {
      return {
         selectedStudent:null,
         isLoading:false,
      }
   },
   methods: {
      createStudyRoom(){
         if(!this.isLoading){
            this.isLoading = true
            let self = this;
            this.$store.dispatch('updateCreateStudyRoom',this.selectedStudent.userId)
               .then(() => {
                  self.isLoading = false;
                  this.$store.dispatch('updateFollowersItems')
                  self.$closeDialog()
               });
         }
      }
   },
   computed: {
      myFollowers(){
         return this.$store.getters.getFollowersItems;
      },
   },
   created() {
      this.$store.dispatch('updateFollowersItems')
   },

}
</script>

<style lang="less">
.createStudyRoomDialog{
   background: white;
}
</style>