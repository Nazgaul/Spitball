<template functional>
    <div class="p-result">
        <v-container fluid class="pa-0 wrap">
            <v-layout row>
                <v-flex class="first-grid hidden-xs-only left-side-bar">
                    <slot name="options">
                    </slot>
                </v-flex>
                <v-container class="px-2 pt-0 ma-0" fluid style="min-width:0">
                    <v-layout row>
                        <v-flex class="sec-result">
                            <div class="d-flex mobile-filter">
                                <slot name="mobileFilter"></slot>
                            </div>
                            <div v-if="props.filterSelection.length" class="pb-3">
                                <template v-for="item in props.filterSelection">
                                    <slot name="selectedFilters" :item="item"></slot>
                                </template>
                            </div>
                            <slot name="data">
                            </slot>
                        </v-flex>
                        <v-flex v-if="props.breakPointSideBar" class="ml-2 side-bar">
                            <slot name="adsense">
                                <div class="side-bar-wrapper">
                                    <adsense ad-client="ca-pub-1215688692145777"
                                             ad-slot="3866041406"
                                             ad-style="display: block; width:264px; height:224px;"
                                             ad-format="auto" class="mb-2 order-lg1 side-element">
                                    </adsense>
                                    <slot name="suggestCell"></slot>
                                    <adsense ad-client="ca-pub-1215688692145777"
                                             ad-slot="5547053037" class="order-lg3 side-element"
                                             ad-style="display: block; width:264px; height:224px;">
                                    </adsense>
                                </div>
                            </slot>
                        </v-flex>
                    </v-layout>
                </v-container>
            </v-layout>
        </v-container>
    </div>
</template>
<style lang="less" src="./generalPage.less"></style>
