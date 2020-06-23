<template>
  <div>
    <sbDialog
      :activateOverlay="false"
      :showDialog="$store.getters.getShowEditDataDialog"
      :maxWidth="'760px'"
      :popUpType="'editUserInfo'"
      :content-class="'edit-dialog'"
      :isPersistent="true">
      <tutorInfoEdit />
    </sbDialog>
  </div>
</template>

<script>
import tutorInfoEdit from '../../profileHelpers/userInfoEdit/tutorInfoEdit.vue';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue'

export default {
  components:{tutorInfoEdit,sbDialog}
}
</script>