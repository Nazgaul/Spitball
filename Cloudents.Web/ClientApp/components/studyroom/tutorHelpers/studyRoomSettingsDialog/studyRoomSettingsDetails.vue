<template>
    <div class="settingDetailsWrap ml-12">
        <div class="mb-12 settingDetails">
            <div>
                <span class="mr-2" v-t="'studyRoomSettings_room_name'"></span>
                <span>{{roomName}}</span>
            </div>
            <div>
                <span class="mr-2" v-t="'studyRoomSettings_tutor_name'"></span>
                <span>{{roomTutor.tutorName}}</span>
            </div>
            <div>
                <span class="mr-2" v-t="'studyRoomSettings_price'"></span>
                <span>{{roomTutor.tutorPrice}}</span>
            </div>
            <div>
                <span class="mr-2" v-t="'studyRoomSettings_schedule_date'"></span>
                <span></span>
            </div>
            <div>
                <span class="mr-2" v-t="'studyRoomSettings_room_link'"></span>
                <span>{{roomLink}}</span>
            </div>
        </div>

        <div class="text-center">
            <div class="mb-8" v-show="!isRoomActive">
                <div v-t="'studyRoomSettings_clock_counter'"></div>
                <sessionStartCounter @updateRoomisActive="val => isRoomActive = val" />
            </div>
            <v-btn 
                class="joinNow white--text px-8"
                @click="startSession"
                :disabled="!isRoomActive"
                height="50"
                color="#5360FC"
                rounded
                depressed
            >
                {{$t('studyRoomSettings_join_now')}}
            </v-btn>
        </div>
    </div>
</template>

<script>
import sessionStartCounter from '../sessionStartCounter/sessionStartCounter.vue'

export default {
    components: {
        sessionStartCounter
    },
    props: {
        id: {
            type: String,
        },
        isRoomActive: {
            type: Boolean,
            default: false,
            required: true
        }
    },
    data() {
        return {
        }
    }, 
    computed: {
        roomName() {
            return this.$store.getters?.getRoomName
        },
        roomTutor() {
            return this.$store.getters?.getRoomTutor
        },
        roomLink() {
            return `${window.origin}/studyroom/${this.id}`
        }
    },
    methods:{
        startSession(){
            this.$store.dispatch('updateEnterRoom',this.id)
        }
    }
}
</script>