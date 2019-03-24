<template>
    <v-layout class="user-online-status-wrap">
            <div :class="['status-container', isOnline ? 'online' : 'offline']">
                <span  v-if="isOnline" class="online">Online</span>
                <span v-else class="offline">Offline</span>
            </div>

    </v-layout>
</template>
<script>
    export default {
        name: "userOnlineStatus",
        props: {
            isOnline: {
                type: Boolean,
                default: false
            },
        },
    }
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';
    .user-online-status-wrap{
        .status-container{
            border-radius: 8.5px;
            padding: 2px 14px;
            @media(max-width: @screen-xs){
                padding: 2px 8px;
            }
            &.online{
                background-color: #6aff70;
            }
            &.offline{
                background-color: @color-white;
            }
        }
        .online{
            font-family: @fontOpenSans;
            font-size: 12px;
            font-weight: 600;
            color:@profileTextColor;
            @media(max-width: @screen-xs){
                font-size: 10px;
            }
        }
        .offline{

        }
    }

</style>