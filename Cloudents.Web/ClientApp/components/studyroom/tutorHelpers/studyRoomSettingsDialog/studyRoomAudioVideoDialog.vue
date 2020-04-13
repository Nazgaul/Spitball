<template>
    <v-dialog :value="true" max-width="680" content-class="studyRoomAudioVideoDialog" @click:outside="$emit('updateSettingDialogState', false)">
settingState
    </v-dialog>
</template>
<script>
export default {
    name: 'studyRoomAudioVideoDialog',
}
</script>
<style lang="less" scoped>
    .studyRoomAudioVideoDialog {
        background: #fff;
    }
</style>