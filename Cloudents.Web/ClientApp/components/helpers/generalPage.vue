<template functional>
    <div class="p-result">
        <slot name="banner"></slot>
        <v-container fluid class="pa-0 wrap">
            <v-layout row>
                <v-flex class="first-grid hidden-xs-only left-side-bar">
                    <slot name="sideBar">
                    </slot>
                </v-flex>
                <v-layout class="main-section" row>
                    <v-flex class="sec-result">
                        <slot name="main"></slot>
                    </v-flex>
                    <v-flex class="ml-2 side-bar" v-if="props.breakPointSideBar">
                        <slot name="rightSide">
                            <!--<div class="side-bar-wrapper">-->
                                <!--<adsense ad-client="ca-pub-1215688692145777"-->
                                         <!--ad-style="display:inline-block;width:300px;height:250px"-->
                                         <!--ad-slot="5350268955"-->
                                         <!--data-ad-format="rectangle"-->
                                         <!--class="mb-2 order-lg1 side-element">-->
                                <!--</adsense>-->
                                <!--<slot name="suggestCell"></slot>-->
                                <!--<adsense ad-client="ca-pub-1215688692145777"-->
                                         <!--data-ad-format="rectangle"-->
                                         <!--ad-style="display:inline-block;width:300px;height:250px"-->
                                         <!--ad-slot="3866041406" class="order-lg3 side-element">-->
                                <!--</adsense>-->
                            <!--</div>-->
                        </slot>
                    </v-flex>
                </v-layout>
            </v-layout>
        </v-container>
    </div>
</template>
<style lang="less" src="./generalPage.less"></style>
