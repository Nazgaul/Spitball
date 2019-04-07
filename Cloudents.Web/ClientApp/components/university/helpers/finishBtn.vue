<template>
    <v-btn @click="finish()" round-outline class="sb-round sb-outline px-4">
        <span class="caption font-weight-bold text-capitalize">Finished</span>
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
                this.$router.go(-1)
            }
        },
    }
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
.sb-outline{
    border-radius: 16px;
    border: solid 1px @colorBlue;
    background-color: transparent!important;
    color: @colorBlue!important;
}
</style>