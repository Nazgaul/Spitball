<template>
    <div class="settingDetailsWrap ml-12">
        <div class="mb-12 settingDetails">
            <div class="mb-1" v-for="(item, index) in roomDetails" :key="index">
                <span class="detailName mr-2" v-t="item.text"></span>
                <span class="detailValue">{{item.value}}</span>
            </div>
        </div>

        <div class="counterWrap text-center">
            <template>
                <div class="mb-8" v-if="!isRoomActive">
                    <div v-t="'studyRoomSettings_clock_counter'"></div>
                    <sessionStartCounter @updateCounterFinish="$emit('updateRoomIsActive', true)" />
                </div>
                <div class="mb-8" v-else v-t="'studyRoomSettings_ready'"></div>
            </template>
            <v-btn 
                class="joinNow white--text px-8"
                @click="$store.dispatch('updateEnterRoom', id)"
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
        isRoomActive: {
            type: Boolean,
            default: false,
            required: true
        }
    },
    computed: {
        roomDetails() {
            return [
                { text: 'studyRoomSettings_room_name', value: this.roomName },
                { text: 'studyRoomSettings_tutor_name', value: this.roomTutor.tutorName },
                { text: 'studyRoomSettings_price', value: this.roomTutor.tutorPrice },
                { text: 'studyRoomSettings_schedule_date', value: '' },
                { text: 'studyRoomSettings_room_link', value: this.roomLink },
            ]
        },
        roomName() {
            return this.$store.getters?.getRoomName
        },
        roomTutor() {
            return this.$store.getters?.getRoomTutor
        },
        roomLink() {
            // @idan - I think this better approach getting the room id with $route.params instead of passing props
            // TODO: Make room link getter from store
            return `${window.origin}/studyroom/${this.$route.params.id}`
        }
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.settingDetailsWrap {
    max-width: 400px;
    color: @global-purple;
    .settingDetails {
        .detailName {
            font-size: 16px;
            font-weight: 600;
        }
        .detailValue {
        }
    }

    .counterWrap {
        font-weight: 600;
        .joinNow {
            font-weight: 600;
        }
    }
}
</style>