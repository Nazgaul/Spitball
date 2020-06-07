<template>
    <div style="height: 100%;">
        <div class="p-result">
            <v-container fluid class="pa-0 wrap">
                <v-layout class="p-result-layout">
                    <v-layout class="main-section" :class="{'justify-center': mdAndDown}">
                        <div class="sec-result" :class="[name ==='tutor' ? 'tutor-result'  : '']">
                            <slot name="main"></slot>
                        </div>
                        <div :class="['side-bar','hidden-md-and-down']" v-if="breakPointSideBar && name !== 'tutor'">
                            <slot name="rightSide"></slot>
                        </div>
                    </v-layout>
                </v-layout>
            </v-container>
        </div>
    </div>
</template>
<script>
export default {
    props:['mdAndDown','breakPointSideBar','name']
}
</script>
<style lang="less" src="./generalPage.less"></style>
