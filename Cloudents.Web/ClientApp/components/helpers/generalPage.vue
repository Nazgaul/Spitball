<template functional>
    <component is="slot">
        <slot></slot>
        <v-content class="p-result">
        <v-container fluid class="pa-0 wrap">
            <v-layout row>
                <v-flex class="first-grid hidden-sm-and-down">
                </v-flex>
                <v-flex>
                    <slot name="verticalNavbar"></slot>
                    <div>
                        <template v-for="item in props.filterSelection"><slot name="selectedFilters" :item="item"></slot></template>
                    </div>
                    <!--<app-menu :term="userText"></app-menu>-->
                </v-flex>
            </v-layout>
            <v-layout row class="pt-3">
                <v-flex class="first-grid hidden-sm-and-down">
                    <slot name="options">
                    </slot>
                </v-flex>
                <v-container class="results-section px-2" fluid>
                    <v-layout row>
                        <v-flex class="sec-result">
                            <slot name="data">
                            </slot>
                        </v-flex>
                        <v-flex class="ml-2 hidden-md-only hidden-xs-only side-bar">
                            <slot name="adsense">
                                <adsense ad-client="ca-pub-1215688692145777"
                                         ad-slot="3866041406"
                                         ad-style="display: block; width:336px; height:280px;"
                                         ad-format="auto" class="mb-2">
                                </adsense>
                                <adsense ad-client="ca-pub-1215688692145777"
                                         ad-slot="5547053037"
                                         ad-style="display: block; width:336px;height:280px">
                                </adsense>
                            </slot>
                        </v-flex>
                    </v-layout>
                </v-container>
                <!--</v-flex>-->
            </v-layout>
        </v-container>
    </v-content></component>
</template>
<style lang="less" src="./generalPage.less"></style>
