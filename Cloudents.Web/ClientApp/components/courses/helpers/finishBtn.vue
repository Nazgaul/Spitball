<template>
    <v-btn @click="finish()" round-outline class="sb-round sb-outline px-4 elevation-0">
        <span class="caption font-weight-bold text-capitalize" v-language:inner>courses_btn_finished</span>
    </v-btn>
</template>

<script>
    export default {
        name: "finishBtn",
        data() {
            return {

            }
        },
        methods: {
            finish(){
                this.$router.push({name: 'note'})
            }
        },
    }
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
.sb-outline{
    border-radius: 36px;
    border: solid 1px @colorBlue;
    background-color: transparent!important;
    color: @colorBlue!important;

}
</style>