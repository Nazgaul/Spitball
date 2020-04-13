<template>
    <v-snackbar
        absolute
        top
        @input="$store.commit('clearComponent')"
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