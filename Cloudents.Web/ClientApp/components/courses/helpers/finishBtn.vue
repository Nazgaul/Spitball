<template>
    <v-btn sel="finished_courses" @click="finish()" round-outline class="sb-round sb-outline px-4 elevation-0">
        <span class="caption font-weight-bold text-capitalize" v-language:inner>courses_btn_finished</span>
    </v-btn>
</template>

<script>
import {mapMutations} from 'vuex'
    export default {
        name: "finishBtn",
        data() {
            return {

            }
        },
        methods: {
            ...mapMutations(['UPDATE_SEARCH_LOADING']),
            finish(){
                this.UPDATE_SEARCH_LOADING(true);
                this.$router.push({name: 'feed'})
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