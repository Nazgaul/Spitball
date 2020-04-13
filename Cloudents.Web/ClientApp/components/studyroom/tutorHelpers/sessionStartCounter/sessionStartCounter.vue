<template>
    <span>{{time.days}}:{{time.hours}}:{{time.minutes}}:{{time.seconds}}</span>
</template>

<script>
export default {
    data() {
        return {
            time:{
                days:'00',
                hours:'00',
                minutes:'00',
                seconds:'00'
            }
        }
    },
    computed: {
        sessionTime(){
            let end = this.$store.getters.getSessionTimeEnd;
            let start = this.$store.getters.getSessionTimeStart;
            return this.getTimeFromMs(end - start);
        },
    },
    methods: {
        getTimeFromMs(mills){
            let ms = 1000*Math.round(mills/1000); // round to nearest second
            let d = new Date(ms);
            return (`${d.getUTCHours()}:${d.getUTCMinutes()}:${d.getUTCSeconds()} `)
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
                // this.isRoomActive = true;
                this.$emit('updateRoomisActive', true)
            }
        }
    },
    beforeDestroy(){
        this.isLoading = false;
        // this.setSesionClickedOnce(false);
        global.onbeforeunload = function() {}
    },
    created() {
        if(this.$store.getters.getRoomIsBroadcast) {
            this.setParamsInterval();
        } else{
            this.isRoomNow = true;
        }
    },
}
</script>