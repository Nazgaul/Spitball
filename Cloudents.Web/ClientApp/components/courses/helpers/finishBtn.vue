<template>
    <v-btn sel="finished_courses" @click="finish()" round-outline class="sb-round sb-outline px-6 elevation-0">
        <span class="font-weight-bold text-capitalize" v-t>courses_btn_finished</span>
    </v-btn>
</template>

<script>
    export default {
        name: "finishBtn",
        methods: {
            finish(){
                this.$router.push('/')
            }
        },
    }
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
.sb-outline{
    border-radius: 36px;
    border: solid 1px @global-blue;
    background-color: transparent!important;
    color: @global-blue!important;
    margin: 6px 8px;
    span{
        font-size: 14px!important;
    }
}
</style>