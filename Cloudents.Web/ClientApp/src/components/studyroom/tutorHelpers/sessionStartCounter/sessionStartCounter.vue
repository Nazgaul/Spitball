<template>
    <div class="counterComponent">
        <span>{{time.days}}</span><span class="counterDots">:</span><span>{{time.hours}}</span><span class="counterDots">:</span><span>{{time.minutes}}</span><span class="counterDots">:</span><span>{{time.seconds}}</span>
    </div>
</template>

<script>
export default {
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
        setParamsInterval(){
            this.interVal = setInterval(this.getNow, 1000);
            this.getNow();
        },
        getNow() {
            let countDownDate = new Date(this.$store.getters.getRoomDate || this.$store.getters.getCourseDetails?.startTime).getTime();
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
            if (distance < minute * 10){
                this.$emit('updateCounterMinsLeft', true)
            }
            if (distance < 0) {
                clearInterval(this.interVal);
                this.$emit('updateCounterFinish', true)
            }
        }
    },
    beforeDestroy(){
        this.isLoading = false;
    },
    created() {
        if(this.$store.getters.getRoomIsBroadcast || this.$store.getters.getCourseDetails?.startTime) {
            this.setParamsInterval();
        } else{
            this.$emit('updateCounterMinsLeft', true)
            this.$emit('updateCounterFinish', true)
        }
    },
}
</script>
<style lang="less">
.counterComponent{
    text-align: left;
    /*rtl:ignore */
    direction: ltr;
}
</style>