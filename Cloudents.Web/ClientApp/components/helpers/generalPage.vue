<template functional>
    <div style="height: 100%;">
        <div class="p-result">
            <v-container fluid class="pa-0 wrap">
                <v-layout class="p-result-layout">
                    <v-flex class="first-grid hidden-xs-only left-side-bar">
                        <slot name="sideBar">
                        </slot>
                    </v-flex>
                    <v-layout class="main-section" :class="{'justify-center': props.mdAndDown}">
                        <v-flex class="sec-result" :class="[props.name ==='tutor' ? 'tutor-result'  : '']">
                            <slot name="main"></slot>
                        </v-flex>
                        <v-flex :class="['side-bar','hidden-md-and-down']" v-if="props.breakPointSideBar && props.name !== 'tutor'">
                            <slot name="rightSide"></slot>
                        </v-flex>
                    </v-layout>
                </v-layout>
            </v-container>
        </div>
    </div>
</template>
<style lang="less" src="./generalPage.less"></style>
