<template>
    <v-layout class="user-online-status-wrap">
            <span v-if="isOnline" class="online-circle" ></span>
    </v-layout>
</template>
<script>
import {mapGetters} from 'vuex'
    export default {
        name: "userOnlineStatus",
        props: {
            userId:{
                type: Number,
                required: true,
                default: -1
            }
        },
        computed:{
            ...mapGetters(['getUserStatus']),
            isOnline(){
                return this.getUserStatus[this.userId] || false;
            }
        }
    }
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    .user-online-status-wrap{
        .online-circle{
            border-radius: 50%;
            width: 10px;
            height: 10px;
            background-color: #00ff14;
        }
    }

</style>