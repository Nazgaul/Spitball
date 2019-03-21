<template>
    <div class="message" :class="{'myMessage': isMine}" v-html="message.text"></div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex'
export default {
    props:{
        message:{
            type:Object
        }
    },
    computed:{
        ...mapGetters(['accountUser']),
        isMine(){
            return this.accountUser.id === this.message.userId
        }
    }
}
</script>

<style lang='less'>
.message{
    text-align: right;
    &.myMessage{
        text-align: left;
    }
}

</style>
