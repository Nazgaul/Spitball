<template>
        <main-header :userText="userText" :submitRoute="submitRoute" :currentSelection="currentSelection" :toolbarHeight="$vuetify.breakpoint.mdAndUp ? 120 : 152">
            <verticals-tabs :currentSelection="currentSelection" slot="extraHeader"></verticals-tabs>
            <!--<v-flex class="line" slot="extraHeader">-->
                <!--<v-layout row>-->
                    <!--<div class="gap ma-0" v-if="$vuetify.breakpoint.mdAndUp"></div>-->
                    <!--&lt;!&ndash;<nav-bar></nav-bar>&ndash;&gt;-->
                    <!--<v-tabs class="verticals-bar" :value="currentSelection" :scrollable="false">-->
                        <!--<v-tabs-bar>-->
                            <!--<v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_updateType(tab.id)" :class="['spitball-text-'+tab.id,tab.id===currentSelection?'tabs__item&#45;&#45;active':'']"-->
                                         <!--class="mr-4 vertical">-->
                                <!--{{tab.name}}-->
                            <!--</v-tabs-item>-->
                            <!--<v-tabs-slider :color="`color-${currentSelection}`"></v-tabs-slider>-->
                        <!--</v-tabs-bar>-->
                    <!--</v-tabs>-->
                <!--</v-layout>-->
            <!--</v-flex>-->
        </main-header>
</template>
<script src="./header.js"></script>


