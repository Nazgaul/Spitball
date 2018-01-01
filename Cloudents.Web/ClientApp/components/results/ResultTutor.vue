<template v-once>
    <a class="d-block pa-2 tutor-cell" :target="$vuetify.breakpoint.xsOnly?'_self':'_blank'" :href="item.url">
        <v-container class="pa-0">
            <v-layout row>
                <avatar class="mr-2" :fullname="item.name" :image="item.image" :radius="0" :size="88"></avatar>
                <v-flex>
                    <v-container class="pa-0 full-height">
                        <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0">
                                <v-container class="pa-0">
                                    <v-layout row>
                                        <v-flex  class="tutor-title">{{item.name}}</v-flex>
                                        <v-flex  v-if="item.fee"><div class="price text-xs-right">${{item.fee}} / hour</div></v-flex>
                                    </v-layout>
                                        <div class="description" v-if="item.description">{{item.description}}</div>
                                    
                                </v-container>
                            </v-flex>
                            <v-layout class="bottom" row >
                                <v-flex class="location text-xs-left" v-if="item.city">{{item.city}}, {{item.state}}</v-flex>
                                <v-flex class="pa-0 location text-xs-right" v-if="item.online">
                                    Offers online lessons
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
        props: { item: { type: Object, required: true } }
    }
</script>
<style src="./ResultTutor.less" lang="less">
   
</style>
