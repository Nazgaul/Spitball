<template v-once>
    <a class="d-block pa-2" :target="$vuetify.breakpoint.xsOnly?'_self':'_blank'" :href="item.url">
        <v-container class="pa-0">
            <v-layout row>
                <avatar class="mr-2" :fullname="item.name" :image="item.image" :radius="0" :size="88"></avatar>
                <v-flex>
                    <v-container class="pa-0 full-height">
                        <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0">
                                <v-container class="pa-0">
                                    <v-layout row>
                                        <v-flex xs8 class="tutor-title">{{item.name}}</v-flex>
                                        <v-flex xs4 v-if="item.fee"><div class="price text-xs-right">${{item.fee}} / hour</div></v-flex>
                                    </v-layout>
                                        <div class="description" v-if="item.description">{{item.description}}</div>
                                    
                                </v-container>
                            </v-flex>
                            <v-layout class="bottom" row justify-space-between align-item-center>
                                <v-flex class="location" v-if="item.city">{{item.city}}, {{item.state}}</v-flex>
                                <v-flex class="pa-0 location" v-if="item.online">
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
<style src="./ResultTutor.less" lang="less" scoped>
   
</style>
