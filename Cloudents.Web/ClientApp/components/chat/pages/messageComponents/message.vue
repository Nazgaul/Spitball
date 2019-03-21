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
    max-width:211px;
    text-align: right;
    margin: 5px 0;
    margin-left: auto;
    margin-right: unset;
    border-radius: 8px 8px 0 8px;
    background-color: #f5f5f5;
    padding: 10px;
    &.myMessage{
        text-align: left;
        margin: 5px 0;
        margin-left: unset;
        margin-right: auto;
        background-color: #dfdeff;
        border-radius: 8px 8px 8px 0;
    }
}

</style>
