<template>
  <div>
    <sbDialog
      :activateOverlay="false"
      :showDialog="getShowEditDataDialog"
      :maxWidth="'760px'"
      :popUpType="'editUserInfo'"
      :content-class="'edit-dialog'"
      :isPersistent="true">

      <tutorInfoEdit v-if="isTutor"/>
      <userInfoEdit v-else></userInfoEdit>
    </sbDialog>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import tutorInfoEdit from '../../profileHelpers/userInfoEdit/tutorInfoEdit.vue';
import userInfoEdit from '../../profileHelpers/userInfoEdit/userInfoEdit.vue';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue'

export default {
    components:{tutorInfoEdit,userInfoEdit,sbDialog},
    data() {
      return {
  
      }
    },
    computed: {
      ...mapGetters(['getShowEditDataDialog','getProfile']),
      isTutor(){
        return !!this.getProfile && this.getProfile.user.isTutor
      },  
    },
}
</script>

<style>

</style>