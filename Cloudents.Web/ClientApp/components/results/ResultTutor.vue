<template v-once>
    <a class="d-block tutor-cell" :target="$vuetify.breakpoint.xsOnly?'_self':'_blank'" :href="item.url">
        <v-container class="pa-0" @click="$ga.event('Search_Results', 'Tutors',`#${index+1}_${item.source}`)">
            <v-flex class="q-price" v-if="item.fee"><span class="price">${{item.fee}} / hour</span></v-flex>

            <v-layout row class="result-cell-content">
                <avatar class="mr-3" :fullname="item.name" :image="item.image" :radius="0" :size="88"></avatar>
                <v-flex>
                    <v-container class="pa-0 full-height">
                        <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0">
                                <v-container class="pa-0">
                                    <v-layout row>

                                        <v-flex class="tutor-title-wrap">
                                            <span class="tutor-title">{{item.name}}</span>
                                        </v-flex>
                                    </v-layout>
                                    <div class="description-wrap" v-if="item.description">
                                        <span class="description">{{item.description}}</span>
                                    </div>

                                </v-container>
                            </v-flex>
                            <v-layout class="bottom-wrap" column>
                                <v-flex class="location text-xs-left" v-if="item.city"><span class="bottom">{{item.city}}, {{item.state}}</span></v-flex>
                                <v-flex class="pa-0 location" v-if="item.online">
                                    <span class="bottom">Offers online lessons</span>
                                </v-flex>
                            </v-layout>
                        </v-layout>
                    </v-container>
                </v-flex>
            </v-layout>
        </v-container>
    </a>
</template>
<script>
    import Avatar from 'vue-avatar-component'
    export default {
        components: {
            Avatar
        },
        props: { item: { type: Object, required: true }, index: { Number } }
    }
</script>
<style src="./ResultTutor.less" lang="less">

</style>
