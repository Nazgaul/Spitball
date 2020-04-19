<template>
    <v-snackbar
        absolute
        top
        @input="$store.commit('clearComponent')"
        class="counterToaster"
        :timeout="0"
        :value="true"
    >
        <span class="mr-2" v-t="'studyRoom_toaster_session_date'"></span>
        <sessionStartCounter @updateCounterFinish="$store.commit('clearComponent')" />
    </v-snackbar>
</template>


<script>
import sessionStartCounter from '../../../studyroom/tutorHelpers/sessionStartCounter/sessionStartCounter.vue'
export default {
    name: '',
    components: {
        sessionStartCounter
    }
}
</script>

<style lang="less">
    .counterToaster {
        // TODO: @maor @idan study room removed mobile-not-support css style
        z-index: 99999;
    }
</style>