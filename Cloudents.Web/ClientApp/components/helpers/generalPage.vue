<template functional>
    <component is="slot">
        <slot></slot>
        <v-content class="p-result">
            <v-container fluid class="pa-0 wrap">
                <v-layout row>
                    <v-flex class="filters-section first-grid hidden-sm-and-down">
                        <slot name="options">
                        </slot>
                    </v-flex>
                    <v-flex class="results-section">
                        <v-layout column>
                            <v-flex class="first-grid hidden-sm-and-down">
                            </v-flex>
                            <v-flex class="navbar-wrapper">
                                <slot name="verticalNavbar"></slot>
                                <!--<app-menu :term="userText"></app-menu>-->
                            </v-flex>
                            <v-container fluid class="pa-2">
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
                    </v-flex>
                </v-layout>
            </v-container>
        </v-content>
    </component>
</template>
<style lang="less" src="./generalPage.less"></style>
