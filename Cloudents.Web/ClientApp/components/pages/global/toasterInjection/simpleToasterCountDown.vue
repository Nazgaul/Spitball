<template>
    <v-snackbar
        absolute
        top
        @input="onCloseToaster"
        class="counterToaster"
        :timeout="3000000"
        :value="true"
    >
        <span>{{$t('studyRoom_toaster_session_date')}} {{time.days}}:{{time.hours}}:{{time.minutes}}:{{time.seconds}}</span>
    </v-snackbar>
</template>


<script>
export default {
    name: '',
    data() {
        return {
            interVal:null,
            time:{
                days:'00',
                hours:'00',
                minutes:'00',
                seconds:'00'
            }
        }
    },
    methods: {
        onCloseToaster() {
            this.$store.commit('clearComponent')
        },
        setParamsInterval(){
            this.interVal = setInterval(this.getNow, 1000);
            this.getNow();
        },
        getNow() {
            let countDownDate = new Date(this.$store.getters.getRoomDate).getTime();
            let now = new Date();
            let distance = countDownDate - now;
            
            const second = 1000;
            const minute = second * 60;
            const hour = minute * 60;
            const day = hour * 24;
            

            this.time.days = Math.floor(distance / (day)).toLocaleString('en-US', {minimumIntegerDigits: 2});
            this.time.hours = Math.floor((distance % (day)) / (hour)).toLocaleString('en-US', {minimumIntegerDigits: 2});
            this.time.minutes = Math.floor((distance % (hour)) / (minute)).toLocaleString('en-US', {minimumIntegerDigits: 2});
            this.time.seconds = Math.floor((distance % (minute)) / second).toLocaleString('en-US', {minimumIntegerDigits: 2});
            if (distance < 0) {
                clearInterval(this.interVal);
                this.$store.commit('clearComponent')
            }
        }
    },
    created() {
        this.setParamsInterval();
    },
}
</script>

<style lang="less">
    .counterToaster {
        z-index: 99999; // for overide studyroom 
    }
</style>